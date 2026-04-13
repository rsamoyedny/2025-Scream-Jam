using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;

public class SteeringWheelControls : MonoBehaviour, IMouseInteract
{
    [SerializeField]
    private WheelBounds bounds;

    [SerializeField] private float _resetTime;

    [Serializable]
    private struct WheelBounds
    {
        public Vector2 Center;
        public float innerRadius;
        public float outerRadius;

        public readonly bool Contains(Vector2 position, out Vector2 delta)
        {
            delta = Center - position;
            float distanceTo = delta.magnitude;
            return distanceTo < outerRadius && distanceTo > innerRadius;
        }

        public readonly override string ToString()
        {
            return $"{{Center: ({Center.x}, {Center.y}) | Max Distance: {outerRadius} | Min Distance: {innerRadius}}}";
        }
    }
    
    // Lazy Singleton
    [HideInInspector]
    public static SteeringWheelControls Instance;
    void Awake() => Instance ??= this;

    private float originalDirection;
    
    /// <summary>
    /// Is the player dragging on the steering wheel with the intent of rotating it
    /// </summary>
    public bool IsWheelMoving { get; private set; }

    /// <summary>
    /// Negative angle = ccw motion; Positive angle = cw motion
    /// </summary>
    public float AngleToVertical { get; private set; }

    public static bool IsBeingSpun => Instance != null && Instance.AngleToVertical != 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InputSystem.actions["Attack"].canceled += context => ReleaseWheel();
        bounds = ConvertToPercentBounds(bounds);
    }

    public void OnPress(Vector2 mousePos)
    {
        if (!bounds.Contains(mousePos, out Vector2 direction)) return;

        StopCoroutine(ResetRotation());
        IsWheelMoving = true;
        originalDirection = GetAngleDeg(direction);
    }

    public void OnToggleOn()
    {
        ReleaseWheel();
    }

    public void OnMouseMove(Vector2 mousePos)
    {
        if (!IsWheelMoving)
            return;

        float newAngle = GetAngleDeg(bounds.Center - mousePos);
        float deltaAngle = originalDirection - newAngle;

        if (deltaAngle <= -180)
        {
            deltaAngle += 360;
        }

        AngleToVertical = deltaAngle;
    }

    public void ReleaseWheel()
    {
        originalDirection = 0f;
        IsWheelMoving = false;

        if (Instance != null)
        {
            StartCoroutine(ResetRotation());
        }
    }

    public IEnumerator ResetRotation()
    {
        float timer = 0;
        float startValue = AngleToVertical;

        while (timer < _resetTime)
        {
            timer += Time.deltaTime;

            AngleToVertical = Mathf.Lerp(startValue, 0, timer / _resetTime);

            yield return new WaitForEndOfFrame();
        }

        AngleToVertical = 0f;
    }

#if UNITY_EDITOR
    public void OnDrawGizmosSelected()
    {
        // WheelBounds newBounds = ConvertToPercentBounds(bounds);
        // Vector3 weightedPos = Camera.main.ScreenToWorldPoint(new Vector3(newBounds.Center.x, newBounds.Center.y, frustrumStart));

        // //Gizmos.DrawWireSphere(Camera.main.ScreenToWorldPoint(new Vector3(newBounds.Center.x,newBounds.Center.y, frustrumStart)), 0.1f * frustrumStart);

        // float worldHeight = Camera.main.orthographicSize;
        // float worldWidth = worldHeight * Camera.main.aspect;

        // newBounds.outerRadius = bounds.outerRadius / 100 * worldWidth;
        // newBounds.innerRadius = bounds.innerRadius / 100 * worldWidth;

        // Debug.Log(newBounds.outerRadius);

        // Gizmos.color = Color.green;
        // Gizmos.DrawWireSphere(weightedPos, newBounds.outerRadius * frustrumStart);

        //Gizmos.color = Color.red;

        //Gizmos.DrawWireSphere(weightedPos, newBounds.innerRadius * frustrumStart);
    }
#endif

    protected float GetAngleDeg(Vector2 directionVector) => Mathf.Atan2(directionVector.y, directionVector.x) * Mathf.Rad2Deg;

    private static WheelBounds ConvertToPercentBounds(WheelBounds bounds)
    {
        float yBounds = Camera.main.pixelHeight;
        float xBounds = yBounds * Camera.main.aspect;

        Vector2 modCenter = bounds.Center / 100;

        WheelBounds newBounds = new();

        newBounds.Center = new Vector2(modCenter.x * xBounds, modCenter.y * yBounds);

        newBounds.outerRadius = bounds.outerRadius / 100 * xBounds;
        newBounds.innerRadius = bounds.innerRadius / 100 * xBounds;

        return newBounds;
    }
}
