using UnityEngine;

public class PlayerControls : MonoBehaviour, IControllable
{
    public float Move { get; set; }
    public bool Jump
    {
        get => _jump;
        set
        {
            if (!value) { _jumpWasReleased = true; }
            _jump = value;
        }
    }
    public bool PopJumpReleasedState()
    {
        if (!_jump) { return true; }
        if (!_jumpWasReleased) { return false; }
        _jumpWasReleased = false;
        return true;
    }

    private bool _jump = false;
    private bool _jumpWasReleased = true;
}
