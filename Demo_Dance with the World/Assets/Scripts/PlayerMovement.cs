using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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

    [HideInInspector]
    public Vector3 pos;

    public float APIX = 1000f;
    public float APIY = 1000f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        animator = GetComponent<Animator>();
        pos = Quaternion.AngleAxis(-changeAngleY, Vector3.right) * Quaternion.AngleAxis(changeAngleX, Vector3.up)
            * (cameraPos.forward * 10 + cameraPos.position); ;
    }

    void Update()
    {
        CheckGrounded();
        KeyInput();
        SpeedControl();
        SetDrag();
        RotateCamera();

    }

    private void OnAnimatorIK(int layerIndex)
    {
        animator.SetLookAtWeight(1, 0.1f, 1);
        animator.SetLookAtPosition(pos);


        //print(cameraPos.eulerAngles);
        //Camera.main.gameObject.transform
    }

    private void FixedUpdate()
    {
        MovePlayer();
        RotatePlayer();
    }
    void KeyInput()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        animator.SetFloat("Direction", horizontalInput);

        verticalInput = Input.GetAxis("Vertical");
        animator.SetFloat("Speed", Mathf.Min(1, Mathf.Sqrt(verticalInput * verticalInput + horizontalInput * horizontalInput)));

        if (Input.GetKey(jumpkey) && readyToJump && isGrounded)
        {
            readyToJump = false;
            k = changeAngleY;
            animator.SetBool("Jump", true);

            Invoke(nameof(ResetJump), jumpCooldown);
            //print("Jump!");
        }

        inputX = Input.GetAxis("Mouse X");
        //animator.SetFloat("Direction", inputX);
        if (!Input.GetKey(KeyCode.W))
        {
            changeAngleX += inputX;
            changeAngleX = Mathf.Clamp(changeAngleX, -51f, 51f);
        }


        changeAngleY += Input.GetAxis("Mouse Y");
        changeAngleY = Mathf.Clamp(changeAngleY, -65f, 70f);


        if (Input.GetKeyDown(KeyCode.W))
        {
            changeAngleX = 0;
            this.transform.rotation = Quaternion.LookRotation(new Vector3(pos.x, cameraPos.position.y, pos.z) - cameraPos.position);
        }

        if (animator.GetBool("Jump"))
        {
            k = Mathf.Lerp(k, -45f, Time.deltaTime * 2f);
            changeAngleY = Mathf.Clamp(changeAngleY, k, 70f);

        }
        //print(changeAngleY);

    }

    void MovePlayer()
    {
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
        if (Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.S) || Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.D))
        {
            moveDirection = Vector3.zero;
        }
        //animator.SetFloat(("Direction"),moveDirection.magnitude);
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
        //if (Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.S))
        //{
        //    rb.velocity = transform.TransformDirection(new Vector3(rb.velocity.x, rb.velocity.y, 0));
        //}
        //if (Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.D))
        //    rb.velocity = transform.TransformDirection(new Vector3(0, rb.velocity.y, rb.velocity.z));
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
        //print(changeAngleY);
        Debug.DrawLine(cameraPos.position, pos, Color.red);
    }

    void RotatePlayer()
    {
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