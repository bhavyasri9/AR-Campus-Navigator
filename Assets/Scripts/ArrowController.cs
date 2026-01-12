using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowController : MonoBehaviour
{
    public Transform arCamera;
    public Transform destination;

    void Start()
    {
        if (arCamera == null)
        {
            arCamera = Camera.main.transform;
        }
    }

    void Update()
    {
        if (destination == null) return;

        Vector3 direction = destination.position - transform.position;
        direction.y = 0; // keep arrow flat

        if (direction.magnitude > 0.1f)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }
}
