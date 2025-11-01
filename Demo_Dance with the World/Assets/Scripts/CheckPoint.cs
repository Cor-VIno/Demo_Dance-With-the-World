using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;

public class CheckPoint : MonoBehaviour {
    public int levelId;
    [Header("请按照入栈顺序填写")] public List<E_MagMode> playerHasMagTypes = new(2);
    private bool hasCollision;
    private bool isTeleport;
    private Vector3 rebirthPos;

    private void Awake() {
        Messager.Register<PlayerNeedResetMessage>(this, ResetPlayer);
        rebirthPos = transform.Find("RebirthPos").position;
    }

    private void ResetPlayer(PlayerNeedResetMessage message) {
        if (message.LevelId != levelId) {
            return;
        }

        isTeleport = true;
        message.PlayerRigidbody.velocity = Vector3.zero;
        message.PlayerRigidbody.angularVelocity = Vector3.zero;
        message.PlayerMagComponent.InsideReset(playerHasMagTypes);
        message.PlayerTransform.position = rebirthPos + new Vector3(0, 1.5f, 0);
        Messager.Send(new LevelResetMessage(levelId));
        if (message.IsRebirth) {
            Messager.Send(new DisplayMessage("已重生", 3f));
        } else {
            Messager.Send(new DisplayMessage("已重置当前关卡物品", 3f));
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (!hasCollision && !isTeleport && other.attachedRigidbody.CompareTag("Player")) {
            Messager.Send(new CheckPointMessage(levelId));
            Messager.Send(new DisplayMessage("已设置重生点", 3f));
            hasCollision = true;
        }
    }

    private void OnTriggerExit(Collider other) {
        if (hasCollision && other.attachedRigidbody.CompareTag("Player")) {
            hasCollision = false;
            isTeleport = false;
        }
    }
}