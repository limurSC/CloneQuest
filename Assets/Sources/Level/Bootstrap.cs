using System;
using UnityEngine;

public class Bootstrap : MonoBehaviour
{
    [SerializeField] private IControllable _playerControls;

    private PlayerInput _playerInput;

    private void Start()
    {
        _playerInput = new();
        SubscribeControls();
    }

    private void SubscribeControls()
    {
        _playerInput.Game.Move.started += (ctx) => _playerControls.Move = MathF.Round(ctx.ReadValue<float>(), 2);
        _playerInput.Game.Move.canceled += (ctx) => _playerControls.Move = 0f;
        _playerInput.Game.Jump.started += (ctx) => _playerControls.Jump = true;
        _playerInput.Game.Jump.canceled += (ctx) => _playerControls.Jump = false;
        _playerInput.Enable();
    }

    private void OnValidate()
    {
        _playerControls ??= FindObjectOfType<PlayerControls>();
    }
}
