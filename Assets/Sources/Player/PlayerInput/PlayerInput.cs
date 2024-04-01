public class PlayerInput
{
    public bool Enable { get => _enable; set => SetState(value); }

    private readonly PlayerActions _playerActions;
    private readonly IControllable _controllable;
    private bool _enable = false;

    public PlayerInput(IControllable controllable)
    {
        _playerActions = new();
        _controllable = controllable;
        BindControls();
    }

    private void BindControls()
    {
        _playerActions.Game.Move.started += (ctx) => _controllable.Move = ctx.ReadValue<float>();
        _playerActions.Game.Move.canceled += (ctx) => _controllable.Move = 0f;
        _playerActions.Game.Jump.started += (ctx) => _controllable.Jump = true;
        _playerActions.Game.Jump.canceled += (ctx) => _controllable.Jump = false;
    }

    private void SetState(bool enable)
    {
        if (enable == _enable) { return; }
        _enable = enable;
        if (enable) { SetEnable(); } else { SetDisable(); }
    }

    private void SetEnable()
    {
        _playerActions.Game.Enable();
    }

    private void SetDisable()
    {
        _playerActions.Game.Disable();
    }
}
