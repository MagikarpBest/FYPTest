using System.Collections;
using UnityEngine;

public class MainMenuInitializer : SceneInitializer
{
    [SerializeField] private ScreenBase _mainMenuScreen;
    [SerializeField] private Transform _cameraTransform;
    public override void Initialize(System.Action onComplete = null)
    {
        if (_mainMenuScreen != null)
        {
            // Canvas inheritenced from ScreenBase will be disabled by default, 
            // need to push it to the screen stack to make it visible.
            GameScreenManager.Push(_mainMenuScreen, instant: true);
        }
        else
        {
            Debug.LogWarning("Main menu screen is not assigned.");
        }

        Camera mainCamera = Camera.main;
        if (mainCamera != null && _cameraTransform != null)
        {
            mainCamera.transform.position = _cameraTransform.position;
            mainCamera.transform.rotation = _cameraTransform.rotation;
        }

        AudioService.PlayMusic(AudioEvent.Music.TitleScreen);

        onComplete?.Invoke();
    }

    private void OnDestroy()
    {
        AudioService.StopMusic();
    }
}
