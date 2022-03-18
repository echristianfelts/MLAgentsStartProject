using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdHocSensor : MonoBehaviour
{
    [SerializeField]
    public Transform target;


    // Start is called before the first frame update
    void Start()
    {
        transform.position = target.position;
        transform.rotation = target.rotation;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position = target.position;
        transform.rotation = target.rotation;
    }
}
