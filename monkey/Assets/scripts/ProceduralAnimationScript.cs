using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralAnimationScript : MonoBehaviour
{
    public Transform spider;
    public Transform originalPosition;
    public Vector3 targetPosition;
    public float distance = 5;
    public float limbTravelTime = 6;

    void Update()
    {
        transform.position = (targetPosition + transform.position * limbTravelTime) / (limbTravelTime + 1);
        if (Vector3.Distance(targetPosition, originalPosition.position) > distance)
        {
            targetPosition = originalPosition.position + originalPosition.forward * (distance / 1.5f);
            distance = 3 + Random.Range(0, 1.4f);
        }
    }
}
