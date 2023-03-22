using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace Beamable.Microservices.Pokemon.Storage.Models
{
	public record PendingPokemon
	{
		[BsonElement("_id")]
		public ObjectId Id { get; set; } = ObjectId.GenerateNewId();
		public DateTime ExpiresAt { get; set; } = DateTime.Now.AddMinutes(10.0);
		public PokemonText Pokemon { get; set; }
		public ScenarioGG.Inference Inference { get; set; }

		public PokemonView ToView()
        {
			string imageUrl = null;
			if(Inference.images.Length > 0) {
				imageUrl = Inference.images[0].url;
			}

			return new PokemonView
			{
				id = Id.ToString(),
				name = Pokemon.name,
				type = Pokemon.type,
				abilities = Pokemon.abilities,
				moves = Pokemon.moves,
				description = Pokemon.description,
				health = Pokemon.stats.health,
				attack = Pokemon.stats.attack,
				defense = Pokemon.stats.defense,
				specialAttack = Pokemon.stats.special_attack,
				specialDefense = Pokemon.stats.special_defense,
				speed = Pokemon.stats.speed,
				imageUrl = imageUrl,
				expiresAt = ((DateTimeOffset)ExpiresAt).ToUnixTimeSeconds()
			};
        }
	}
	
	[Serializable]
	public class PokemonText
	{
		[JsonRequired] public string name;
		[JsonRequired] public string[] type;
		[JsonRequired] public string[] abilities;
		[JsonRequired] public string[] moves;
		[JsonRequired] public string description;
		[JsonRequired] public PokemonStats stats;
	}

	[Serializable]
	public class PokemonStats
	{
		[JsonProperty("hp")] public int health;
		[JsonProperty("atk")] public int attack;
		[JsonProperty("def")] public int defense;
		[JsonProperty("sp.atk")] public int special_attack;
		[JsonProperty("sp.def")] public int special_defense;
		[JsonProperty("speed")] public int speed;
	}
}