using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PolygonExamples.Scripts
{
    public class ItemPresenter : MonoBehaviour
    {
        [SerializeField] private Image _icon;
        [SerializeField] private TextMeshProUGUI _amount;

        public void Setup(Sprite icon, string amount)
        {
            _icon.sprite = icon;
            _amount.text = amount;
        }
    }
}