using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadCollision : MonoBehaviour
{
    private void OnCollisionEnter(Collision other) {
        if (other.gameObject.CompareTag("Player")) {
            other.gameObject.GetComponent<PlayerMag>().Rebirth();
        }
    }
}
