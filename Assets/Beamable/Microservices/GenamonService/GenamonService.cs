using Beamable.Microservices.Pokemon.Storage;
using Beamable.Microservices.Pokemon.Storage.Models;
using Beamable.Server;
using Beamable.Server.Api.Notifications;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Beamable.Common.Inventory;
using UnityEngine;

namespace Beamable.Microservices
{
	[Microservice("GenamonService")]
	public class GenamonService : Microservice
	{
		private const string Prompt = "Generate a new pokemon based on the original 150, and provide a character sheet based on the original gameboy games. The character sheet should include name, type (array of human-readable), stats (numeric), abilities (human-readable), moves (human-readable), and description (briefly describe the appearance). The stats should include hp, atk, def, sp.atk, sp.def, and speed. The output should exclusively contain a valid minified json string with lowercase field names.";

        private async Task GeneratePending(IMicroserviceNotificationsApi services, long gamerTag, List<PokemonText> pokemons)
        {
            if(pokemons.Count == 0)
            {
                return;
            }

            var records = new List<PendingPokemon>();
            foreach(var pokemon in pokemons)
            {
                var scenarioPrompt = $"Pokemon,{pokemon.description.Replace(",", "")}";
                var inferenceResponse = await ScenarioGG.CreateInference(scenarioPrompt);

                records.Add(new PendingPokemon
                {
                    Pokemon = pokemon,
                    Inference = inferenceResponse.inference
                });
            }

            var db = await Storage.GenamonStorageDatabase();
            var insertSuccessful = await PendingPokemonCollection.TryInsert(db, records);
            if (!insertSuccessful)
            {
                throw new Exception("Inserting pending pokemon failed!");
            }

            int pollCount = 0;
            bool completed = false;
            var updatedResponses = new ScenarioGG.InferenceResponse[] { };
            while (!completed)
            {
                // Wait 1 second before polling again
                await Task.Delay(1000);

                updatedResponses = await Task.WhenAll(records.Select(record =>
                {
                    return ScenarioGG.GetInference(record.Inference.id);
                }));

                completed = updatedResponses.All(rsp => rsp.inference.IsCompleted);
                pollCount += 1;

                if (pollCount >= 300 && !completed)
                {
                    throw new Exception("Polling timed out after 10 attempts.");
                }
            }

            bool updated = await PendingPokemonCollection.UpdateInferences(db, updatedResponses.Select(rsp => rsp.inference));
            if(updated)
            {
                Debug.Log("Inferences Completed & Updated!");
                await services.NotifyPlayer(gamerTag, "genamon.ready", "");
            }
            else
            {
                Debug.LogWarning("Inferences were not successfully updated.");
            }
        }

        [Callable("status")]
        public async Task<GetStatusResponse> GetStatus()
        {
            var db = await Storage.GenamonStorageDatabase();
            var genamon = await PendingPokemonCollection.GetReadyPokemon(db);
            
            return new GetStatusResponse {
                ready = genamon.Select(g => g.ToView()).ToList()
            };
        }

		[Callable("")]
		public async Task Generate()
		{
            var db = await Storage.GenamonStorageDatabase();
            var pendingPokemon = await PendingPokemonCollection.GetAvailablePokemon(db);
            if(pendingPokemon.Count > 0)
            {
                // Return an error here instead
                return;
            }
            
            var completionResponse = await OpenAI.Send(Prompt);
            var pokemons = new List<PokemonText>();
            foreach (var choice in completionResponse.choices)
            {
                if (choice.finish_reason == "stop")
                {
                    PokemonText pokemon = null;
                    try
                    {
                        pokemon = JsonConvert.DeserializeObject<PokemonText>(choice.text.Trim());
                    }
                    catch(Exception e)
                    {
                        Debug.LogException(e);
                    }
                    finally
                    {
                        if (pokemon != null)
                        {
                            pokemons.Add(pokemon);
                        }
                    }
                }
            }

            await GeneratePending(Services.Notifications, Context.UserId, pokemons);
        }

        [ClientCallable("collect")]
        public async void Collect(string genamonId, ItemRef itemRef)
        {
            var playerId = Context.UserId;
            var inventory = Services.Inventory;
            var notifications = Services.Notifications;
            
            var db = await Storage.GenamonStorageDatabase();
            var genamonList = await PendingPokemonCollection.GetById(db, genamonId);

            if (genamonList.Count > 0)
            {
                var genamon = genamonList[0].ToView();
                await inventory.AddItem(itemRef, new Dictionary<string, string>
                {
                    {"$image", genamon.imageUrl},
                    {"image", genamon.imageUrl},
                    {"imageUrl", genamon.imageUrl}, //workaround
                    {"name", genamon.name},
                    {"$symbol", genamon.name},
                    {"elementalType", string.Join(",", genamon.type)},
                    {"health", genamon.health.ToString()},
                    {"attack", genamon.attack.ToString()},
                    {"defense", genamon.defense.ToString()},
                    {"specialAttack", genamon.specialAttack.ToString()},
                    {"specialDefense", genamon.specialDefense.ToString()},
                    {"speed", genamon.speed.ToString()},
                    {"abilities", string.Join(',', genamon.abilities)},
                    {"moves", string.Join(',', genamon.moves)},
                    {"$description", genamon.description},
                    {"desc", genamon.description}
                });

                await PendingPokemonCollection.DeleteById(db, genamonId);
                await notifications.NotifyPlayer(playerId, "genamon.ready", "");
            }
            else
            {
                throw new MicroserviceException(400, "GenamonUnavailable",
                    "This genamon is no longer available.");
            }
        }
	}
}
