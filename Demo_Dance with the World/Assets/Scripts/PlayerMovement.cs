using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed;
    public float groundDrag;
    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    bool readyToJump = true;

    [Header("Keybinds")]
    public KeyCode jumpkey = KeyCode.Space;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    bool isGrounded;

    public Transform orientation;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    Rigidbody rb;

    Animator animator;
    public Transform cameraPos;
    float changeAngleX;
    float changeRotX;
    float changeAngleY;
    float k;
    float inputX;
    bool isR;
    [HideInInspector]
    public Vector3 pos;

    public float APIX = 1000f;
    public float APIY = 1000f;

    private bool isAligningBody = false;
    private bool isFinalAdjustment = false;
    private Quaternion targetBodyRotation;
    private Quaternion initialBodyRotation;
    private Quaternion initialCameraRotation;
    private float rotationProgress;
    private float rotationDuration;
    private Vector3 rotationAxis;
    private float initialAngleDifference;
    private Vector3 targetCameraLookAt;
    private float torqueReductionThreshold = 0.8f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        animator = GetComponent<Animator>();
        pos = Quaternion.AngleAxis(-changeAngleY, Vector3.right) * Quaternion.AngleAxis(changeAngleX, Vector3.up)
            * (cameraPos.forward * 10 + cameraPos.position);

        isR = false;
        isAligningBody = false;
        isFinalAdjustment = false;
    }

    void Update()
    {
        CheckGrounded();
        KeyInput();
        SpeedControl();
        SetDrag();

        if (!isAligningBody && !isFinalAdjustment)
        {
            RotateCamera();
        }
        Debug.DrawLine(cameraPos.position, new Vector3(pos.x, cameraPos.position.y, pos.z), Color.red);
    }

    private void OnAnimatorIK(int layerIndex)
    {
        animator.SetLookAtWeight(1, 0.1f, 1);
        animator.SetLookAtPosition(pos);
    }

    private void FixedUpdate()
    {
        MovePlayer();

        if (!isFinalAdjustment)
        {
            RotatePlayer();
        }
    }

    void KeyInput()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        animator.SetFloat("Direction", horizontalInput);

        verticalInput = Input.GetAxis("Vertical");
        if (verticalInput > 0)
            animator.SetFloat("Speed", Mathf.Min(1, Mathf.Sqrt(verticalInput * verticalInput + horizontalInput * horizontalInput)));
        else if (verticalInput < 0)
            animator.SetFloat("Speed", Mathf.Max(-1, -Mathf.Sqrt(verticalInput * verticalInput + horizontalInput * horizontalInput)));
        else
            animator.SetFloat("Speed", -Mathf.Abs(horizontalInput / 2));

        if (Input.GetKey(jumpkey) && readyToJump && isGrounded)
        {
            readyToJump = false;
            k = changeAngleY;
            animator.SetBool("Jump", true);
            Invoke(nameof(ResetJump), jumpCooldown);
        }

        inputX = Input.GetAxis("Mouse X");

        if (!isAligningBody && !isFinalAdjustment)
        {
            if (!Input.GetKey(KeyCode.W))
            {
                changeAngleX += inputX;
                changeAngleX = Mathf.Clamp(changeAngleX, -51f, 51f);
            }
        }

        changeAngleY += Input.GetAxis("Mouse Y");
        changeAngleY = Mathf.Clamp(changeAngleY, -65f, 70f);

        if ((Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D)) && !isAligningBody && !isFinalAdjustment && isGrounded)
        {
            StartCoroutine(AlignBodyWithCamera());
        }

        if (animator.GetBool("Jump"))
        {
            k = Mathf.Lerp(k, -45f, Time.deltaTime * 2f);
            changeAngleY = Mathf.Clamp(changeAngleY, k, 70f);
        }
    }

    IEnumerator AlignBodyWithCamera()
    {
        isAligningBody = true;

        initialBodyRotation = transform.rotation;
        initialCameraRotation = Camera.main.transform.rotation;

        Vector3 cameraForward = Camera.main.transform.forward;
        cameraForward.y = 0;
        targetBodyRotation = Quaternion.LookRotation(cameraForward);

        initialAngleDifference = Quaternion.Angle(initialBodyRotation, targetBodyRotation);
        rotationDuration = Mathf.Clamp(initialAngleDifference / 180f, 0.1f, 0.5f);

        rotationAxis = Vector3.Cross(transform.forward, cameraForward.normalized);
        rotationAxis = rotationAxis.normalized;

        rb.angularVelocity = Vector3.zero;

        float initialTorque = initialAngleDifference * APIX * 0.001f;
        rb.AddTorque(rotationAxis * initialTorque, ForceMode.VelocityChange);

        targetCameraLookAt = pos;

        rotationProgress = 0f;
        while (rotationProgress < 1f)
        {
            rotationProgress += Time.deltaTime / rotationDuration;
            rotationProgress = Mathf.Clamp01(rotationProgress);

            float currentAngle = Quaternion.Angle(transform.rotation, targetBodyRotation);

            float torqueFactor = 1f;
            if (rotationProgress > torqueReductionThreshold)
            {
                torqueFactor = 1f - (rotationProgress - torqueReductionThreshold) / (1f - torqueReductionThreshold);
            }

            float torqueMagnitude = currentAngle * APIX * Time.deltaTime * torqueFactor;
            rb.AddTorque(rotationAxis * torqueMagnitude, ForceMode.VelocityChange);

            Camera.main.transform.rotation = initialCameraRotation;

            if (currentAngle < 5f)
            {
                break;
            }

            yield return null;
        }

        isAligningBody = false;
        isFinalAdjustment = true;

        float adjustmentDuration = 0.1f;
        float adjustmentProgress = 0f;
        Quaternion startRotation = transform.rotation;

        while (adjustmentProgress < 1f)
        {
            adjustmentProgress += Time.deltaTime / adjustmentDuration;
            adjustmentProgress = Mathf.Clamp01(adjustmentProgress);

            transform.rotation = Quaternion.Slerp(startRotation, targetBodyRotation, adjustmentProgress);

            Camera.main.transform.rotation = initialCameraRotation;

            yield return null;
        }

        transform.rotation = targetBodyRotation;

        changeAngleX = 0f;
        RotateCamera();

        isFinalAdjustment = false;
    }

    void MovePlayer()
    {
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        if (isGrounded)
        {
            if (animator.GetFloat("Speed") > 0)
                rb.AddForce(moveDirection.normalized * moveSpeed * 5f, ForceMode.Force);
            if (animator.GetFloat("Speed") < 0)
                rb.AddForce(moveDirection.normalized * moveSpeed * 2.5f, ForceMode.Force);
        }
        else
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
    }

    void CheckGrounded()
    {
        isGrounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);
    }

    void SetDrag()
    {
        if (isGrounded)
            rb.drag = groundDrag;
        else
            rb.drag = 0.2f;
    }

    void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        if (flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }

    void ResetJump()
    {
        readyToJump = true;
    }

    void RotateCamera()
    {
        Quaternion rot = Quaternion.AngleAxis(changeAngleX, this.transform.up) *
                 Quaternion.AngleAxis(-changeAngleY, this.transform.right);
        pos = rot * (cameraPos.forward * 10) + cameraPos.position;

        Camera.main.transform.rotation = Quaternion.LookRotation(pos - cameraPos.position);
    }

    void RotatePlayer()
    {
        if (isAligningBody) return;

        if (changeAngleX == 51f || changeAngleX == -51f)
        {
            rb.AddTorque(transform.up * inputX * APIX / 300, ForceMode.VelocityChange);
        }

        if (Input.GetKey(KeyCode.W))
        {
            rb.AddTorque(transform.up * inputX * APIX / 300, ForceMode.VelocityChange);
        }
    }

    void JumpOver()
    {
        animator.SetBool("Jump", false);
        animator.SetBool("Rest", false);
    }

    void Jump()
    {
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }
}