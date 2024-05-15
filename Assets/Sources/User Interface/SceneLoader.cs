using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public static void LoadScene(string sceneName)
    {
        EventBus.Invoke<IBeforeLevelUnloadHandler>((obj) => obj.OnBeforeLevelUnload());
        SceneManager.LoadScene(sceneName);
    }
    
    public static void TryLoadingNextLevel()
    {
        LevelManager.LoadNextLevel();
    }
}
