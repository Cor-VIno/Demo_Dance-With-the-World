using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum E_MagMode {
    None,
    N,
    S,
}

public class LevelResetMessage {
    public int LevelId;

    public LevelResetMessage(int levelId) {
        LevelId = levelId;
    }
}

public class MagneticController : MonoBehaviour {
    public E_MagMode magMode = E_MagMode.None;
    public int levelId;
    private E_MagMode initMagType;
    public bool canMove;
    public bool hasInfinityMag;
    protected Outline outline;
    protected Rigidbody rb;

    private void Awake() {
        Messager.Register<LevelResetMessage>(this, message => {
            if (message.LevelId == levelId) {
                Init();
            }
        });
    }

    protected void Start() {
        outline = GetComponent<Outline>();
        rb = GetComponent<Rigidbody>();
        UpdateColor();
        UpdateCanMove();
        initMagType = magMode;
        Init();
    }

    private void Init() {
        SetMagMode(initMagType);
    }

    private void Update() {
        UpdateColor();
    }

    public void SetMagMode(E_MagMode mode) {
        magMode = mode;
        UpdateColor();
    }

    public E_MagMode TakeMagMode() {
        E_MagMode mode = magMode;
        if (!hasInfinityMag) {
            magMode = E_MagMode.None;
            UpdateColor();
        }

        return mode;
    }

    protected void UpdateColor() {
        if (magMode == E_MagMode.N)
            outline.OutlineColor = Color.red;
        else if (magMode == E_MagMode.S)
            outline.OutlineColor = Color.blue;
        else
            outline.OutlineColor = Color.black;
    }

    public void LookingAt() {
        outline.OutlineWidth = 5f;
    }

    public void NotLookingAt() {
        outline.OutlineWidth = 0f;
    }

    public void SetCanMove(bool newCanMove) {
        canMove = newCanMove;
        UpdateCanMove();
    }

    protected void UpdateCanMove() {
        rb.constraints = canMove ? RigidbodyConstraints.None : RigidbodyConstraints.FreezeAll;
    }

    public void GenerateInteractionForce(GameObject formObj, float force, bool isAttract) {
        if (!canMove) {
            return;
        }

        Vector3 dis = formObj.transform.position - transform.position;
        rb.AddForce(dis.normalized * ((isAttract ? 1 : -1) * force), ForceMode.Force);
    }
}