using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public enum States
    {
        NORMAL,
        JUMP,
        STATES
    };

    public States currentState;

    private Rigidbody rb;
    private Animator anim;

    private bool facingRight;

    [SerializeField]
    private float pMeterFillRate;
    [SerializeField]
    private float pMeterDropRate;
    [SerializeField]
    private float pMeterMax;

    public float pMeter;
    public bool pActive;

    [SerializeField]
    private float walkSpeed;
    [SerializeField]
    private float runSpeed;
    [SerializeField]
    private float topSpeed;
    [SerializeField]
    private float floatSpeed;
    [SerializeField]
    private float acceleration;

    private float speed;

    [SerializeField]
    private Vector2 jumpHeights;

    private float jumpHeight;

    [SerializeField]
    private float flyHeight;
    [SerializeField]
    private float floatHeight;

    private bool floating;

    [SerializeField]
    private Transform feet;
    [SerializeField]
    private LayerMask whatIsGround;
    [SerializeField]
    private float groundCheckRadius;
    [SerializeField]
    private bool grounded;

    private float horizontalAxis;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();

        facingRight = false;
    }

    private void LateUpdate()
    {
        anim.speed = (Mathf.Abs(rb.velocity.x) / 6.0f) + 0.5f;

        if (!grounded)
        {
            // Fly animations
            if (!pActive)
            {
                if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Jump") && rb.velocity.y > 0.5f && !floating)
                {
                    anim.Play("Jump");
                }

                if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Floating") && rb.velocity.y > -0.25f && floating)
                {
                    anim.Play("Floating");
                }

                if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Fall") && rb.velocity.y <= -0.25f)
                {
                    anim.Play("Fall");
                }
            }
            // Jump animations
            else
            {
                if (!anim.GetCurrentAnimatorStateInfo(0).IsName("PJump") && rb.velocity.y > 0.5f && !floating)
                {
                    anim.Play("PJump");
                }

                if (!anim.GetCurrentAnimatorStateInfo(0).IsName("PFly") && rb.velocity.y > -0.25f && floating)
                {
                    anim.Play("PFly");
                }

                if (!anim.GetCurrentAnimatorStateInfo(0).IsName("PFall") && rb.velocity.y <= -0.25f)
                {
                    anim.Play("PFall");
                }
            }
        }
        else
        {
            // Run animations
            if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Idle") && Mathf.Abs(rb.velocity.x) <= 0.1f)
            {
                anim.Play("Idle");
            }

            if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Walk") && Mathf.Abs(rb.velocity.x) > 0.1f && Mathf.Abs(rb.velocity.x) <= runSpeed)
            {
                anim.Play("Walk");
            }

            if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Run") && Mathf.Abs(rb.velocity.x) > runSpeed)
            {
                anim.Play("Run");
            }
        }

        if (facingRight)
        {
            transform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);
        }
        else
        {
            transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        horizontalAxis = Input.GetAxisRaw("Horizontal");

        grounded = Physics.CheckSphere(feet.position, groundCheckRadius, whatIsGround);

        if (rb.velocity.x > 0.0f)
            facingRight = true;
        else if (rb.velocity.x < 0.0f)
            facingRight = false;

        switch (currentState)
        {
            case States.NORMAL:
                // Player jumped
                if (!grounded)
                {
                    currentState = States.JUMP;
                    break;
                }

                // Reset speed and jump height
                speed = walkSpeed;
                jumpHeight = jumpHeights.x;

                // Player running
                if (Input.GetButton("Fire3"))
                    speed = runSpeed;

                // If player is above 90% of the run speed
                if (Mathf.Abs(rb.velocity.x) >= runSpeed * 0.9f)
                {
                    // Add to PMeter
                    if (pMeter < pMeterMax)
                    {
                        pMeter += Time.deltaTime * pMeterFillRate;
                    }
                    // PMeter has reached its max
                    else
                    {
                        pMeter = pMeterMax;

                        speed = topSpeed;
                        jumpHeight = jumpHeights.y;
                    }
                }
                else
                {
                    // Decrease PMeter
                    if (pMeter > 0.0f)
                        pMeter -= Time.deltaTime * pMeterDropRate;
                    else
                        pMeter = 0.0f;
                }

                // Set PActive (Player can fly now)
                if (pMeter == pMeterMax)
                    pActive = true;
                else
                    pActive = false;

                // Player jumps
                if (Input.GetButtonDown("Jump"))
                {
                    float jumpVel = Mathf.Sqrt(2 * Mathf.Abs(Physics.gravity.y) * jumpHeight);

                    rb.velocity = new Vector3(rb.velocity.x, jumpVel, rb.velocity.z);
                }
                break;
            case States.JUMP:
                if (grounded)
                {
                    if (pActive)
                        pMeter = 0.0f;

                    currentState = States.NORMAL;
                    break;
                }

                // Jump button released or falling
                if (!Input.GetButton("Jump") || rb.velocity.y < -0.25f)
                {
                    speed = walkSpeed;

                    floating = false;

                    rb.AddForce(Vector3.down);
                }

                if (pActive)
                {
                    if (pMeter > 0.0f)
                    {
                        pMeter -= Time.deltaTime;

                        // Propel the player up
                        if (Input.GetButtonDown("Jump"))
                        {
                            floating = true;

                            float jumpVel = Mathf.Sqrt(2 * Mathf.Abs(Physics.gravity.y) * flyHeight);

                            rb.velocity = new Vector3(rb.velocity.x, jumpVel, rb.velocity.z);
                        }
                    }
                    else
                    {
                        pMeter = 0.0f;
                        pActive = false;
                    }
                }
                else
                {
                    if (Input.GetButtonDown("Jump") && rb.velocity.y < -0.25f)
                    {
                        // Player can slow down their fall
                        speed = floatSpeed;

                        floating = true;

                        float jumpVel = Mathf.Sqrt(2 * Mathf.Abs(Physics.gravity.y) * floatHeight);

                        rb.velocity = new Vector3(rb.velocity.x, jumpVel, rb.velocity.z);
                    }

                    if (pMeter > 0.0f)
                        pMeter -= Time.deltaTime * pMeterDropRate;
                    else
                        pMeter = 0.0f;
                }
                break;
        }

        rb.velocity = Vector3.Lerp(rb.velocity, new Vector3(speed * horizontalAxis, rb.velocity.y, rb.velocity.z), acceleration * Time.deltaTime);
    }
}
