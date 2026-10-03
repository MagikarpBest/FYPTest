using FMODUnity;
using UnityEngine;

public static class AudioService
{
    private static readonly IAudioService FALLBACK_SERVER = new NullAudioManager();
    private static IAudioService s_currentService = FALLBACK_SERVER;
    public static void SetAudioService(IAudioService audioService)
    {
        s_currentService = audioService ?? FALLBACK_SERVER;
    }
    // SFX

    public static void PlaySFX(EventReference eventReference)
    {
        s_currentService.PlaySFX(eventReference);
    }

    public static void PlaySFX3D(EventReference eventReference, Vector3 position)
    {
        s_currentService.PlaySFX3D(eventReference, position);
    }

    // Persistent Audio

    public static AudioHandler Play(EventReference eventReference)
    {
        return s_currentService.Play(eventReference);
    }

    public static AudioHandler Play3D(EventReference eventReference, Vector3 position)
    {
        return s_currentService.Play3D(eventReference, position);
    }

    public static void Stop(AudioHandler handler, bool allowFadeOut = true)
    {
        s_currentService.Stop(handler, allowFadeOut);
    }

    // Music

    public static AudioHandler PlayMusic(EventReference eventReference, bool allowFadeOut = true)
    {
        return s_currentService.PlayMusic(eventReference, allowFadeOut);
    }

    public static void StopMusic(bool allowFadeOut = true)
    {
        s_currentService.StopMusic(allowFadeOut);
    }

    public static void SetMusicParameter(string parameterName, float value)
    {
        s_currentService.SetMusicParameter(parameterName, value);
    }

    // Snapshots

    public static AudioHandler PlaySnapshot(EventReference eventReference)
    {
        return s_currentService.PlaySnapshot(eventReference);
    }

    public static void StopSnapshot(AudioHandler handler, bool allowFadeOut = true)
    {
        s_currentService.StopSnapshot(handler, allowFadeOut);
    }

    public static void SetMasterVolume(float volume)
    {
        s_currentService.SetMasterVolume(volume);
    }

    public static float GetMasterVolume()
    {
        return s_currentService.GetMasterVolume();
    }

    public static void SetMusicVolume(float volume)
    {
        s_currentService.SetMusicVolume(volume);
    }

    public static float GetMusicVolume()
    {
        return s_currentService.GetMusicVolume();
    }

    public static void SetSFXVolume(float volume)
    {
        s_currentService.SetSFXVolume(volume);
    }

    public static float GetSFXVolume()
    {
        return s_currentService.GetSFXVolume();
    }
}