using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicRotation : MonoBehaviour
{
    public float speed = .3f;
    // Update is called once per frame
    void Update()
    {
        transform.Rotate(new Vector3(0, speed, 0));
    }
}
