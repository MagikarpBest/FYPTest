using UnityEngine;

public static class GameScreenManager
{
    private static SimpleScreenController current;

    public static void Register(SimpleScreenController manager) => current = manager;

    public static void Unregister(SimpleScreenController manager)
    {
        if (current == manager)
            current = null;
    }

    public static void Push(ScreenBase screen)
    {
        if (current == null)
        {
            Debug.Log("No SimpleScreenManager registered!");
            return;
        }

        current.Push(screen);
    }

    public static void Pop()
    {
        if (current == null)
        {
            Debug.Log("No SimpleScreenManager registered!");
            return;
        }
        current.Pop();
    }
}