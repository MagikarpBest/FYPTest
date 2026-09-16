using System;
using FMOD;
using FMOD.Studio;
using UnityEngine;

public class AudioHandler
{
    private EventInstance _eventInstance;
    private bool _isReleased;

    public bool IsValid =>
        !_isReleased && _eventInstance.isValid();

    public bool IsFinished
    {
        get
        {
            if (!IsValid)
            {
                return true;
            }

            PLAYBACK_STATE playbackState;

            RESULT result =
                _eventInstance.getPlaybackState(
                    out playbackState);

            if (result != RESULT.OK)
            {
                return true;
            }

            return playbackState == PLAYBACK_STATE.STOPPED;
        }
    }

    public AudioHandler(EventInstance eventInstance)
    {
        _eventInstance = eventInstance;
    }

    #region Playback

    public void Play()
    {
        if (!IsValid)
        {
            return;
        }

        _eventInstance.start();
    }

    public void Stop(bool allowFadeOut = true)
    {
        if (!IsValid)
        {
            return;
        }

        STOP_MODE stopMode = allowFadeOut
            ? STOP_MODE.ALLOWFADEOUT
            : STOP_MODE.IMMEDIATE;

        _eventInstance.stop(stopMode);
    }

    public void Pause()
    {
        if (!IsValid)
        {
            return;
        }

        _eventInstance.setPaused(true);
    }

    public void Resume()
    {
        if (!IsValid)
        {
            return;
        }

        _eventInstance.setPaused(false);
    }

    #endregion

    #region Parameters

    public void SetParameter(
        string parameterName,
        float value)
    {
        if (!IsValid)
        {
            return;
        }

        _eventInstance.setParameterByName(
            parameterName,
            value);
    }

    public void SetParameter(
        PARAMETER_ID parameterId,
        float value)
    {
        if (!IsValid)
        {
            return;
        }

        _eventInstance.setParameterByID(
            parameterId,
            value);
    }

    #endregion

    #region 3D

    public void SetPosition(Vector3 position)
    {
        if (!IsValid)
        {
            return;
        }

        ATTRIBUTES_3D attributes = new ATTRIBUTES_3D
        {
            position = new VECTOR
            {
                x = position.x,
                y = position.y,
                z = position.z
            },
            velocity = new VECTOR
            {
                x = 0.0f,
                y = 0.0f,
                z = 0.0f
            },
            forward = new VECTOR
            {
                x = 0.0f,
                y = 0.0f,
                z = 1.0f
            },
            up = new VECTOR
            {
                x = 0.0f,
                y = 1.0f,
                z = 0.0f
            }
        };

        _eventInstance.set3DAttributes(attributes);
    }

    public void SetTransform(Transform transform)
    {
        if (!IsValid || transform == null)
        {
            return;
        }

        ATTRIBUTES_3D attributes = new ATTRIBUTES_3D
        {
            position = new VECTOR
            {
                x = transform.position.x,
                y = transform.position.y,
                z = transform.position.z
            },
            velocity = new VECTOR
            {
                x = 0.0f,
                y = 0.0f,
                z = 0.0f
            },
            forward = new VECTOR
            {
                x = transform.forward.x,
                y = transform.forward.y,
                z = transform.forward.z
            },
            up = new VECTOR
            {
                x = transform.up.x,
                y = transform.up.y,
                z = transform.up.z
            }
        };

        _eventInstance.set3DAttributes(attributes);
    }

    #endregion

    #region Volume

    public void SetVolume(float volume)
    {
        if (!IsValid)
        {
            return;
        }

        _eventInstance.setVolume(volume);
    }

    #endregion

    #region Callback

    public void SetCallback(
        EVENT_CALLBACK callback)
    {
        if (!IsValid)
        {
            return;
        }

        _eventInstance.setCallback(callback);
    }

    #endregion

    #region Lifecycle

    public void Update()
    {
        if (!IsValid)
        {
            return;
        }

        if (IsFinished)
        {
            Release();
        }
    }

    public void Release()
    {
        if (_isReleased)
        {
            return;
        }

        _isReleased = true;

        if (_eventInstance.isValid())
        {
            _eventInstance.release();
        }
    }

    #endregion
}