using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraManager : MonoBehaviour
{
    public delegate void CameraChangeEvent(PlayerLookState frame);
    public event CameraChangeEvent OnCameraChange;

    private Bounds2D _displayBounds;

    private PlayerLookState _currentLookState = PlayerLookState.None;
    public PlayerLookState PreviousLookState { get; private set; }
    public PlayerLookState CurrentLookState
    {
        get { return _currentLookState; }
        set
        {
            if (value == PlayerLookState.None) return;

            PreviousLookState = _currentLookState;
            _currentLookState = value;

            _activeMouseInteraction = GetActiveButtons(value);
            OnCameraChange?.Invoke(value);
        }
    }

    public PlayerLookState CameraLookState = PlayerLookState.None;

    public bool LookingRightOfWheel => (_currentLookState & PlayerLookState.RightFrames) > 0;
    public bool LookingDownVertical => (_currentLookState & PlayerLookState.Verticals) > 0;

    public Bounds2D DisplayBounds => _displayBounds;

    public static CameraManager Instance;

    private void Awake()
    {
        Instance = this;
        // if (Instance == null)
        // {
        //     Debug.Log(gameObject.name);
        //     Instance = this;
        //     DontDestroyOnLoad(gameObject);
        // }
        // else
        // {
        //     Destroy(gameObject);
        // }
    }

    void OnDestroy()
    {
        InputSystem.actions["Attack"].performed -= OnMouseClick;
        InputSystem.actions["Look"].performed -= OnMouseMove;
    }

    public List<FrameBounds> frames = new();

    private IMouseInteract[] _activeMouseInteraction = new IMouseInteract[0];
    public IMouseInteract[] ActiveButtons => _activeMouseInteraction;

    [HideInInspector] public bool IsInPlay = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;

        InputSystem.actions["Attack"].performed += OnMouseClick;
        InputSystem.actions["Look"].performed += OnMouseMove;
        float yBounds = Camera.main.pixelHeight;
        float xBounds = yBounds * Camera.main.aspect;
        _displayBounds = new Bounds2D(0, 0, Camera.main.scaledPixelWidth, Camera.main.scaledPixelHeight);

        CurrentLookState = PlayerLookState.SteeringWheel;
    }

    private void OnMouseClick(InputAction.CallbackContext context)
    {
        if (!IsInPlay) return;

        Vector2 mousePos = Mouse.current.position.value;

        foreach (var boundary in _activeMouseInteraction)
        {
            boundary.OnPress(mousePos);
        }
    }

    private void OnMouseMove(InputAction.CallbackContext context)
    {
        if (!IsInPlay) return;

        Vector2 mousePos = Mouse.current.position.value;

        foreach (var boundary in _activeMouseInteraction)
        {
            boundary.OnMouseMove(mousePos);
        }
    }

    public static Vector2 GetMousePos() => Mouse.current.position.value;

    public IMouseInteract[] GetActiveButtons(PlayerLookState direction)
    {
        List<IMouseInteract> mouseInteracts = new();

        foreach (var frame in frames) // Possible directions
        {
            if (frame.direction == direction) // Correct direction
            {
                foreach (var gameObject in frame.bounds) // Associated gameObjects
                {
                    if (!gameObject.activeSelf)
                    {
                        Debug.Log("Object disabled");
                        continue;
                    }
                    foreach (var boundary in gameObject.GetComponents<IMouseInteract>()) // GameObjects valid components
                    {
                        mouseInteracts.Add(boundary);
                        boundary.OnToggleOn();
                    }
                }
                return mouseInteracts.ToArray();
            }
        }
        return mouseInteracts.ToArray();
    }
}
