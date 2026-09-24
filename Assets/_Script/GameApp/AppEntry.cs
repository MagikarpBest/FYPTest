using UnityEngine;
using UnityEngine.SceneManagement;

public static class AppEntry
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static async void Init()
    {
        if (IsSceneLoaded("AppBootstrap"))
        {
            Debug.Log("AppEntry: AppBootstrap is already loaded.");
            return;
        }

        //await SceneManager.LoadSceneAsync("AppBootstrap", LoadSceneMode.Additive);
    }

    private static bool IsSceneLoaded(string sceneName)
    {
        Scene scene = SceneManager.GetSceneByName(sceneName);
        return scene.IsValid() && scene.isLoaded;
    }
}