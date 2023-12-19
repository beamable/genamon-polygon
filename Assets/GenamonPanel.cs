using System;
using System.Collections;
using System.Collections.Generic;
using Beamable;
using Beamable.Common.Api;
using Beamable.Common.Inventory;
using Beamable.Server.Clients;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class GenamonPanel : MonoBehaviour
{
    public ItemRef genamonRef;
    
    [SerializeField]
    private PokemonView _genamon;

    [SerializeField] private string _federatedId;

    public ParticleSystem fireworks;
    public GameObject container;
    public TMP_Text genamonName;
    public TMP_Text genamonTypeText;
    public TMP_Text genamonHpText;
    public TMP_Text genamonAtkText;
    public TMP_Text genamonDefText;
    public TMP_Text genamonSpAtkText;
    public TMP_Text genamonSpDefText;
    public TMP_Text genamonSpdText;
    public TMP_Text genamonDescriptionText;
    public RawImage genamonImage;

    public Button collectBtn;
    public Button cryptoBtn;

    public PokemonView Genamon
    {
        get => _genamon;
        set
        {
            if (_genamon != value)
            {
                _genamon = value;
                Refresh();
            }
        }
    }

    public void Open(PokemonView genamon, string federatedId)
    {
        _federatedId = federatedId;
        Genamon = genamon;
    }

    void Start()
    {
        Close();
    }

    public async void ViewOnChain()
    {
        await BeamContext.Default.OnReady;
        await BeamContext.Default.Accounts.OnReady;

        if (BeamContext.Default.Accounts.Current.ExternalIdentities.Length > 0)
        {
            var walletAddress = BeamContext.Default.Accounts.Current.ExternalIdentities[0].userId;
            Application.OpenURL($"https://explorer.solana.com/address/{walletAddress}?cluster=devnet");
        }
    }

    public void Close()
    {
        Genamon = null;
        Refresh();
    }

    public void CelebrateAndClose()
    {
        //fireworks.Emit(4);
        Close();
    }
    
    public async void Collect()
    {
        var playerOne = BeamContext.Default;
        if (!string.IsNullOrEmpty(Genamon.id))
        {
            try
            {
                await playerOne.Microservices().GenamonService().Collect(Genamon.id, genamonRef);
                CelebrateAndClose();
            } 
            catch (RequesterException ex) when (ex.RequestError.error == "GenamonUnavailable")
            {
                Debug.LogWarning(ex.RequestError.message);
            }
        }
    }

    private void Refresh()
    {
        if (Genamon == null || string.IsNullOrEmpty(Genamon.name))
        {
            container.SetActive(false);
            genamonImage.texture = null;
            _federatedId = null;
        }
        else
        {
            if (string.IsNullOrEmpty(Genamon.id))
            {
                collectBtn.gameObject.SetActive(false);
                if(!string.IsNullOrEmpty(_federatedId))
                    cryptoBtn.gameObject.SetActive(true);
            }
            else
            {
                cryptoBtn.gameObject.SetActive(false);
                collectBtn.gameObject.SetActive(true);
            }

            string genaType = string.Join(", ", Genamon.type);
            string hp = $"HP: {Genamon.health}";
            string atk = $"ATK: {Genamon.attack}";
            string def = $"DEF: {Genamon.defense}";
            string spAtk = $"SP.ATK: {Genamon.specialAttack}";
            string spDef = $"SP.DEF: {Genamon.specialDefense}";
            string spd = $"SPD: {Genamon.speed}";
            
            genamonName.SetText(Genamon.name);
            genamonDescriptionText.SetText(Genamon.description);
            
            genamonHpText.SetText(hp);
            genamonTypeText.SetText(genaType);

            genamonAtkText.SetText(atk);
            genamonDefText.SetText(def);
            genamonSpAtkText.SetText(spAtk);
            genamonSpDefText.SetText(spDef);
            genamonSpdText.SetText(spd);

            StartCoroutine(DownloadImage(Genamon.imageUrl));
            container.SetActive(true);
        }
    }

    IEnumerator DownloadImage(string uri)
    {   
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(uri);
        yield return request.SendWebRequest();
        if(request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError) 
            Debug.Log(request.error);
        else
        {
            genamonImage.texture = ((DownloadHandlerTexture) request.downloadHandler).texture;
        }
    }
}
