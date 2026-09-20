using FMODUnity;
using UnityEngine;

// Interface just for the FMODAudioManager, just existed for service locator pattern and 
// avoid to write FMODAudioManager.Instance in the code.
// Definately not a good idea to use but this is not going to be large project so I don't want to over-engineer it.
public interface IAudioService
{
    // One-shot SFX
    void PlaySFX(EventReference eventReference);

    void PlaySFX3D(
        EventReference eventReference,
        Vector3 position);

    // Persistent audio
    AudioHandler Play(EventReference eventReference);

    AudioHandler Play3D(
        EventReference eventReference,
        Vector3 position);

    void Stop(
        AudioHandler handler,
        bool allowFadeOut = true);

    // Music
    AudioHandler PlayMusic(
        EventReference eventReference,
        bool allowFadeOut = true);

    void StopMusic(
        bool allowFadeOut = true);

    void SetMusicParameter(
        string parameterName,
        float value);

    // Snapshots
    AudioHandler PlaySnapshot(
        EventReference eventReference);

    void StopSnapshot(
        AudioHandler handler,
        bool allowFadeOut = true);
}