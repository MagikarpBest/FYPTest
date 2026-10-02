using FMOD.Studio;
using FMODUnity;
using UnityEngine;

/// <summary>
/// FMODAudioController is used to modify the volume of different audio buses in the FMOD audio system.
/// This class directly attach to FMODAudioManager.
/// </summary>
public sealed class FMODAudioController
{
    private Bus _masterBus;
    private Bus _musicBus;
    private Bus _uiSFXBus;
    private Bus _ambientBus;
    private Bus _world3DSFXBus;

    private const string MasterBusPath = "bus:/";
    private const string MusicBusPath = "bus:/Music";
    private const string UI_SFXBusPath = "bus:/UI_SFX";
    private const string World_3D_SFXBusPath = "bus:/World_3D_SFX";
    private const string AmbientBusPath = "bus:/Ambient";

    public FMODAudioController()
    {
        InitializeBuses();
    }

    private void InitializeBuses()
    {
        _masterBus = RuntimeManager.GetBus(MasterBusPath);
        _musicBus = RuntimeManager.GetBus(MusicBusPath);
        _uiSFXBus = RuntimeManager.GetBus(UI_SFXBusPath);
        _ambientBus = RuntimeManager.GetBus(AmbientBusPath);
        _world3DSFXBus = RuntimeManager.GetBus(World_3D_SFXBusPath);

        SetMasterVolume(1.0f);
        SetMusicVolume(1.0f);
        SetSFXVolume(1.0f);
    }

    #region Master

    public void SetMasterVolume(float volume)
    {
        SetBusVolume(_masterBus, volume);
    }

    public float GetMasterVolume()
    {
        return GetBusVolume(_masterBus);
    }

    #endregion

    #region Music

    public void SetMusicVolume(float volume)
    {
        SetBusVolume(_musicBus, volume);
    }

    public float GetMusicVolume()
    {
        return GetBusVolume(_musicBus);
    }

    #endregion

    #region SFX

    public void SetSFXVolume(float volume)
    {
        SetBusVolume(_uiSFXBus, volume);
        SetBusVolume(_world3DSFXBus, volume);
        SetBusVolume(_ambientBus, volume);
    }

    public float GetSFXVolume()
    {
        return GetBusVolume(_uiSFXBus);
    }

    #endregion

    #region Bus

    private void SetBusVolume(
        Bus bus,
        float volume)
    {
        volume = Mathf.Clamp01(volume);

        bus.setVolume(volume);
    }

    private float GetBusVolume(Bus bus)
    {
        bus.getVolume(
            out float volume,
            out _);

        return volume;
    }

    #endregion
}