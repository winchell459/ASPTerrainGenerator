using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float Speed = 5;
    public float Jump = 10;
    public float MaxJumpSpeed = 5;

    private Rigidbody2D rb;
    private Animator anim;

    public float raycastLength = 0.5f;
    public LayerMask groundMask;
    public bool GroundCheck;

    private PlayerHandler ph;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        ph = FindObjectOfType<PlayerHandler>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B) && GetComponentInChildren<Bomb>())
        {
            Debug.Log("Throwing Bomb");
            GetComponentInChildren<Bomb>().Throw(transform);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        GroundCheck = groundCheck();
        float j = handleJumping();
        
        
        rb.velocity = new Vector2(h * Speed, rb.velocity.y + j * Jump);

        anim.SetFloat("Velocity", Mathf.Abs(rb.velocity.y));
        anim.SetFloat("VelocityH", Mathf.Abs(rb.velocity.x));

        if (h > 0) transform.localScale = new Vector3(1, 1, 1);
        else if (h < 0) transform.localScale = new Vector3(-1, 1, 1);

        //for Nathan
        //anim.SetFloat("Velocity", rb.velocity.magnitude);
    }
    public bool jumping;
    public bool secondJumping ;
    public bool jumped;
    public bool releaseJump;
    private float handleJumping()
    {
        int j = Input.GetKey(KeyCode.Space) ? 1 : 0;

        //check for on jump down
        if (j > 0 && releaseJump) jumped = true; //OnKeyDown()
        else jumped = false;
        releaseJump = j > 0 ? false : true;

        
        if (rb.velocity.y > MaxJumpSpeed) {
            j = 0;
            jumping = false;
            //secondJumping = false;
        }
        if (GroundCheck)
        {
            ph.Inventory.ResetAccessory(AbilityItem.AbilityTypes.DoubleJump);
        }

        //single jump
        if (GroundCheck || jumping && !secondJumping)
        {
            if (j > 0 && !jumping)
            {
                jumping = true;
                return j;
            }
            //continue to accelerate while holding button
            else if (j > 0 && jumping)
            {
                return j;
            }

            else
            {
                jumping = false;
                return 0;
            }
        }
        //double jump
        else 
        {
            Debug.Log("Has Double Jump");
            if(j>0 && !secondJumping && !jumping && jumped && ph.Inventory.UseAccessory(AbilityItem.AbilityTypes.DoubleJump))
            {
                jumping = true;
                secondJumping = true;
                return j;
            }
            //continue to accelerate while holding button
            else if (j > 0 && jumping && secondJumping)
            {
                return j;
            }
            else
            {
                jumping = false;
                return 0;
            }
        }
       
        
        

    }

    private bool groundCheck()
    {
        
        bool grounded = false;
        //RaycastHit2D hit;
        //Ray2D ray = new Ray2D(transform.position, -transform.up * raycastLength);
        if(Physics2D.Raycast(transform.position, -transform.up, raycastLength, groundMask))
        {
            grounded = true;
            secondJumping = false;
        }
        return grounded;
    }
}
