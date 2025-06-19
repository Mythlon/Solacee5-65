using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallRun : MonoBehaviour
{
    [Header("Wallrunning")]
    public LayerMask whatIsWall; 
    public LayerMask whatIsGround;
    public float wallRunForce;
    public float wallClimbSpeed;
    public float wallJumpUpForce;
    public float wallJumpSideForce;
    public float maxWallRunTime;
    private float wallRunTimer;


    [Header("Input")]

    public KeyCode upwardsRunKey = KeyCode.LeftShift;
    public KeyCode downWardsRunKey = KeyCode.LeftControl;
    private bool upwardsRunning;
    private bool downwardsRunning;
    public KeyCode jumpKey = KeyCode.Space;
    private float horizontalInput;
    private float verticalInput;


    [Header("Detection")]
    public float wallCheckDistance;
    public float minJumpHeight;
    private RaycastHit leftWallHit;
    private RaycastHit rightWallHit;
    private bool wallLeft;
    private bool wallRight;

    [Header("Exiting")]
    private bool exitingWall;
    public float exitWallTime;
    private float exitWallTimer;

    [Header("Gravity")]
    public bool useGravity;
    public float gravityCounterForce;


    [Header("Refs")]

    public Transform orientation;
    public PlayerCam cam;

    private PlayerMovement pm;

    private Ledges lg;

    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        pm = GetComponent<PlayerMovement>();
        lg = GetComponent<Ledges>();
    }

    private void CheckForWall()
    {
        wallRight = Physics.Raycast(transform.position, orientation.right, out rightWallHit, wallCheckDistance, whatIsWall);
        wallLeft = Physics.Raycast(transform.position, -orientation.right, out leftWallHit, wallCheckDistance, whatIsWall);
    }

    private void Update()
    {
        CheckForWall();
        StateMachine();
    }

    private void FixedUpdate()
    {  
        if(pm.wallrunning)
            WallRunningMovement();
        
    }

    private bool AboveGround()
    {
        return !Physics.Raycast(transform.position, Vector3.down, minJumpHeight, whatIsGround);
    }

    private void StateMachine()
    {   //Inputs
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        upwardsRunning = Input.GetKey(upwardsRunKey);
        downwardsRunning = Input.GetKey(downWardsRunKey);

        if((wallLeft||wallRight) && verticalInput > 0 && AboveGround() && !exitingWall)
        {
            if(!pm.wallrunning)
                StartWallRun();
            //wall run timer
            if (wallRunTimer > 0)
                wallRunTimer -= Time.deltaTime;
                
            
            if(wallRunTimer <= 0 && pm.wallrunning)
            {
                exitingWall = true;
                exitWallTimer = exitWallTime;

            if (exitingWall && (wallLeft || wallRight))  // This ensures the player doesn't instantly reattach
            {
                exitWallTimer = exitWallTime;
            }

                
            }



            //wall jump 
            if (Input.GetKeyDown(jumpKey)) WallJump();

            //start wallrun
        }
            //stop wallrun
        //state 2 - leaving wall
        else if (exitingWall)
        {
            if(pm.wallrunning)
                StopWallRun();

            if (exitWallTimer > 0)
                exitWallTimer -= Time.deltaTime;

            if(exitWallTimer <= 0)
                exitingWall = false;

        }

        //state 3 
        else 
        {
            if(pm.wallrunning)
                StopWallRun();

            

        }
    }


    private void StartWallRun()
    {   
        pm.wallrunning = true;

        wallRunTimer = maxWallRunTime;

        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        //camera effects
        cam.DoFov(90f);
        if (wallLeft) cam.DoTilt(-5f);
        if (wallRight) cam.DoTilt(5f);


    }

    private void WallRunningMovement()
    {

        rb.useGravity = useGravity;
        

        Vector3 wallNormal = wallRight ? rightWallHit.normal : leftWallHit.normal;

        Vector3 wallForward = Vector3.Cross(wallNormal, transform.up);

        if ((orientation.forward - wallForward).magnitude > (orientation.forward - -wallForward).magnitude)
            wallForward = -wallForward;

        // forward force 
        rb.AddForce(wallForward*wallRunForce, ForceMode.Force);


        //upwards & downwards force 

        bool nearWallTop = !Physics.Raycast(transform.position + Vector3.up * 0.5f, -wallNormal, 1f, whatIsWall);

        // Upwards & downwards force
        if (upwardsRunning && !nearWallTop) // Prevent going over the top
            rb.velocity = new Vector3(rb.velocity.x, wallClimbSpeed, rb.velocity.z);

        if (downwardsRunning)
            rb.velocity = new Vector3(rb.velocity.x, -wallClimbSpeed, rb.velocity.z);

        //push to wall force
        if (!(wallLeft && horizontalInput > 0) && !(wallRight && horizontalInput < 0))
            rb.AddForce(-wallNormal * 100, ForceMode.Force);

        if(useGravity)
            rb.AddForce(transform.up * gravityCounterForce, ForceMode.Force);

    }

    private void StopWallRun()
    {
        pm.wallrunning = false;

        //reset cam effects
        cam.DoFov(80f);
        cam.DoTilt(0f);



    }

    private Vector3 lastWallNormal = Vector3.zero; // Store the last wall normal
    private float minWallJumpAngle = 30f; // Minimum required angle difference

    private void WallJump()
    {

        if (lg.holding || lg.exitingLedge) return;

        Vector3 wallNormal = wallRight ? rightWallHit.normal : leftWallHit.normal;

    // Prevent jumping on the same wall by checking angle difference
        if (lastWallNormal != Vector3.zero) // Ensure there is a previous normal
        {
        float angleDifference = Vector3.Angle(lastWallNormal, wallNormal);

        if (angleDifference < minWallJumpAngle)
        {
            Debug.Log("Wall jump blocked: Too similar to the last wall.");
            return; // Stop jump if the walls are too similar
        }
        }
        //exit wall state
        exitingWall = true;
        exitWallTimer = exitWallTime;
        

        Vector3 forceToApply = transform.up * wallJumpUpForce + wallNormal * wallJumpSideForce;

        //apply force
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(forceToApply, ForceMode.Impulse);

        lastWallNormal = wallNormal;

    }


}
