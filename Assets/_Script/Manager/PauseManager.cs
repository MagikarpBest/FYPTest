using UnityEngine;

public static class PauseManager
{
    public static bool IsPaused { get; private set; }

    public static void SetGamePause(bool pause)
    {
        IsPaused = pause;
        Time.timeScale = pause ? 0f : 1f;
    }
}