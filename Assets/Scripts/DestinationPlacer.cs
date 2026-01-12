using System.Collections;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.Collections.Generic;

public class DestinationPlacer : MonoBehaviour
{
    public GameObject destinationPrefab;
    private ARRaycastManager raycastManager;
    private GameObject spawnedDestination;

    static List<ARRaycastHit> hits = new List<ARRaycastHit>();

    void Start()
    {
        raycastManager = GetComponent<ARRaycastManager>();
    }

    void Update()
    {
        if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            if (raycastManager.Raycast(Input.GetTouch(0).position, hits, TrackableType.PlaneWithinPolygon))
            {
                Pose hitPose = hits[0].pose;
                Vector3 adjustedPosition = hitPose.position;
                adjustedPosition.y += 0.02f; 

                if (spawnedDestination == null)
                {
                    spawnedDestination = Instantiate(destinationPrefab, hitPose.position, Quaternion.Euler(90,0,0));
                }
                else
                {
                    spawnedDestination.transform.position = adjustedPosition;
                }
            }
        }
    }
}
