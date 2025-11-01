using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum E_MagMode {
    None,
    N,
    S,
}

[RequireComponent(typeof(Outline), typeof(Rigidbody))]
public class MagneticController : MonoBehaviour {
    public E_MagMode magMode = E_MagMode.None;
    public int levelId;
    public bool canMove;
    public bool hasInfinityMag;
    public Material nMaterial;
    public Material sMaterial;
    public Material noneMaterial;

    private E_MagMode initMagType;
    private Vector3 initPos;
    private Vector3 initRot;
    private bool initCanMove;
    protected Outline outline;
    protected Rigidbody rb;

    private void Awake() {
        Messager.Register<LevelResetMessage>(this, message => {
            if (message.LevelId == levelId) {
                LevelReset();
            }
        });
        Messager.Register<SparkMessage>(this, Spark);
    }

    protected void Start() {
        outline = GetComponent<Outline>();
        rb = GetComponent<Rigidbody>();
        UpdateColor();
        UpdateCanMove();
        Init();
    }

    private void Init() {
        initMagType = magMode;
        initPos = transform.position;
        initRot = transform.eulerAngles;
        initCanMove = canMove;
    }

    private void LevelReset() {
        SetMagMode(initMagType);
        transform.position = initPos;
        transform.eulerAngles = initRot;
        SetCanMove(initCanMove);
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    private void Update() {
        UpdateColor();
    }

    public void SetMagMode(E_MagMode mode) {
        magMode = mode;
        //UpdateColor();
    }

    public E_MagMode TakeMagMode() {
        E_MagMode mode = magMode;
        if (!hasInfinityMag) {
            magMode = E_MagMode.None;
            UpdateColor();
        }

        return mode;
    }

    private void Spark(SparkMessage message) {
    }

    protected void UpdateColor() {
        if (magMode == E_MagMode.N) {
            outline.OutlineColor = Color.red;
            GetComponent<Renderer>().material = nMaterial;
        } else if (magMode == E_MagMode.S) {
            outline.OutlineColor = Color.blue;
            GetComponent<Renderer>().material = sMaterial;
        } else {
            outline.OutlineColor = Color.black;
            GetComponent<Renderer>().material = noneMaterial;
        }
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