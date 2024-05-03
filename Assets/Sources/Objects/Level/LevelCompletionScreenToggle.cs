using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelCompletionScreenToggle : MonoBehaviour
{
    [SerializeField] private Collider2D _levelCompletionCollider;
    [SerializeField] private GameCanvasEvents _gameCanvasEvents;

    private void OnTriggerEnter2D(Collider2D other)
    {
        ToggleCompletionScreen();
    }

    void ToggleCompletionScreen()
    {
        _gameCanvasEvents.OnLevelCompletion();
        EventBus.Invoke<IPauseToggled>(act => act.OnPauseToggled());
    }

    #if UNITY_EDITOR
    private void OnValidate()
    {
        _gameCanvasEvents = FindAnyObjectByType<GameCanvasEvents>();
        _levelCompletionCollider = GetComponent<Collider2D>();
    }
    #endif
}
