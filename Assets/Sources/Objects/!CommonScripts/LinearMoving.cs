using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class LinearMoving : MonoBehaviour
{
    public Vector2 Pivot => _pivot;
    public bool Moving => _coroutine != null;
    public event Action TargetReached;
    [SerializeField] private Rigidbody2D _rigidbody;
    private Vector2 _pivot;
    private Coroutine _coroutine;

    public void MoveTo(Vector2 targetRelative, float velocity)
    {
        if (_coroutine != null) { Stop(); }
        _coroutine = StartCoroutine(MoveRoutine(targetRelative, velocity));
    }

    public void Stop()
    {
        if (_coroutine == null) { return; }
        StopCoroutine(_coroutine);
        StopMoving();
    }

    private IEnumerator MoveRoutine(Vector2 targetRelative, float velocity)
    {
        var target = _pivot + targetRelative;
        var current = _rigidbody.position;
        var velocityVector = (target - current).normalized * velocity;
        var minDistance = velocity * velocity * Time.fixedDeltaTime * Time.fixedDeltaTime;
        _rigidbody.velocity = velocityVector;
        yield return new WaitWhile(() => (target - _rigidbody.position).sqrMagnitude > minDistance);
        _rigidbody.position = target;
        StopMoving();
        TargetReached?.Invoke();
    }

    private void StopMoving()
    {
        _rigidbody.velocity = Vector2.zero;
        _coroutine = null;
    }

    private void Awake()
    {
        _pivot = _rigidbody.position;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (_rigidbody == null) { _rigidbody = GetComponent<Rigidbody2D>(); }
    }
#endif
}
