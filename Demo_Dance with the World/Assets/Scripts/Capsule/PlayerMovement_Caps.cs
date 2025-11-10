using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement_Caps : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;

    public float groundDrag;
    public float magDrag = 10f;

    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    bool readyToJump;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    public LayerMask magnetometric;
    bool grounded;
    public float sphereRadius = 0.2f;

    bool maged;


    public Transform orientation;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        readyToJump = true;
    }

    private void Update()
    {
        //grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround)|| Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, magnetometric);
        CheckGroundedMaged();
        MyInput();
        SpeedControl();


        // Handle drag
        if (maged)
            rb.drag = magDrag;
        else if (grounded)
            rb.drag = groundDrag;
        else
            rb.drag = 0;
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    void CheckGroundedMaged()
    {
        // 方案1：使用SphereCast
        float checkDistance = playerHeight * 0.5f + 0.1f;
        Vector3 sphereStart = transform.position + Vector3.up * sphereRadius;

        grounded = Physics.SphereCast(sphereStart, sphereRadius, Vector3.down, out RaycastHit hit, checkDistance, whatIsGround);
        maged = Physics.SphereCast(sphereStart, sphereRadius, Vector3.down, out hit, checkDistance, magnetometric);

        // 方案2：使用CheckSphere（注释掉上面的，取消注释下面的）

        //Vector3 checkPosition = transform.position - Vector3.up * (playerHeight * 0.5f - sphereRadius);
        //grounded = Physics.CheckSphere(checkPosition, sphereRadius, whatIsGround) 
        //          || Physics.CheckSphere(checkPosition, sphereRadius, magnetometric);

    }

    void MyInput()
    {
        // Input
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        // Jump
        if (Input.GetKey(jumpKey) && readyToJump && (grounded || maged))
        {
            readyToJump = false;
            Jump();
            Invoke(nameof(RestJump), jumpCooldown);
        }
    }

    void MovePlayer()
    {
        // Calculate movement direction
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
        // Move the player
        if (grounded || maged)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
        else
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
    }

    void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        // Limit velocity if needed
        if (flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }

    void Jump()
    {
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    void RestJump()
    {
        readyToJump = true;
    }
}
