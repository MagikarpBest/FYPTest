using System.Collections.Generic;
using UnityEngine;

public class SimpleScreenController : MonoBehaviour
{
    private readonly List<ScreenBase> screens = new();

    public void Push(ScreenBase newScreen, bool instant = false)
    {
        // ==========================================================
        // unfocus current screen, if any
        // add new screen to end of collection
        // show the new screen, respecting 'instant'
        // ==========================================================
        if (screens.Count > 0)
        {
            ScreenBase current = screens[^1];
            current.Unfocus();
        }

        screens.Add(newScreen);
        newScreen.Show(instant);
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

    // The first screen to show, MUST NOT BE NULL
    [SerializeField] private ScreenBase startingScreen;

    // If true, the starting screen will instantly be shown
    [SerializeField] private bool instantlyShowStartingScreen = false;

    private void Awake() => GameScreenManager.Register(this);

    private void OnDestroy() => GameScreenManager.Unregister(this);

    private void Start()
    {
        if (startingScreen == null)
        {
            Debug.LogError("Starting screen is not assigned in SimpleScreenController!");
            return;
        }
        Push(startingScreen, instantlyShowStartingScreen);
    }

#if UNITY_EDITOR

    private void OnGUI()
    {
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