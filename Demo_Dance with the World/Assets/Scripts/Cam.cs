using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cam : MonoBehaviour
{
    public Transform cameraPosition;
    void Update()
    {
        this.transform.position = cameraPosition.position;
        this.transform.rotation = cameraPosition.rotation;
    }
}
