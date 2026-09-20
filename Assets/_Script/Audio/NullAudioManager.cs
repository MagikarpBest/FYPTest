using FMODUnity;
using UnityEngine;

// Interface just for the FMODAudioManager, just existed for service locator pattern and 
// avoid to write FMODAudioManager.Instance in the code.
// Definately not a good idea to use but this is not going to be large project so I don't want to over-engineer it.
public class NullAudioManager : IAudioService
{
    // One-shot SFX
    public void PlaySFX(EventReference eventReference)
    {
        // Null implementation
        UnityEngine.Debug.LogWarning("[NullAudioManager] PlaySFX called with eventReference: " + eventReference);
    }

    public void PlaySFX3D(
        EventReference eventReference,
        Vector3 position)
    {
        // Null implementation
        UnityEngine.Debug.LogWarning("[NullAudioManager] PlaySFX3D called with eventReference: " + eventReference + " and position: " + position);
    }

    // Persistent audio
    public AudioHandler Play(EventReference eventReference)
    {
        UnityEngine.Debug.LogWarning("[NullAudioManager] Play called with eventReference: " + eventReference);
        return null;
    }

    public AudioHandler Play3D(
        EventReference eventReference,
        Vector3 position)
    {
        return null;
    }

    public void Stop(
        AudioHandler handler,
        bool allowFadeOut = true)
    {
        // Null implementation
    }

    // Music
    public AudioHandler PlayMusic(
        EventReference eventReference,
        bool allowFadeOut = true)
    {
        return null;
    }

    public void StopMusic(
        bool allowFadeOut = true)
    {
        // Null implementation
    }

    public void SetMusicParameter(
        string parameterName,
        float value)
        {
            // Null implementation
        }

    // Snapshots
    public AudioHandler PlaySnapshot(
        EventReference eventReference)
    {
        return null;
    }

    public void StopSnapshot(
        AudioHandler handler,
        bool allowFadeOut = true)
    {
        // Null implementation
    }
}