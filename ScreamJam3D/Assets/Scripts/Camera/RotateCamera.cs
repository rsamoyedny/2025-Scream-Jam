using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateCamera : MonoBehaviour
{
    [Serializable]
    private struct RotationNamePair
    {
        public PlayerLookState frame;
        public Quaternion rotation;
        public Vector3 position;
    }

    [SerializeField]
    private List<RotationNamePair> rotations;

    [SerializeField]
    private PlayerLookState defaultDirection;

    public float rotateTime = 0.1f;

    private readonly Dictionary<PlayerLookState, Quaternion> frameRotations = new();
    private readonly Dictionary<PlayerLookState, Vector3> framePositions = new();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foreach (var pair in rotations)
        {
            frameRotations[pair.frame] = pair.rotation;
            framePositions[pair.frame] = pair.position;
        }

        CameraManager.Instance.OnCameraChange += Rotate;
        Rotate(defaultDirection);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnDestroy()
    {
        CameraManager.Instance.OnCameraChange -= Rotate;
    }

    private void Rotate(PlayerLookState state)
    {
        StopCoroutine(Rotation(state));
        StartCoroutine(Rotation(state));
    }

    public IEnumerator Rotation(PlayerLookState state)
    {
        CameraManager.Instance.CameraLookState |= state;

        Transform t = Camera.main.transform;
        Quaternion start = t.localRotation;
        Quaternion end = frameRotations[state];

        Vector3 startPos = t.localPosition;
        Vector3 endPos = framePositions[state];

        float timer = 0;

        while (timer < rotateTime)
        {
            float time = timer / rotateTime;
            t.SetLocalPositionAndRotation(
                Vector3.Slerp(startPos, endPos, time),
                Quaternion.Slerp(start, end, time));
            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        t.localRotation = end;
        CameraManager.Instance.CameraLookState = state;
    }
}
