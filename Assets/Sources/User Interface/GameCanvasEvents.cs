using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class GameCanvasEvents : MonoBehaviour
{
    [SerializeField]
    private GameObject _pauseScreen;
    [SerializeField]
    private GameObject _levelCompletionScreen;

    public void OnLevelCompletion()
    {
        if(!_levelCompletionScreen.gameObject.activeInHierarchy)
            _levelCompletionScreen.SetActive(true);
    }

    public void OnPausePress()
    {
        _pauseScreen.SetActive(true);
    }
}
