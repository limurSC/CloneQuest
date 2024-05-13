using UnityEngine.SceneManagement;

public static class LevelManager
{
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
        var nextContext = new LevelContext(levelContext.Index + 1, levelContext.ListIds, levelContext.FromId);
        Load(nextContext);
    }
}
