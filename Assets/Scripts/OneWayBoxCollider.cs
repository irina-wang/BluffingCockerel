using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]

public class OneWayBoxCollider : MonoBehaviour
{
    [SerializeField] private Vector3 entryDirection = Vector3.up;
    [SerializeField] private bool localDirection = false;
    [SerializeField] private float triggerScale = 1.25f;

    private new BoxCollider collider = null;

    private BoxCollider collisionCheckTrigger = null;

    private void Awake()
    {
        collider = GetComponent<BoxCollider>();
        collider.isTrigger = false;

        collisionCheckTrigger = gameObject.AddComponent<BoxCollider>();
        collisionCheckTrigger.size = collider.size * triggerScale;
        collisionCheckTrigger.center = collider.center;
        collisionCheckTrigger.isTrigger = true;
    }

    private void OnTriggerStay(Collider other) 
    {
        if (Physics.ComputePenetration(
            collisionCheckTrigger, transform.position, transform.rotation,
            other, other.transform.position, other.transform.rotation,
            out Vector3 collisionDirection, out float penetrationDepth))
        {
            Vector3 direction;
            // transform into location direction
            if (localDirection) {
                direction = transform.TransformDirection(entryDirection.normalized);
            } else {
                direction = entryDirection;
            }


            float dot = Vector3.Dot(direction, collisionDirection);
            // Opposite direction; passing is not allowed
            if (dot < 0) {
                Physics.IgnoreCollision(collider, other, false);
            } else {
                Physics.IgnoreCollision(collider, other, true);
            }
        }
    }
}