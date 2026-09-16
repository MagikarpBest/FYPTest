using FMODUnity;
using UnityEngine;
using UnityEngine.InputSystem;

public class AudioCall : MonoBehaviour
{
    private void Start()
    {
        // Example usage of AudioService to play SFX and Music
        PlaySFX();
        PlayMusic();
    }

    private void Update()
    {
        // Example usage of AudioService to stop music on key press
        if (Keyboard.current.sKey.wasPressedThisFrame)
        {
            PlaySFX();
        }
    }

    private void PlaySFX()
    {
        AudioService.PlaySFX3D(AudioEvent.SFX.Rock_Clash, transform.position);
    }

    private void PlayMusic()
    {
        AudioService.PlayMusic(AudioEvent.Music.TitleScreen);
    }

    private void StopMusic()
    {
        AudioService.StopMusic();
    }
}