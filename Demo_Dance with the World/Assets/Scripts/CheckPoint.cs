using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour {
    public int levelId;

    private void OnCollisionEnter(Collision other) {
        if (other.gameObject.CompareTag("Player")) {
            Messager.Send(new LevelResetMessage(levelId));
        }
    }
}