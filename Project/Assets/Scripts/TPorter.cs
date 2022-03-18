using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TPorter : MonoBehaviour
{


    void OnTriggerStay(Collider other)
    {
        if (Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            if (other.gameObject.tag == "TPorter")
            {
                this.gameObject.transform.position = other.GetComponent<TPorterTarget>().targetTransform.transform.position;
            }
        }
    }
}
