using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamPos : MonoBehaviour
{
    public Transform cameraPos;
    public Transform playerPos;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.position = cameraPos.position;
        this.transform.rotation = playerPos.rotation;
    }
}
