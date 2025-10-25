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
    float changeAngleY;
    float k;

    public Vector3 pos;
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
        RotatePlayer();




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
    }
    void KeyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        //animator.SetFloat("Direction", horizontalInput);

        verticalInput = Input.GetAxisRaw("Vertical");
        animator.SetFloat("Speed", verticalInput);

        if (Input.GetKey(jumpkey) && readyToJump && isGrounded)
        {
            readyToJump = false;

            animator.SetBool("Jump", true);

            Invoke(nameof(ResetJump), jumpCooldown);
            print("Jump!");
        }


        changeAngleX += Input.GetAxis("Mouse X");
        changeAngleX = Mathf.Clamp(changeAngleX, -51f, 51f);
        
        changeAngleY += Input.GetAxis("Mouse Y");
        changeAngleY = Mathf.Clamp(changeAngleY, -65f, 70f);
        if (animator.GetBool("Jump"))
        {
            k = Mathf.Lerp(k, -65f, Time.deltaTime);
            changeAngleY = Mathf.Clamp(changeAngleY, k, 70f);
        }
    }

    void MovePlayer()
    {
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        if (isGrounded)
        {
            if (verticalInput > 0)
                rb.AddForce(moveDirection.normalized * moveSpeed * 5f, ForceMode.Force);
            if (verticalInput < 0)
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
        Quaternion rot = Quaternion.AngleAxis(changeAngleX, Vector3.up) *
                 Quaternion.AngleAxis(-changeAngleY, Vector3.right);
        pos = rot * (cameraPos.forward * 10) + cameraPos.position;

        Camera.main.transform.rotation = Quaternion.LookRotation(pos - cameraPos.position);
        print(changeAngleY);
        Debug.DrawLine(cameraPos.position, pos, Color.red);
    }

    void RotatePlayer()
    {
        if (changeAngleX == 51f)
        {
            changeAngleX--;
            transform.rotation = transform.rotation * Quaternion.Euler(0f, 2.5f, 0f);
        }
        if (changeAngleX == -51f)
        {
            changeAngleX++;
            transform.rotation = transform.rotation * Quaternion.Euler(0f, -2.5f, 0f);
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
        k = changeAngleY;



    }
}