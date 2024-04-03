using UnityEngine;

public class PlayerControls : MonoBehaviour, IControllable
{
    public float Move { get => enable ? _move : 0f; set => _move = value; }
    public bool Jump
    {
        get => enable && _jump;
        set
        {
            if (!value) { _jumpWasReleased = true; }
            _jump = value;
        }
    }
    public bool PopJumpReleasedState()
    {
        if (!_jump || !enable) { return true; }
        if (!_jumpWasReleased) { return false; }
        _jumpWasReleased = false;
        return true;
    }

    private bool enable = false;
    private float _move = 0f;
    private bool _jump = false;
    private bool _jumpWasReleased = true;

    private void OnEnable() { enable = true; }
    private void OnDisable() { enable = false; }
}
