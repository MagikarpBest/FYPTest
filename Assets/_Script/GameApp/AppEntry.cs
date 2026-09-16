using UnityEngine;

public static class AppEntry
{
    static AppEntry() { }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    // want this function to be called when Unity engine starts!
    private static void Init()
    {
        FMODAudioManager audioManagerObject = new GameObject("FMODAudioManager").AddComponent<FMODAudioManager>();
        AudioService.SetAudioService(audioManagerObject);

        // audioManagerInstance.SetVolume(volume);
        // audioManagerInstance.SetPitch(pitch);
    }
}
