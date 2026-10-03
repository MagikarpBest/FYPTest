using UnityEngine;
using System;

public static class GameScreenManager
{
    private static SimpleScreenController current;
    // Listener provided for InputManager to know when to enable/disable player input.
    public static event Action<bool> OnUIActiveChanged;
    public static bool IsStackUIActive => current != null && !current.IsEmpty;

    public static void Register(SimpleScreenController manager)
    {
        if (current != null)
            current.OnActiveChanged -= HandleActiveChanged;

        current = manager;
        current.OnActiveChanged += HandleActiveChanged;
    }

    public static void Unregister(SimpleScreenController manager)
    {
        if (current != manager) return;

        current.OnActiveChanged -= HandleActiveChanged;
        current = null;

        OnUIActiveChanged?.Invoke(false);
    }

    private static void HandleActiveChanged(bool isActive) => OnUIActiveChanged?.Invoke(isActive);

    public static void Push(ScreenBase screen, bool instant = false)
    {
        if (current == null)
        {
            Debug.Log("No SimpleScreenManager registered!");
            return;
        }

        current.Push(screen, instant);
    }

    public static void Pop(bool instant = false)
    {
        if (current == null)
        {
            Debug.Log("No SimpleScreenManager registered!");
            return;
        }
        current.Pop(instant);
    }

    public static void Reset()
    {
        current.Reset();
    }
}