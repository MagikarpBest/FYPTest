using System.Collections.Generic;
using UnityEngine;
using System;

public class SimpleScreenController : MonoBehaviour
{
    [SerializeField] private bool _debugMode = false;
    private readonly List<ScreenBase> screens = new();

    public event Action<bool> OnActiveChanged;

    public void Push(ScreenBase newScreen, bool instant = false)
    {
        // ==========================================================
        // unfocus current screen, if any
        // add new screen to end of collection
        // show the new screen, respecting 'instant'
        // ==========================================================
        // 'wasEmpty' Check whether thee stack is being from empty to non-empty, to trigger OnActiveChanged event.
        // So don't put this after the new screen is added, because that would always be true.
        bool wasEmpty = screens.Count == 0;

        if (screens.Count > 0)
        {
            ScreenBase current = screens[^1];
            current.Unfocus();
        }

        screens.Add(newScreen);
        newScreen.Show(instant);

        if (wasEmpty)
            OnActiveChanged?.Invoke(true);
    }

    public void Pop(bool instant = false)
    {
        // ==========================================================
        // hide current screen, respecting 'instant'
        // remove current screen from end of collection
        // focus previous screen
        // ==========================================================
        if (screens.Count == 0) return;

        ScreenBase current = screens[^1];
        current.Hide(instant);
        screens.RemoveAt(screens.Count - 1);

        if (screens.Count > 0)
        {
            screens[^1].Focus();
        }
        else
        {
            OnActiveChanged?.Invoke(false); // Trigger OnActiveChanged event when the stack becomes empty.
        }
    }

    public void Reset()
    {
        foreach (ScreenBase screen in screens)
        {
            screen.Hide(instant: true);
        }
        screens.Clear();
        OnActiveChanged?.Invoke(false);
    }

    private void Update()
    {
        // ==========================================================
        // IMPORTANT: Input System only! No Legacy Input allowed!
        // if Escape key is pressed...
        // check if collection has more than 0 screen in it
        // if yes, check if the current screen should honor back button
        //      if yes, pop
        // otherwise, do nothing
        // ==========================================================
        if (UnityEngine.InputSystem.Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (screens.Count > 0)
            {
                ScreenBase current = screens[^1];
                if (current != null && current.ShouldHonorBackButton())
                {
                    Pop();
                }
            }
        }
    }

    private void Awake() => GameScreenManager.Register(this);
    private void OnDestroy() => GameScreenManager.Unregister(this);

#if UNITY_EDITOR

    private void OnGUI()
    {
        if (!_debugMode) return;

        GUIStyle fontStyle = new GUIStyle();
        fontStyle.fontSize = 36;
        fontStyle.normal.textColor = Color.white;

        GUILayout.BeginVertical();

        GUILayout.Label("SimpleScreenManager [Editor DebugView]", fontStyle);
        GUILayout.Label("Screens:", fontStyle);
        for (int i = 0; i < screens.Count; i++)
        {
            bool isLast = i == screens.Count - 1;
            fontStyle.normal.textColor = isLast ? Color.green : Color.white;

            var screen = screens[i];
            GUILayout.BeginHorizontal();
            GUILayout.Space(20);
            GUILayout.Label($"[{i}] {screen.name} {(isLast ? "<--" : "")}", fontStyle);
            GUILayout.EndHorizontal();
        }

        GUILayout.EndVertical();
    }

#endif
}