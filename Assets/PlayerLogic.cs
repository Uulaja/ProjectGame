using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLogic : MonoBehaviour
{
    //movement
    public float moveSpeed;
    public float groundDrag;
    //groundcheck
    public float playerheight;
    public LayerMask whatIsGround;
    bool grounded;
    bool readytojump;
    //jump
    public float jumpForce;
    public float jumpCooldown;
    public KeyCode jumpkey = KeyCode.Space;
    public float airMultiplier;
    public Transform orientation;
    float horizontalInput;
    float verticalInput;
    Vector3 moveDirection;
    Rigidbody rb;

    //public Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        readytojump = true;
    }

    // Update is called once per frame
    void Update()
    {
        grounded = Physics.Raycast(transform.position, Vector3.down, playerheight * 0.5f + 0.2f, whatIsGround);
        Myinput();
        SpeedControl();
        if (grounded)
        {
            rb.drag = groundDrag;

        }
        else
        {
            rb.drag = 0;
        }
        Debug.Log("Kondisi ground " + grounded);
        Debug.Log("Kondisi jump" + readytojump);

    }
    public void FixedUpdate()
    {
        Movement();
    }

    private void Myinput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if (Input.GetKey(jumpkey) && readytojump && grounded)
        {
            readytojump = false;
            Jump();
            Invoke(nameof(ResetJump), jumpCooldown);
        }

    }
    private void Movement()
    {
        
        moveDirection = transform.forward * verticalInput + transform.right * horizontalInput;
        if (grounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
        }
        else if(!grounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
        }
        //anim.SetBool("Run", true);
    }
    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        if (flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }
    private void Jump()
    {
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
     }
    private void ResetJump()
    {
        readytojump = true;
    }
}
