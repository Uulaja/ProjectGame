using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLogic : MonoBehaviour
{
    //movement
    public float moveSpeed;
    public float groundDrag;
    //groundcheck
    public float playerHeight;
    public LayerMask whatIsGround;
    bool grounded;
    bool readyToJump;
    //jump
    public float jumpForce;
    public float jumpCooldown;
    public KeyCode jumpKey = KeyCode.Space;
    public float airMultiplier;
    public Transform orientation;
    float horizontalInput;
    float verticalInput;
    Vector3 moveDirection;
    Rigidbody rb;

    public Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        readyToJump = true;
    }

    // Update is called once per frame
    void Update()
    {
        // Ground check
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);
        MyInput();
        SpeedControl();
        shootlogic();
        dodge();
        useitem();
        // Set drag based on grounded state
        if (grounded)
        {
            rb.drag = groundDrag;
        }
        else
        {
            rb.drag = 0;
        }

        // Debugging logs
        Debug.Log("Grounded: " + grounded);
        Debug.Log("Ready to Jump: " + readyToJump);
    }

    // FixedUpdate is called once per physics frame
    public void FixedUpdate()
    {
        Movement();
    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        // Jump logic
        if (Input.GetKey(jumpKey) && readyToJump && grounded)
        {
            readyToJump = false;
            Jump();
            Invoke(nameof(ResetJump), jumpCooldown);
        }
        
    }

    private void Movement()
    {
        moveDirection = transform.forward * verticalInput + transform.right * horizontalInput;

        if (grounded)
        {
            // Trigger running animation when moving
            if (horizontalInput != 0 || verticalInput != 0)
            {
                anim.SetBool("run", true);
            }
            else
            {
                anim.SetBool("run", false);
            }

            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
        }
        else if (!grounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
        }
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        // Limit velocity if needed
        if (flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }

    private void Jump()
    {
        // Reset Y velocity to ensure consistent jump force
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
        anim.SetBool("jump", true);
    }

    private void ResetJump()
    {
        readyToJump = true;
        anim.SetBool("jump", false);

    }
    private void shootlogic()
    {
        if (Input.GetKey(KeyCode.Mouse0))
        {
            anim.SetBool("shoot", true);
        }
        else
        {
            anim.SetBool("shoot", false);
        }
    }
    private void dodge()
    {
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.D))
        {
            
            anim.SetBool("Dright", true);
            anim.SetBool("Dleft", false);
        }
        else if (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.A))
        {
            anim.SetBool("Dright", false);
            anim.SetBool("Dleft", true);
        }
        else
        {
            anim.SetBool("Dright", false);
            anim.SetBool("Dleft", false);
        }
    }
    private void useitem()
    {
        if (Input.GetKey(KeyCode.E))
        {
            anim.SetBool("use", true);
        }
        else
        {
            anim.SetBool("use", false);
        }
    }
}
