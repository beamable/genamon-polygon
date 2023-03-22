using System.Collections;
using Beamable.Player;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using VirtualList;

public class GenamonTileView : MonoBehaviour, IViewFor<PlayerItem>
{
    public TMP_Text genamonNameText;
    public RawImage genamonImage;
    
    [SerializeField] private string _genamonImageUri;
    [SerializeField] private PlayerItem _genamonData;

    public void Set(PlayerItem genamonItem)
    {
        StopCoroutine(DownloadImage());
        _genamonData = genamonItem;

        string genamonName;
        if (_genamonData.Properties.TryGetValue("name", out genamonName))
        {
            genamonNameText.SetText(genamonName);
        }

        string genamonUri;
        if (_genamonData.Properties.TryGetValue("imageUrl", out genamonUri))
        {
            _genamonImageUri = genamonUri;
            StartCoroutine(DownloadImage());
        }
    }

    public void OnClick()
    {
        var panel = FindObjectOfType<GenamonPanel>();
        if (panel != null)
        {
            panel.Open(ToView(), _genamonData.FederatedId.Value);
        }
    }

    private PokemonView ToView()
    {
        return new PokemonView
        {
            name = _genamonData.Properties["name"],
            type = _genamonData.Properties["elementalType"].Split(","),
            abilities = _genamonData.Properties["abilities"].Split(","),
            moves = _genamonData.Properties["moves"].Split(","),
            description = _genamonData.Properties["desc"],
            health = int.Parse(_genamonData.Properties["health"]),
            attack = int.Parse(_genamonData.Properties["attack"]),
            defense = int.Parse(_genamonData.Properties["defense"]),
            specialAttack = int.Parse(_genamonData.Properties["specialAttack"]),
            specialDefense = int.Parse(_genamonData.Properties["specialDefense"]),
            speed = int.Parse(_genamonData.Properties["speed"]),
            imageUrl = _genamonData.Properties["imageUrl"],
            expiresAt = long.MaxValue
        };
    }
    
    IEnumerator DownloadImage()
    {   
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(_genamonImageUri);
        yield return request.SendWebRequest();
        if(request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError) 
            Debug.Log(request.error);
        else
        {
            genamonImage.texture = ((DownloadHandlerTexture) request.downloadHandler).texture;
        }
    }
}
