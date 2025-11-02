using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class MainMenuRoadMover : MonoBehaviour
{
    [SerializeField] private float roadVelocity;
    private Vector3 _movementVector;

    void Start()
    {
        _movementVector = new Vector3(roadVelocity, 0, 0);
    }

    void Update()
    {
        Vector3 currDelta = _movementVector * Time.deltaTime;
        
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).position -= currDelta;
        }
    }
}
