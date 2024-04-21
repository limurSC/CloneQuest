using UnityEngine;
using UnityEngine.Events;

public class MonoSoftResetListener : MonoBehaviour, ILevelSoftResetStartHandler, ILevelSoftResetEndHandler
{
    [SerializeField] protected UnityEvent<float> StartActions = new();
    [SerializeField] protected UnityEvent EndActions = new();

    public void OnSoftResetStart(float duration) => StartActions.Invoke(duration);
    public void OnSoftResetEnd() => EndActions.Invoke();

    protected void Awake()
    {
        EventBus.Subscribe<ILevelSoftResetStartHandler>(this);
        EventBus.Subscribe<ILevelSoftResetEndHandler>(this);
    }
    protected void OnDestroy()
    {
        EventBus.Unsubscribe<ILevelSoftResetStartHandler>(this);
        EventBus.Unsubscribe<ILevelSoftResetEndHandler>(this);
    }
}