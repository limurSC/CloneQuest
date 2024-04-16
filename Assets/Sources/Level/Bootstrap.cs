using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bootstrap : MonoBehaviour
{

    [SerializeField] private GameObject _playerPrefab;
    [SerializeField] private GameObject _clonePrefab;

    private GameObject _player;
    private RecordingPlayerInput _playerInput;
    private IControllable _playerControls;
    private Vector3 _spawnPoint;
    private List<(InputReplay input, Transform transform)> _clones = new();
    private bool _pause;

    private void Start()
    {
        _player = FindObjectOfType<PlayerControls>().gameObject;
        _spawnPoint = _player.transform.position;
        _playerControls = _player.GetComponent<IControllable>();

        _playerInput = new(_playerControls);

        var input = new PlayerActions();
        input.Game.Clone.started += (ctx) => Coroutines.Run(SpawnClone());
        input.Game.Restart.started += (ctx) => { Restart(); EventBus.Invoke<ISoftReset>((obj) => obj.SoftReset(0.5f)); };
        input.Game.Esc.started += (ctx) => TogglePause();
        input.Game.Enable();

        _playerInput.Enable = true;

    }

    IEnumerator SpawnClone()
    {
        var inputRecord = _playerInput.Reset();
        var clone = Instantiate(_clonePrefab);
        var cloneControls = clone.GetComponent<IControllable>();
        var cloneInput = new InputReplay(cloneControls, inputRecord);
        _clones.Add((cloneInput, clone.transform));
        _player.transform.position = _spawnPoint;
        _clones.ForEach((clone) => { clone.input.Stop(); clone.transform.position = _spawnPoint; });
        yield return new WaitForSecondsRealtime(0.5f);
        _clones.ForEach((clone) => clone.input.Start());
        _playerInput.Enable = true;
    }

    void Restart()
    {
        _clones.ForEach((clone) => { Destroy(clone.transform.gameObject); });
        _clones.Clear();
        _player.transform.position = _spawnPoint;
        _playerInput.Reset();
        _playerInput.Enable = true;
    }

    void TogglePause()
    {
        Time.timeScale = _pause ? 1f : 0f;
        _pause = !_pause;
    }
}