using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(WaypointMoving))]
[RequireComponent(typeof(WaypointLineRenderer))]
public class PlatformMoving : MonoBehaviour, IActivatable, ILevelSoftResetStartHandler
{
    public bool Active { get => _active; set => ActiveSet(value); }
    public UnityEvent<bool> OnActivateSet;
    [SerializeField][Range(0.1f, 10f)] float _velocity = 1f;
    [SerializeField] private WaypointMoving _waypointMoving;
    [SerializeField] private WaypointLineRenderer _waypointLineRenderer;
    private bool _active;

    private void ActiveSet(bool active)
    {
        if (active == _active) { return; }
        _active = active;
        if (active) { _waypointMoving.MoveForward(_velocity); }
        else { _waypointMoving.MoveBackward(_velocity); }
        OnActivateSet?.Invoke(active);
    }

    public void OnSoftResetStart(float duration)
    {
        _active = false;
        _waypointMoving.MoveBackwardByTime(duration);
        OnActivateSet?.Invoke(_active);
    }

    private void Start()
    {
        _waypointLineRenderer.SetPoints(_waypointMoving.Pivot, _waypointMoving.Points);
        EventBus.Subscribe<ILevelSoftResetStartHandler>(this);
    }
    private void OnDestroy() { EventBus.Unsubscribe<ILevelSoftResetStartHandler>(this); }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (_waypointMoving == null) { _waypointMoving = GetComponent<WaypointMoving>(); }
        if (_waypointLineRenderer == null) { _waypointLineRenderer = GetComponent<WaypointLineRenderer>(); }
    }
#endif
}