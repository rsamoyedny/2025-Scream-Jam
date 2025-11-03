using System.Collections.Generic;
using UnityEngine;

public class RoadGeneration : MonoBehaviour
{
    // Fields
    [Header("Editable Values")]
    public int maxRoadPieces;
    [Tooltip("WARNING: Going above 90 can lead to intersection!")]
    public float maxRoadAngle;
    public int maxRoadPiecesBehindPlayer;

    [Header("Scene Objects/Prefabs")]
    public Transform truckTransform;
    public Transform roadParent;
    public List<GameObject> roadPieces;
    public GameObject checkpointPrefab;

    // Road pieces present in scene
    private List<GameObject> placedRoad;

    // Current road direction, used to stop intersection
    private float angle;

    // Initial offset Vector
    private Vector3 _initialOffset = new(0,-4,10);

    // Place 1st road piece, create more as needed to fill list
    void Start()
    {
        placedRoad = new List<GameObject>(maxRoadPieces + 1);
        for (int i = 0; i < maxRoadPieces; i++)
            NextRoadPiece(null);
    }

    /// <summary>
    /// Place a road piece and have it align with previous pieces if they exist
    /// </summary>
    public void NextRoadPiece(GameObject crossedRoad)
    {
        int index = -1;
        // Ensure road crossed isn't the back, allowing for deletion of back and placement of new road
        if (crossedRoad != null && placedRoad.Count > 0)
        {
            for (int i = 0; i < placedRoad.Count; i++)
            {
                if (crossedRoad == placedRoad[i])
                {
                    index = i;
                    break;
                }
            }
        }
        // Pick a random new road piece and add it to the list of placed road pieces
        if (index != 0)
        {
            int roadPieceIndex = Random.Range(0, roadPieces.Count);
            GameObject newRoadPiece = Object.Instantiate(roadPieces[roadPieceIndex], Vector3.zero, Quaternion.identity, roadParent);
            Transform newRoadTransform = newRoadPiece.transform;
            float turnAngle = 0;
            switch (roadPieceIndex)
            {
                case 1:
                    turnAngle = 65.5f;
                    break;
                case 2:
                    turnAngle = 27.5f;
                    break;
            }
            // Random turning or turning to avoid intersection
            bool choice = Random.Range(0f, 1f) > 0.5f;
            if (angle + turnAngle > maxRoadAngle)
                choice = true;
            if (angle - turnAngle < -maxRoadAngle)
                choice = false;
            if (roadPieceIndex > 0 && choice)
            {
                BoxCollider checkpoint = newRoadPiece.GetComponentInChildren<BoxCollider>();
                Vector3 checkpointPosition = Vector3.Scale(checkpoint.transform.position, new Vector3(1, 1, -1));
                Quaternion checkpointRotation = checkpoint.transform.rotation;
                Object.Destroy(checkpoint.gameObject);
                newRoadTransform.localScale = new Vector3(1, 1, -1);
                turnAngle *= -1;
                Component.Instantiate(checkpointPrefab, checkpointPosition, checkpointRotation, newRoadTransform);
            }
            placedRoad.Add(newRoadPiece);
            angle += turnAngle;
            // Overflow, remove last road
            if (placedRoad.Count > maxRoadPieces && index > maxRoadPiecesBehindPlayer - 1)
            {
                GameObject roadToDelete = placedRoad[0];
                placedRoad.RemoveAt(0);
                Object.Destroy(roadToDelete);
            }
            // If this is the 1st piece, put the truck on its spawn point
            if (placedRoad.Count == 1)
            {
                Transform spawn = GetTransform(newRoadTransform, "Spawn");
                roadParent.position = truckTransform.position + _initialOffset + (newRoadTransform.position - spawn.position);
            }
            // Otherwise, place new piece such that its connected aligns with the previous piece's pivot
            else
            {
                Transform previousPiece = placedRoad[placedRoad.Count - 2].transform;
                Transform previousPivot = GetTransform(previousPiece, "Pivot");
                newRoadTransform.rotation = previousPivot.rotation;
                newRoadTransform.position = previousPivot.position + (newRoadTransform.position - GetTransform(newRoadTransform, "Connector").position);
            }
        }
    }

    /// <summary>
    /// Gets the child transform of an object with a given tag
    /// </summary>
    /// <param name="parent">The parent transform</param>
    /// <param name="tag">The tag of the child transform</param>
    /// <returns>The desired child transform, or the parent transform if the child doesn't exist</returns>
    private Transform GetTransform(Transform parent, string tag)
    {
        Transform[] childTransforms = parent.GetComponentsInChildren<Transform>();
        foreach (Transform childTransform in childTransforms)
        {
            if (childTransform.tag == tag)
                return childTransform;
        }
        return parent;
    }
}