using System.Collections.Generic;
using UnityEngine;
using Beamable;
using Beamable.Server.Clients;
using System;
using System.Linq;
using Beamable.Player;
using VirtualList;

public class GenamonSpawner : MonoBehaviour
{
    public VirtualVerticalList inventoryList;
    public List<PokemonView> genamonViews;
    public List<GenamonSpawnPoint> spawnPoints;

    private readonly System.Random _random = new();
    private DateTime _nextRefresh = DateTime.MaxValue;

    async void Start()
    {
        var components = FindObjectsOfType<GenamonSpawnPoint>();
        spawnPoints = components.ToList();
        
        var playerOne = BeamContext.Default;
        await playerOne.OnReady;
        await playerOne.Inventory.Refresh();
        
        playerOne.Api.NotificationService.Subscribe("genamon.ready", _ =>
        {
            Debug.Log("Genamon Ready notification received!");
            OnGenamonReady();
        });
        
        var playerGenamon = playerOne.Inventory.GetItems("items.genamon");
        playerGenamon.OnDataUpdated += OnPlayerInventoryUpdated;

        OnPlayerInventoryUpdated(playerGenamon.ToList());
        OnGenamonReady();
    }

    void OnPlayerInventoryUpdated(List<PlayerItem> playerGenamon)
    {
        inventoryList.SetSource(new SimpleSource<PlayerItem, GenamonTileView>(playerGenamon));
    }

    async void Generate()
    {
        _nextRefresh = DateTime.Now.AddMinutes(10);
        
        var playerOne = BeamContext.Default;
        await playerOne.Microservices().GenamonService().Generate();
    }

    async void OnGenamonReady()
    {
        var playerOne = BeamContext.Default;
        var readyRsp = await playerOne.Microservices().GenamonService().GetStatus();
        genamonViews = readyRsp.ready;

        if (genamonViews == null || genamonViews.Count == 0)
        {
            _nextRefresh = DateTime.Now;
            return;
        }

        // Clear the spawn points and reset them
        foreach (var spawnPoint in spawnPoints)
        {
            spawnPoint.Genamon = null;
        }
        
        Debug.Log("Genamon spawning...");
        foreach (var genamonView in genamonViews)
        {
            var available = spawnPoints.FindAll(sp => !sp.HasGenamon);
            if (available.Count == 0)
                break;
            
            var index = _random.Next(available.Count);
            available[index].Genamon = genamonView;
        }
        Debug.Log("Genamon spawned!");
    }
    
    void Update()
    {
        if (_nextRefresh <= DateTime.Now)
        {
            Generate();
        }
    }
}
