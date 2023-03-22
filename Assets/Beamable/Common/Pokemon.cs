using System.Collections.Generic;
using Newtonsoft.Json;
using System;

[Serializable]
public class GetStatusResponse
{
    public List<PokemonView> ready;
}

[Serializable]
public class PokemonView
{
    public string id;
    public string name;
    public string[] type;
    public string[] abilities;
    public string[] moves;
    public string description;

    public int health;
    public int attack;
    public int defense;
    public int specialAttack;
    public int specialDefense;
    public int speed;
    
    public string imageUrl;
    public long expiresAt;

    public bool IsExpired
    {
        get => expiresAt <= DateTimeOffset.UtcNow.ToUnixTimeSeconds();
    }
}