using UnityEngine;

public class RecordingPlayerInput : PlayerInput
{
    private InputRecord _record = new();
    private float _startTime;

    public RecordingPlayerInput(IControllable controllable) : base(controllable)
    {
        BindControls();
        BindStateChange();
    }

    public InputRecord Record => _record;
    public InputRecord Reset()
    {
        Enable = false;
        var current = _record;
        current.Trim();
        _record = new();
        return current;
    }

    private float LocalTime => Time.time - _startTime;
    private void BindControls()
    {
        _playerActions.Game.Jump.started += (ctx) => _record.Add(InputRecord.Type.JumpStart, LocalTime);
        _playerActions.Game.Jump.canceled += (ctx) => _record.Add(InputRecord.Type.JumpEnd, LocalTime);
        _playerActions.Game.Move.started += (ctx) => _record.Add(InputRecord.Type.MovePerform, LocalTime, ctx.ReadValue<float>());
        _playerActions.Game.Move.canceled += (ctx) => _record.Add(InputRecord.Type.MoveEnd, LocalTime);
    }

    private void BindStateChange()
    {
        OnEnable += () => { _startTime = Time.time; _record.Add(InputRecord.Type.None, LocalTime); };
        OnDisable += () => { _record.Add(InputRecord.Type.None, LocalTime); };
    }
}
