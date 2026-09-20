using FMOD.Studio;
using FMODUnity;
using UnityEngine;

// TODO: Actual implement in FMODAudioManager so AudioService can be reach to modify volume of master, music, and sfx bus
public class FMODAudioController : MonoBehaviour
{
    private static FMODAudioController _instance;

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

    public static FMODAudioController Instance
    {
        get
        {
            if (_instance == null)
            {
                Initialize();
            }

            return _instance;
        }
    }

    private static void Initialize()
    {
        _instance = FindFirstObjectByType<FMODAudioController>();

        if (_instance != null)
        {
            return;
        }

        GameObject controllerObject =
            new GameObject(nameof(FMODAudioController));

        _instance =
            controllerObject.AddComponent<FMODAudioController>();

        DontDestroyOnLoad(controllerObject);
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;

        DontDestroyOnLoad(gameObject);

        InitializeBuses();
    }

    private void InitializeBuses()
    {
        _masterBus = RuntimeManager.GetBus(MasterBusPath);
        _musicBus = RuntimeManager.GetBus(MusicBusPath);
        _uiSFXBus = RuntimeManager.GetBus(UI_SFXBusPath);
        _ambientBus = RuntimeManager.GetBus(AmbientBusPath);
        _world3DSFXBus = RuntimeManager.GetBus(World_3D_SFXBusPath);
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