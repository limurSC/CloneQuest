using System.Collections.Generic;
using UnityEngine;


public class CloneSystem : ILevelSoftResetEndHandler, IBeforeLevelUnloadHandler
{
    public enum ReadyState { Ready, Running, NotReady }
    public ReadyState State { get; private set; }

    private readonly RecordingPlayerInput _playerInput;
    private readonly GameObject _clonePrefab;
    private readonly Vector2 _initialPosition;
    private readonly List<(InputReplay input, GameObject gameObject)> _clones = new();

    private InputRecord _newRecord = null;

    public CloneSystem(RecordingPlayerInput playerInput, GameObject clonePrefab, Vector2 initialPosition)
    {
        _playerInput = playerInput;
        _clonePrefab = clonePrefab;
        _initialPosition = initialPosition;
        playerInput.Reset();
        Subscribe();
    }

    public void Start()
    {
        if (State != ReadyState.Ready) { return; }
        State = ReadyState.Running;
        _playerInput.Enable = true;
        _clones.ForEach(clone => clone.input.Start());
    }

    public void AddCloneAndRestart()
    {
        if (State != ReadyState.Running) { return; }
        _newRecord = _playerInput.ResetAndReturn();
        Restart();
    }

    public void Restart()
    {
        State = ReadyState.NotReady;
        _playerInput.Enable = false;
        _clones.ForEach(clone => clone.input.Reset());
        EventBus.Invoke<ILevelSoftResetStartHandler>(obj => obj.OnSoftResetStart(1f));
    }

    private void SpawnClone(InputRecord inputRecord)
    {
        var clone = Object.Instantiate(_clonePrefab, (Vector3)_initialPosition, Quaternion.identity);
        var cloneControls = clone.GetComponent<IControllable>();
        _clones.Add((new InputReplay(cloneControls, inputRecord), clone));
    }

    public void OnSoftResetEnd()
    {
        if (_newRecord != null) { SpawnClone(_newRecord); _newRecord = null; }
        State = ReadyState.Ready;
    }

    public void OnBeforeLevelUnload() => Unsubscribe();

    private void Subscribe()
    {
        EventBus.Subscribe<ILevelSoftResetEndHandler>(this);
        EventBus.Subscribe<IBeforeLevelUnloadHandler>(this);
    }

    private void Unsubscribe()
    {
        EventBus.Unsubscribe<ILevelSoftResetEndHandler>(this);
        EventBus.Unsubscribe<IBeforeLevelUnloadHandler>(this);
    }
}
