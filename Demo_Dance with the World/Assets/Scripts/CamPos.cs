using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using static UnityEditor.PlayerSettings;

public class CamPos : MonoBehaviour
{
    public Transform cameraPos;
    public PlayerMovement playerMovement;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.position = cameraPos.position;
        //this.transform.rotation = Quaternion.LookRotation(playerMovement.pos - cameraPos.position);
        this.transform.rotation = playerMovement.gameObject.transform.rotation;
    }
}
