using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Events;

public class HealtBarComponent : MonoBehaviour
{
    [SerializeField] private Slider _sliderBar;
    [SerializeField] private TextMeshProUGUI _label;

    public void Init()
    {
        EventsController.Subscribe<EventModels.Game.HPStatus>(this, UpdateBar);
    }

    private void UpdateBar(EventModels.Game.HPStatus e)
    {
        _sliderBar.maxValue = e.HPMax;
        _sliderBar.value = e.HPValue;
        _label.text = $"{e.HPValue}/{e.HPMax}";
    }

    private void OnDestroy()
    {
        EventsController.Unsubscribe<EventModels.Game.HPStatus>(UpdateBar);
    }
}