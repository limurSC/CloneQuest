using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.InputSystem.InputAction;

public class Bootstrap : MonoBehaviour, ILevelLoadHandler, ILevelSoftResetStartHandler, ILevelReadyHandler, ILevelStartHandler, IPauseToggleHandler, ILevelReloadHandler,
    IBeforeLevelUnloadHandler
{
    [SerializeField] private GameObject _clonePrefab;

    private LevelContext _levelContext;
    private CloneSystem _cloneSystem;
    private PlayerActions _input;
    private bool _pause;

    public void OnLevelLoad(LevelContext levelContext)
    {
        EventBus.Unsubscribe<ILevelLoadHandler>(this);
        _levelContext = levelContext;
    }

    private void PrepareLevel()
    {
        var playerControls = FindObjectOfType<PlayerControls>();
        _cloneSystem = new CloneSystem(new(playerControls), _clonePrefab, playerControls.transform.position);
        _input = new PlayerActions();
        _input.Game.Clone.started += (ctx) => { _cloneSystem.AddCloneAndRestart(); };
        _input.Game.Restart.started += (ctx) => { OnLevelRestart(); };
        _input.Game.Esc.started += (ctx) => { TogglePause(); };
        _input.Game.Enable();
        EventBus.Invoke<ILevelReadyHandler>(obj => obj.OnLevelReady());
    }

    public void OnLevelReady()
    {
        _input.Game.Move.actionMap.actionTriggered += OnAnyButtonPressed;
        if (_pause) { TogglePause(); }
    }

    private void OnAnyButtonPressed(CallbackContext ctx) => EventBus.Invoke<ILevelStartHandler>(obj => obj.OnLevelStart());

    public void OnLevelStart()
    {
        _input.Game.Move.actionMap.actionTriggered -= OnAnyButtonPressed;
        _cloneSystem.Start();
        if (_pause) { TogglePause(); }
    }

    public void OnLevelRestart()
    {
        LevelManager.Load(_levelContext);
        if (_pause) { TogglePause(); }
    }

    public void OnPauseToggled() => TogglePause();

    private void TogglePause()
    {
        Time.timeScale = _pause ? 1f : 0f;
        _pause = !_pause;
        // TODO Disable inputs
    }

    public void OnSoftResetStart(float duration)
    {
        StartCoroutine(WaitForSoftResetEnd());
        IEnumerator WaitForSoftResetEnd()
        {
            yield return new WaitForSeconds(duration);
            EventBus.Invoke<ILevelSoftResetEndHandler>(obj => obj.OnSoftResetEnd());
            EventBus.Invoke<ILevelReadyHandler>(obj => obj.OnLevelReady());
        }
    }

    private void Subscribe()
    {
        EventBus.Subscribe<ILevelReadyHandler>(this);
        EventBus.Subscribe<ILevelStartHandler>(this);
        EventBus.Subscribe<ILevelSoftResetStartHandler>(this);
        EventBus.Subscribe<IPauseToggleHandler>(this);
        EventBus.Subscribe<ILevelReloadHandler>(this);
        EventBus.Subscribe<IBeforeLevelUnloadHandler>(this);
    }

    private void Unsubscribe()
    {
        _input.Dispose();
        EventBus.Unsubscribe<ILevelReadyHandler>(this);
        EventBus.Unsubscribe<ILevelStartHandler>(this);
        EventBus.Unsubscribe<ILevelSoftResetStartHandler>(this);
        EventBus.Unsubscribe<IPauseToggleHandler>(this);
        EventBus.Unsubscribe<ILevelReloadHandler>(this);
        EventBus.Unsubscribe<IBeforeLevelUnloadHandler>(this);
    }

    private void Awake() => EventBus.Subscribe<ILevelLoadHandler>(this);


    private void Start()
    {
#if UNITY_EDITOR
        if (_levelContext == null)
        {
            _levelContext = new(0, Array.AsReadOnly(new string[] { SceneManager.GetActiveScene().name }), "");
            Debug.LogWarning($"LevelContext is null set {_levelContext.Id}");
        }
#endif
        Subscribe();
        PrepareLevel();
    }
    private void OnDestroy()
    {
        EventBus.Invoke<IBeforeLevelUnloadHandler>(obj => obj.OnBeforeLevelUnload());
        Unsubscribe();
    }

    public void OnBeforeLevelUnload()
    {
        if (_pause) { TogglePause(); }
    }
}
