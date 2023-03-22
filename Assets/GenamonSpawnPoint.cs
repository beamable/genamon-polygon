using UnityEngine;

public class GenamonSpawnPoint : MonoBehaviour
{
    private SpriteRenderer _renderer;
    private GenamonPanel _panel;
    
    [SerializeField]
    private PokemonView _genamon;
    public PokemonView Genamon
    {
        get => _genamon;
        set
        {
            if (value != _genamon)
            {
                _genamon = value;
                OnGenamonUpdated();
            }
        }
    }

    public bool HasGenamon
    {
        get => _genamon != null;
    }
        
    private void OnGenamonUpdated()
    {
        // Enable or Disable Visual Effect
        if (HasGenamon)
            _renderer.enabled = true;
        else
            _renderer.enabled = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        _renderer = gameObject.GetComponent<SpriteRenderer>();
        _panel = FindObjectOfType<GenamonPanel>();
        OnGenamonUpdated();
    }

    // Update is called once per frame
    void Update()
    {
        if(HasGenamon && _genamon.IsExpired)
        {
            Genamon = null;
        }
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        _panel.Genamon = Genamon;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        _panel.Genamon = null;
    }
}
