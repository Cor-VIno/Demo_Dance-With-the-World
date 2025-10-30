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
    public readonly int LevelId;

    public LevelResetMessage(int levelId) {
        LevelId = levelId;
    }
}

public class SparkMessage {
}

public class MagneticController : MonoBehaviour {
    public E_MagMode magMode = E_MagMode.None;
    public int levelId;
    public bool canMove;
    public bool hasInfinityMag;
    public float colorChangeSpeed = 5f;

    private E_MagMode initMagType;
    private Vector3 initPos;
    private bool initCanMove;
    protected Outline outline;
    protected Rigidbody rb;
    private static readonly int Color1 = Shader.PropertyToID("_Color");

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
        initCanMove = canMove;
    }

    private void LevelReset() {
        SetMagMode(initMagType);
        transform.position = initPos;
        SetCanMove(initCanMove);
        rb.velocity = Vector3.zero;
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

    private void Spark(SparkMessage message) {
        Material mat = GetComponent<Renderer>().material;
        if (outline.OutlineColor != Color.black) {
            mat.SetColor(Color1, outline.OutlineColor);
        }
    }

    protected void UpdateColor() {
        if (magMode == E_MagMode.N)
            outline.OutlineColor = Color.red;
        else if (magMode == E_MagMode.S)
            outline.OutlineColor = Color.blue;
        else
            outline.OutlineColor = Color.black;
        Material mat = GetComponent<Renderer>().material;
        var tempColor = mat.GetColor(Color1);
        mat.SetColor(Color1, Color.Lerp(tempColor, Color.white, Time.deltaTime * colorChangeSpeed));
    }

    private Color FromRgba(float r, float g, float b, float a = 255) {
        return new Color(r / 255f, g / 255f, b / 255f, a / 255f);
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