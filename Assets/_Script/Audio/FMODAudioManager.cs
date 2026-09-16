using System;
using System.Collections.Generic;
using FMOD;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

// TODO: 
//      Implement volume control methods for master, music, and SFX buses
//      Snapshot management/mixing
// 
// Functionality checklist in the future, due to code mostly AI generated:
// - 3D Audio
// - Adaptive music with modifying parameters
// - Play and stop sfx
public class FMODAudioManager : MonoBehaviour, IAudioService
{
    private readonly HashSet<AudioHandler> _activeHandlers = new();
    private AudioHandler _musicHandler; 
    // Used to track the currently playing music and allow modifying its parameters. For adaptive music purpose.
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        UpdateHandlers();
    }

    private void OnDestroy()
    {
        foreach (AudioHandler handler in _activeHandlers)
        {
            handler.Release();
        }

        _activeHandlers.Clear();
    }

    #region One Shot

    /// <summary>
    /// Plays a non-spatial 2D one-shot sound.
    /// </summary>
    public void PlaySFX(EventReference eventReference)
    {
        if (!eventReference.IsNull)
        {
            RuntimeManager.PlayOneShot(eventReference);
        }
    }

    /// <summary>
    /// Plays a spatial 3D one-shot sound at the specified world position.
    /// </summary>
    public void PlaySFX3D(
        EventReference eventReference,
        Vector3 position)
    {
        if (!eventReference.IsNull)
        {
            RuntimeManager.PlayOneShot(
                eventReference,
                position);
        }
    }

    #endregion

    #region Persistent Audio

    /// <summary>
    /// Creates and starts an EventInstance.
    /// </summary>
    public AudioHandler Play(
        EventReference eventReference)
    {
        if (eventReference.IsNull)
        {
            UnityEngine.Debug.LogWarning(
                "FMODAudioManager: Cannot play a null EventReference.");

            return null;
        }

        EventInstance instance =
            RuntimeManager.CreateInstance(eventReference);

        AudioHandler handler = new AudioHandler(instance);

        _activeHandlers.Add(handler);

        instance.start();

        return handler;
    }

    /// <summary>
    /// Creates and starts a 3D EventInstance.
    /// </summary>
    public AudioHandler Play3D(
        EventReference eventReference,
        Vector3 position)
    {
        if (eventReference.IsNull)
        {
            UnityEngine.Debug.LogWarning(
                "FMODAudioManager: Cannot play a null EventReference.");

            return null;
        }

        EventInstance instance =
            RuntimeManager.CreateInstance(eventReference);

        AudioHandler handler = new AudioHandler(instance);

        _activeHandlers.Add(handler);

        handler.SetPosition(position);
        instance.start();

        return handler;
    }

    /// <summary>
    /// Stops and removes an active AudioHandler.
    /// </summary>
    public void Stop(
        AudioHandler handler,
        bool allowFadeOut = true)
    {
        if (handler == null)
        {
            return;
        }

        handler.Stop(allowFadeOut);
        RemoveHandler(handler);
    }

    private void RemoveHandler(AudioHandler handler)
    {
        if (handler == null)
        {
            return;
        }

        _activeHandlers.Remove(handler);
    }

    private void UpdateHandlers()
    {
        if (_activeHandlers.Count == 0)
        {
            return;
        }

        AudioHandler[] handlers = new AudioHandler[_activeHandlers.Count];
        _activeHandlers.CopyTo(handlers);

        foreach (AudioHandler handler in handlers)
        {
            if (handler == null)
            {
                _activeHandlers.Remove(handler);
                continue;
            }

            handler.Update();

            if (handler.IsFinished)
            {
                _activeHandlers.Remove(handler);
            }
        }
    }

    #endregion

    #region Music

    /// <summary>
    /// Starts a new music event and stops the previous music event.
    /// </summary>
    public AudioHandler PlayMusic(
        EventReference eventReference,
        bool allowFadeOut = true)
    {
        StopMusic(allowFadeOut);

        _musicHandler = Play(eventReference);

        return _musicHandler;
    }

    /// <summary>
    /// Stops the currently playing music.
    /// </summary>
    public void StopMusic(
        bool allowFadeOut = true)
    {
        if (_musicHandler == null)
        {
            return;
        }

        Stop(_musicHandler, allowFadeOut);
        _musicHandler = null;
    }

    /// <summary>
    /// Returns the currently playing music handler.
    /// </summary>
    public AudioHandler GetMusicHandler()
    {
        return _musicHandler;
    }

    /// <summary>
    /// Sets a parameter on the currently playing music event.
    /// </summary>
    public void SetMusicParameter(
        string parameterName,
        float value)
    {
        if (_musicHandler == null)
        {
            return;
        }

        _musicHandler.SetParameter(
            parameterName,
            value);
    }

    #endregion

    #region Snapshot

    /// <summary>
    /// Starts an FMOD snapshot.
    /// </summary>
    public AudioHandler PlaySnapshot(
        EventReference snapshotReference)
    {
        return Play(snapshotReference);
    }

    /// <summary>
    /// Stops an FMOD snapshot.
    /// </summary>
    public void StopSnapshot(
        AudioHandler snapshot,
        bool allowFadeOut = true)
    {
        Stop(snapshot, allowFadeOut);
    }

    #endregion
}