using System.Collections.Generic;
using UnityEngine;
using System;

public class SimpleScreenController : MonoBehaviour
{
    [SerializeField] private bool _debugMode = false;
    [SerializeField] private PlayerInputManager _playerInputManager;

    private readonly List<ScreenBase> screens = new();
    public event Action<bool> OnActiveChanged;
    public bool IsEmpty => screens.Count == 0;

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

    private void Awake() => GameScreenManager.Register(this);
    private void Start()
    {
        _playerInputManager = FindFirstObjectByType<PlayerInputManager>();
        BindEscapePressedEvent(true);
    }
    private void OnDestroy() => GameScreenManager.Unregister(this);
    private void OnEnable()
    {
        BindEscapePressedEvent(true);
    }

    private void OnDisable()
    {
        BindEscapePressedEvent(false);
    }

    private void BindEscapePressedEvent(bool enable = true)
    {
        if (_playerInputManager == null)
            return;

        _playerInputManager.OnEscapePressed -= HandleEscapePressed;
        if (enable) _playerInputManager.OnEscapePressed += HandleEscapePressed;
    }

    private void HandleEscapePressed()
    {
        if (screens.Count == 0)
            return;

        ScreenBase current = screens[^1];
        if (current != null && current.ShouldHonorBackButton())
        {
            Debug.Log("pop in controller");
            Pop();
        }
    }

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