using System;
using System.Collections.Generic;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public static class LevelManager
{
    static public List<string> levelsByName;

    public static void Load(LevelContext levelContext)
    {
        SceneManager.sceneLoaded += OnLevelLoaded;
        SceneManager.LoadScene(levelContext.Id);

        void OnLevelLoaded(Scene _, LoadSceneMode __)
        {
            SceneManager.sceneLoaded -= OnLevelLoaded;
            EventBus.Invoke<ILevelLoadHandler>((obj) => obj.OnLevelLoad(levelContext));
        }
    }

    public static void LoadNext(LevelContext levelContext)
    {
        if (levelContext == null)
        {
            SceneLoader.LoadScene("MenuScene");
        }
        var nextContext = new LevelContext(levelContext.Index + 1, levelContext.ListIds, levelContext.FromId);
        Load(nextContext);
    }

    public static void LoadNextLevel()
    {
        var occurence = levelsByName.FindIndex(x => x == SceneManager.GetActiveScene().name);
        if (occurence == levelsByName.Count - 1)
        {
            SceneLoader.LoadScene("MenuScene");
        }
        else
        {
            var context = new LevelContext(occurence + 1, Array.AsReadOnly(levelsByName.ToArray()), SceneManager.GetActiveScene().name);
            LevelManager.Load(context);
        }
    }
}
