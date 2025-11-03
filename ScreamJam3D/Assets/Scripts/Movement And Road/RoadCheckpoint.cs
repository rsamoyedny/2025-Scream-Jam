using System.Collections.Generic;
using Unity.Hierarchy;
using UnityEngine;

public class RoadCheckpoint : MonoBehaviour
{
    // Fields
    private bool hit;
    private RoadGeneration roadGen;

    // Get the road generation script and save it as a reference
    void Start()
    {
        roadGen = GameObject.Find("Road Parent").GetComponent<RoadGeneration>();
    }
    
    // Check to see if this collider is the truck - If so, move the road forward!
    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == "Truck" && !hit)
        {
            hit = true;
            roadGen.NextRoadPiece(transform.parent.gameObject);
        }

    }
}