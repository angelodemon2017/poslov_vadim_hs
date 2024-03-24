using FiniteStateMachine;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

[RequireComponent(typeof(Button))]
public class ButtonSelectCommand : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _labelButton;
    [SerializeField] private Button _selfButton;

    public Action<State> _changeFState;

    private State _state;

    private void OnValidate()
    {
        if (_selfButton == null)
        {
            _selfButton = GetComponent<Button>();
        }
    }

    private void Awake()
    {
        _selfButton.onClick.AddListener(OnClick);
    }

    public void Init(State state, Action<State> actState)
    {
        _state = state;
        _labelButton.text = _state.GetNameState;
        _changeFState = actState;
    }

    private void OnClick()
    {
        _changeFState?.Invoke(_state);
    }

    private void OnDestroy()
    {
        _changeFState = null;
    }
}