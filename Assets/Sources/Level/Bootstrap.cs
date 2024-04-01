using UnityEngine;

public class Bootstrap : MonoBehaviour
{
    [SerializeField] private IControllable _playerControls;

    private void Start()
    {
        var playerInput = new PlayerInput(_playerControls);
        playerInput.Enable = true;
    }


    private void OnValidate()
    {
        _playerControls ??= FindObjectOfType<PlayerControls>();
    }
}
