using UnityEngine;

public class RotateSteeringWheel : MonoBehaviour
{
    private Quaternion _startingRotation;

    void Start()
    {
        _startingRotation = transform.localRotation;
    }

    // Update is called once per frame
    void Update()
    {
        if (SteeringWheelControls.Instance != null)
        {
            transform.localRotation = _startingRotation;
            transform.RotateAround(transform.position, transform.up, SteeringWheelControls.Instance.AngleToVertical);
        }
    }
}
