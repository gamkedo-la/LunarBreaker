using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestructTimer : MonoBehaviour
{
    [SerializeField] private float removeTime = 5f;
    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject,removeTime);
    }

}
