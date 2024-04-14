using UnityEngine;
using UnityEngine.Events;

public class PlatformMoving : WaypointMoving, IActivatable
{
    public bool Active { get => _active; set => SetActive(value); }
    public UnityEvent<bool> OnActivateSet;
    public UnityEvent<bool> OnEdgeReached;
    private bool _active;
    private bool _moving;

    public void SetActive(bool active)
    {
        if (active == _active) return;
        if (_moving) { Stop(); }
        _active = active;
        _moving = true;
        if (_active) { Activate(); } else { Deactivate(); }
        OnActivateSet.Invoke(active);
    }

    private void Activate()
    {
        MoveForward();
        WaypointReached.AddListener(MoveForward);
        TargetReached.AddListener(EdgeReached);
    }
    private void Deactivate()
    {
        MoveBackward();
        WaypointReached.AddListener(MoveBackward);
        TargetReached.AddListener(EdgeReached);
    }

    private void Stop()
    {
        _moving = false;
        RemoveListeners();
    }
    private void EdgeReached()
    {
        Stop();
        OnEdgeReached.Invoke(_active);
    }
    private void RemoveListeners()
    {
        if (_active) { WaypointReached.RemoveListener(MoveForward); }
        else { WaypointReached.RemoveListener(MoveBackward); }
        TargetReached.RemoveListener(EdgeReached);
    }

# if UNITY_EDITOR
    [ContextMenu("Activate")] private void __Activate() => SetActive(true);
    [ContextMenu("Deactivate")] private void __Deactivate() => SetActive(false);
#endif
}