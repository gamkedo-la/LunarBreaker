using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;

public class Agent_MoveTowardsTarget : MonoBehaviour
{
    public Rigidbody rigidbody;
    public float moveSpeed = 1f;
    public float distanceToConsiderWaypointReached = 2f;
    public Transform positionTarget;

    public UnityEvent JustReachedTarget;

    private bool reachedPosTarget = false;

    void Start()
    {
    }

    void Update()
    {
        if (positionTarget != null)
        {
            // check whether we still need to approach
            Vector3 posTargetWithOurHeight = positionTarget.position;
            posTargetWithOurHeight.y = transform.position.y;
            float dist = Vector3.Distance(transform.position, posTargetWithOurHeight);
            bool hadPreviouslyReached = reachedPosTarget;
            reachedPosTarget = dist <= distanceToConsiderWaypointReached;

            // move towards the waypoint
            if (!reachedPosTarget)
            {
                transform.LookAt(posTargetWithOurHeight);
                rigidbody.velocity = transform.forward * moveSpeed;
            }

            if(!hadPreviouslyReached && reachedPosTarget)
            {
                JustReachedTarget.Invoke();
            }
        }
        // stay still when waypoint is reached or no waypoint is set
        if(reachedPosTarget || positionTarget == null)
        {
            rigidbody.velocity = Vector3.zero;
        }
    }
}
