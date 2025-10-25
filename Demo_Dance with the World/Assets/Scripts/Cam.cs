using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cam : MonoBehaviour
{
    public Transform cameraPosition;
    //public PlayerMovement playerMovement;
    void Update()
    {
        this.transform.position = cameraPosition.position;
        //Vector3 pos = 
        //this.transform.rotation = cameraPosition.rotation;
        //print(transform.eulerAngles);
    }
}
