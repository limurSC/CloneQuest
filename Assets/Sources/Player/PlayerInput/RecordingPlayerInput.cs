using UnityEngine;

public class RecordingPlayerInput : PlayerInput
{
    private InputRecord _record = new();
    private bool _isRecording = false;
    private float _deltaTime = 0f;
    private float _disableTime = 0f;

    public RecordingPlayerInput(IControllable controllable) : base(controllable)
    {
        BindControls();
        BindStateChange();
    }

    public InputRecord ResetAndReturn()
    {
        Enable = false;
        _isRecording = false;
        var current = _record;
        current.Trim();
        _record = new();
        return current;
    }

    public void Reset()
    {
        Enable = false;
        _isRecording = false;
        _record.Clear();
    }

    private float LocalTime => Time.time - _deltaTime;
    private void BindControls()
    {
        _playerActions.Game.Jump.started += (ctx) => _record.Add(InputRecord.Type.JumpStart, LocalTime);
        _playerActions.Game.Jump.canceled += (ctx) => _record.Add(InputRecord.Type.JumpEnd, LocalTime);
        _playerActions.Game.Move.performed += (ctx) => _record.Add(InputRecord.Type.MovePerform, LocalTime, RoundFloat(ctx.ReadValue<float>()));
        _playerActions.Game.Move.canceled += (ctx) => _record.Add(InputRecord.Type.MoveEnd, LocalTime);
    }

    private void BindStateChange()
    {
        OnEnable += () =>
        {
            if (_isRecording) { _deltaTime += Time.time - _disableTime; }
            else { _deltaTime = Time.time; _isRecording = true; }
        };
        OnDisable += () => { _disableTime = Time.time; };
    }
}
