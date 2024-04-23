using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    [SerializeField]
    private float maxSpeed = 1.0f;

    [SerializeField]
    private float speedDecay = 0.01f;

    [SerializeField]
    private Rigidbody2D _rb;

    [SerializeField]
    private RigidBodySensor _sensor;

    private Vector2 _platformSpeed = Vector2.zero;

    private void FixedUpdate()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, -Vector2.up);
        if (hit.collider != null && _rb.bodyType != RigidbodyType2D.Static && hit.collider.attachedRigidbody != null)
        {
            if (_platformSpeed != hit.collider.attachedRigidbody.velocity)
            {
                _platformSpeed = hit.collider.attachedRigidbody.velocity;
                _rb.position += _platformSpeed * _platformSpeed * Time.fixedDeltaTime * Time.fixedDeltaTime;
            }

            //Find maximum plausible speed
            Vector2 clampVel = _rb.velocity;
            Vector2 platformVelocity = hit.collider.attachedRigidbody.velocity; 
            Vector2 selfSpeed = clampVel - platformVelocity; //Speed component owned only by our object
            var attachedRbMagnitudeX = MathF.Abs(hit.collider.attachedRigidbody.velocity.x);
            //Clamp it
            selfSpeed.x = Mathf.Clamp(selfSpeed.x, -maxSpeed, maxSpeed);
            //Decay it
            selfSpeed.x -= selfSpeed.x * speedDecay;
            //Apply it
            _rb.velocity = selfSpeed + platformVelocity;
        }
        else
        {
            _platformSpeed = Vector2.zero;
            Vector2 clampVel = _rb.velocity;
            clampVel.x = Mathf.Clamp(clampVel.x, -maxSpeed, maxSpeed);
            clampVel.x -= clampVel.x * speedDecay;
            _rb.velocity = clampVel;
        }
    }


#if UNITY_EDITOR
    private void OnValidate()
    {
        if (_rb == null)
            _rb = GetComponent<Rigidbody2D>();
    }
#endif
}
