using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Xml.Serialization;
using Unity.Mathematics;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class MovablePlatform : MonoBehaviour
{
    [Header("Move Parameters")]
    [SerializeField] private bool _shouldMove = true;
    [SerializeField] private float _speed = 0f;
    [Header("Move Bounds")]
    [SerializeField] private Rigidbody2D _rigidbody;
    [SerializeField] private Vector2 _point1 = Vector2.zero;
    [SerializeField] private Vector2 _point2 = Vector2.zero;
    [SerializeField] private Vector2 _currentPoint = Vector2.zero;
    [SerializeField] private Rigidbody2D _colliderPrefab;
    private bool _speedApplied = false;

    private readonly HashSet<Rigidbody2D> _bodies = new HashSet<Rigidbody2D>();

    void FixedUpdate()
    {
        if (!_shouldMove || _speed == 0)
            return;

        if (!_speedApplied)
        {
            _rigidbody.velocity = (_currentPoint - _rigidbody.position) * _speed;
            _speedApplied = true;
        }

        if (Vector2.Distance(_rigidbody.position, _currentPoint) < 0.1)
        {
            _currentPoint = _currentPoint == _point1 ? _point2 : _point1;
            _speedApplied = false;
        }
            
        foreach (var body in _bodies)
        {
            body.velocity += _rigidbody.velocity;
        }
    }

    void SubscribeObjectOnCollisionMovement(Rigidbody2D obj)
    {
        _bodies.Add(obj);
    }

    void UnSubscribeObjectOnCollisionMovement(Rigidbody2D obj)
    {
        _bodies.Remove(obj);
    }

    void OnCollisionEnter2D(Collision2D collision) 
    {
        var body = collision.gameObject.GetComponent<Rigidbody2D>();
        SubscribeObjectOnCollisionMovement(body);
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        var body = collision.gameObject.GetComponent<Rigidbody2D>();
        UnSubscribeObjectOnCollisionMovement(body);
    }

    private void OnValidate()
    {
        if (_rigidbody == null)
        {
            _rigidbody = GetComponent<Rigidbody2D>();
        }

        _currentPoint = _point1;
    }
#if UNITY_EDITOR
        protected void OnDrawGizmos()
        {
            if (UnityEditor.Selection.activeGameObject != gameObject) return;

            const float radius = 1f;
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(GetPosition(_point1), radius);
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(GetPosition(_point2), radius);


            Vector3 GetPosition(Vector2 point)
            {
                return (Vector3)(point);
            }
        }
#endif
}
