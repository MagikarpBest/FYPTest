using UnityEngine;
using System;

/// <summary>
/// SceneInitializer is an optional startup object that put in scene.
/// It only initialize once all the all the scenes is being loaded then ensure random Singleton or FindObjectByType is execute smoothly.
/// Dependency Injection so hard to me understand ;-;
/// </summary>
public abstract class SceneInitializer : MonoBehaviour
{
    public bool IsTriggered { get; protected set; } = false;
    public abstract void Initialize(Action onComplete);
    public virtual void OnSceneStart()
    {
        SkyManager skyManager = FindAnyObjectByType<SkyManager>();
        if (skyManager != null)
        {
            skyManager.InitEnvironment();
        }
        return;
    }
}