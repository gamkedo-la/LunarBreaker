using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbientZone : MonoBehaviour
{

    public Collider Area;
    public GameObject Player;

    void Update()
    {
        Vector3 closestPoint = Area.ClosestPoint(Player.transform.position);
        transform.position = closestPoint;
    }
}
