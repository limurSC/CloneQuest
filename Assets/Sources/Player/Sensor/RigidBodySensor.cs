using UnityEngine;

public class RigidBodySensor : Sensor
{
    [SerializeField] private LayerMask _layerMask;
    [SerializeField] private Rigidbody2D _rigidbody;
    [SerializeField] private Vector2 _direction;
    [SerializeField] private float _distance;

    public override RaycastHit2D Hit => TimeUpdate() ? CastUpdate()[0] : _lastHit[0];

    private RaycastHit2D[] _lastHit = new RaycastHit2D[1];
    private float _lastUpdated = 0f;

    private bool TimeUpdate()
    {
        var time = Time.fixedTime;
        if (time <= _lastUpdated) { return false; }
        _lastUpdated = time;
        return true;
    }

    [ContextMenu("Update")]
    private RaycastHit2D[] CastUpdate()
    {
        var castResults = new RaycastHit2D[1];
        _rigidbody.Cast(_direction, castResults, _distance);
        _lastHit = castResults;
        return _lastHit;
    }

    private void OnValidate()
    {
        _direction.Normalize();
        if (!_rigidbody) { _rigidbody = GetComponent<Rigidbody2D>(); }
    }

    private void OnDrawGizmos()
    {
        if (UnityEditor.Selection.activeGameObject != gameObject || !enabled) return;
        Gizmos.color = Hit.collider ? Color.yellow : Color.grey;
        Gizmos.DrawRay(_rigidbody.position, _direction * _distance);
    }
}
