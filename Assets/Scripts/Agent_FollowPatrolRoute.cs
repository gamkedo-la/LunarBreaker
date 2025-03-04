using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent_FollowPatrolRoute : MonoBehaviour
{
    public GameObject patrolRoute;
    private Agent_MoveTowardsTarget agent; // assumed to be on the same game object

    public bool oneshot = false; // whether to patrol only once
    public bool pingpong = true;
    private bool loop { get => !oneshot && !pingpong; }

    private int curWaypointIdx = 0;
    private int nbWaypoints;
    private List<Transform> waypoints = new();
    private bool goingForward = true;

    void Start()
    {
        agent = gameObject.GetComponent<Agent_MoveTowardsTarget>();
        if (patrolRoute == null || agent == null) { return; }

        // gather waypoints
        foreach (Transform wp in patrolRoute.GetComponentsInChildren<Transform>())
        {
            if(wp == patrolRoute.transform) { continue; }
            waypoints.Add(wp);
            nbWaypoints += 1;
        }
        // react when target is reached
        agent.JustReachedTarget.AddListener(OnReachedTarget);
        // go!
        SetAgentTargetToCurrentWaypoint();
    }

    void OnReachedTarget()
    {
        int lastWaypoint = goingForward ? nbWaypoints-1 : 0;
        bool wasTargetingLastWaypoint = curWaypointIdx == lastWaypoint;

        if(wasTargetingLastWaypoint && oneshot)
        { // we've arrived!
            agent.positionTarget = null;
            return;
        }
        
        if(wasTargetingLastWaypoint && pingpong)
        { // go the other way
            goingForward = !goingForward;
        }

        curWaypointIdx += goingForward ? 1 : -1;
        if (wasTargetingLastWaypoint && loop)
        { // loop
            curWaypointIdx = 0;
        }

        SetAgentTargetToCurrentWaypoint();
    }

    void SetAgentTargetToCurrentWaypoint()
    {
        agent.positionTarget = waypoints[curWaypointIdx];
    }
}
