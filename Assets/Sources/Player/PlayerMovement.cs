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
    [SerializeField] private Sensor _headSensor;
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

    #region FrameVelocity

    private void HandleVelocity()
    {
        _frameVelocity = _rigidbody.velocity;
    }

    #endregion

    # region Collisions

    private bool _isGrounded;
    private bool _wasGrounded;
    private float _lastGroundTime;

    private void CheckCollisions()
    {
        var hit = _groundSensor.Hit;
        _isGrounded = hit.collider;
        if (_isGrounded)
        {
            _lastGroundTime = Time.fixedTime;
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
        if (direction == 0)
        {
            var deceleration = _isGrounded ? _config.Deceleration : _config.AirDeceleration;
            _frameVelocity.x = Mathf.MoveTowards(_frameVelocity.x, 0, deceleration * Time.fixedDeltaTime);
        }
        else
        {
            var (acceleration, targetVelocity) = _isGrounded ? (_config.Acceleration, _config.Velocity) : (_config.AirAcceleration, _config.AirVelocity);
            _frameVelocity.x = Mathf.MoveTowards(_frameVelocity.x, move * targetVelocity, acceleration * Time.fixedDeltaTime);
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
        if ((_isGrounded || CanUseCoyote) && jump && jumpWasReleased) { ExecuteJump(); }
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
        if (!_isGrounded)
        {
            var gravity = _rigidbody.gravityScale * Physics2D.gravity.y;
            if (_jumpEarlyEnded && _frameVelocity.y > 0) { gravity *= _config.JumpEndEarlyGravityModifier; }
            _frameVelocity.y = Mathf.MoveTowards(_frameVelocity.y, _config.MaxFallVelocity, gravity * Time.deltaTime);
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
        if (!_groundSensor || !_headSensor)
        {
            Debug.LogWarning("One or more sensors not attached");
        }
    }

    #endregion
}
