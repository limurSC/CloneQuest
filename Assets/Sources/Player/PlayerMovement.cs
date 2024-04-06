using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PlayerControls))]
[RequireComponent(typeof(Sensor))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private PlayerControls _controls;
    [SerializeField] private PlayerConfig _config;
    [SerializeField] private Transform _flipAnchor;
    [SerializeField] private Rigidbody2D _rigidbody;
    [SerializeField] private Sensor _groundSensor;

    private Vector2 _frameVelocity;

    void Awake()
    {
        SetRotation(_facingRight);
    }

    void FixedUpdate()
    {
        HandleVelocity();
        CheckCollisions();
        HandleMove();
        HandleJump();
        UpdateRotation();
        HandleGravity();
        ApplyVelocity();
    }

    #region Frame Velocity

    private void HandleVelocity()
    {
        _frameVelocity = _rigidbody.velocity;
    }

    #endregion

    # region Collisions

    private bool _isGrounded;
    private bool _wasGrounded;
    private float _lastGroundTime;
    private float _groundAngle;

    private void CheckCollisions()
    {
        var hit = _groundSensor.Hit;
        _isGrounded = hit.collider;
        if (_isGrounded)
        {
            _lastGroundTime = Time.fixedTime;
            _groundAngle = Vector2.Angle(_groundSensor.Hit.normal, Vector2.up);
            if (!_wasGrounded)
            {
                _wasGrounded = true;
                _coyoteLocked = false;
            }
        }
        else
        {
            if (_wasGrounded)
            {
                _wasGrounded = false;
            }
        }
    }

    #endregion

    #region Move

    private void HandleMove()
    {
        var move = _controls.Move;
        var direction = MathF.Sign(move);
        _facingRight = GetRotationByDirection(direction);
        if (_isGrounded)
        {
            if (_groundAngle > _config.MaxSurfaceAngle) { return; }
            var groundNormal = _groundSensor.Hit.normal;
            var alongGround = new Vector2(groundNormal.y, -groundNormal.x);
            _frameVelocity = (direction == 0)
                ? Vector2.MoveTowards(_frameVelocity, Vector2.zero, _config.Deceleration)
                : Vector2.MoveTowards(_frameVelocity, _config.Velocity * move * alongGround, _config.Acceleration * Time.fixedDeltaTime);
        }
        else
        {
            _frameVelocity.x = (direction == 0)
                ? Mathf.MoveTowards(_frameVelocity.x, 0, _config.AirDeceleration * Time.fixedDeltaTime)
                : Mathf.MoveTowards(_frameVelocity.x, move * _config.AirVelocity, _config.AirAcceleration * Time.fixedDeltaTime);
        }
    }

    #endregion Move

    #region Jump

    private bool _jumpEarlyEnded;
    private bool _coyoteLocked;
    private bool CanUseCoyote => !_coyoteLocked && _lastGroundTime + _config.CoyoteTime >= Time.fixedTime;
    private void HandleJump()
    {
        var jump = _controls.Jump;
        var jumpWasReleased = _controls.PopJumpReleasedState();
        if (!_jumpEarlyEnded && !_isGrounded && jumpWasReleased && _frameVelocity.y > 0) { _jumpEarlyEnded = true; }
        if ((_isGrounded || CanUseCoyote) && jump && jumpWasReleased && _groundAngle <= _config.MaxSurfaceAngle) { ExecuteJump(); }
    }
    private void ExecuteJump()
    {
        _jumpEarlyEnded = false;
        _frameVelocity.y = _config.JumpVelocity;
        _coyoteLocked = true;
    }

    #endregion

    #region HandleGravity

    private void HandleGravity()
    {
        var gravity = _rigidbody.gravityScale * Physics2D.gravity;
        if (!_isGrounded)
        {
            if (_jumpEarlyEnded && _frameVelocity.y > 0) { gravity *= _config.JumpEndEarlyGravityModifier; }
            _frameVelocity.y = Mathf.MoveTowards(_frameVelocity.y, _config.MaxFallVelocity, gravity.y * Time.deltaTime);
            return;
        }
        if (_groundAngle <= _config.MaxSurfaceAngle)
        {
            _rigidbody.AddForce(gravity * -_rigidbody.mass);
        }
    }

    #endregion

    #region Rotation

    [SerializeField] private bool _facingRight;
    private bool _previousFacingRight;
    private bool GetRotationByDirection(float direction) => direction == 0 ? _facingRight : direction > 0;

    private void SetRotation(bool facingRight)
    {
        _facingRight = facingRight;
        var localScale = _flipAnchor.localScale;
        localScale.x = facingRight ? 1f : -1f;
        _flipAnchor.localScale = localScale;
    }

    private bool UpdateRotation()
    {
        if (_previousFacingRight == _facingRight)
            return false;
        _previousFacingRight = _facingRight;
        SetRotation(_facingRight);
        return true;
    }

    #endregion Rotation

    private void ApplyVelocity() => _rigidbody.velocity = _frameVelocity;

    #region Editor

    private void OnValidate()
    {
        if (!_rigidbody) { _rigidbody = GetComponent<Rigidbody2D>(); }
        if (!_controls) { _controls = GetComponent<PlayerControls>(); }
        if (!_groundSensor) { Debug.LogWarning("Sensor not attached"); }
        if (!_config) { Debug.Log("Config not attached"); }
    }

    #endregion
}
