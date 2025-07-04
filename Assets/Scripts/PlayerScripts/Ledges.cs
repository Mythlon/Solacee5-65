using UnityEngine;

public class Ledges : MonoBehaviour
{
    [Header("Refs")]
    public PlayerMovement pm;
    public Transform orientation;
    public Transform cam;
    public Rigidbody rb;

    [Header("Grabbing")]
    public float moveToLedgeSpeed;
    public float maxLedgeGrabDistance;

    public float minTimeOnLedge;
    private float timeOnLedge;

    public bool holding;

    [Header("Ledge Detection")]
    public float ledgeDetectionLength;
    public float ledgeSphereCastRadius;
    public LayerMask whatIsLedge;

    private Transform lastLedge;
    private Transform currLedge;

    private RaycastHit ledgeHit;

    [Header("Ledge Jumping")]
    public KeyCode jumpKey = KeyCode.Space;
    public float ledgeJumpForwardForce;
    public float ledgeJumpUpwardForce;

    [Header("Exit")]
    public bool exitingLedge;
    public float exitLedgeTime;
    private float exitLedgeTimer;

    private void SubStateMachine()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");
        bool anyInputKeyPressed = horizontalInput != 0 || verticalInput != 0;

        //holding onto ledge

        if (holding)
        {
            FreezeRigidBodyOnLedge();

            timeOnLedge += Time.deltaTime;

            if(timeOnLedge > minTimeOnLedge && anyInputKeyPressed) ExitLedgeHold();

            if (Input.GetKeyDown(jumpKey)) LedgeJump();
        }

        //exit ledge
        else if (exitingLedge)
        {
            if(exitLedgeTime > 0) exitLedgeTimer -= Time.deltaTime;
            else exitingLedge = false;

        }
    }

    private void Update()
    {
        LedgeDetection();
        SubStateMachine();
    }
    private void LedgeDetection()
    {
        
        bool ledgeDetected = Physics.SphereCast(transform.position, ledgeSphereCastRadius, cam.forward, out ledgeHit, ledgeDetectionLength, whatIsLedge);

        if (!ledgeDetected) return;

        float distanceToLedge = Vector3. Distance(transform.position, ledgeHit.transform.position);

        if (ledgeHit.transform == lastLedge) return;

        if (distanceToLedge < maxLedgeGrabDistance && !holding) EnterLedgeHold();
    }

    private void LedgeJump()
    {
        ExitLedgeHold();
        Invoke(nameof(DelayedJumpForce), 0.05f);

    }

    private void DelayedJumpForce()
    {
        Vector3 forceToAdd = cam.forward * ledgeJumpForwardForce + orientation.up * ledgeJumpUpwardForce;
        rb.velocity = Vector3.zero;
        rb.AddForce(forceToAdd, ForceMode.Impulse);
    }

    private void EnterLedgeHold()
    {
        holding = true;
        pm.unlimited = true;
        pm.restricted = true;

        currLedge = ledgeHit.transform;
        lastLedge = ledgeHit.transform;

        rb.useGravity = false;
        rb.velocity = Vector3.zero;


    }

    private void FreezeRigidBodyOnLedge()
    {
        rb.useGravity = false; 

        Vector3 directionToLedge = currLedge.position - transform.position;
        float distanceToLedge = Vector3.Distance(transform.position, currLedge.position);

        //move player to ledge
        if(distanceToLedge > 1f)
        {
            if(rb.velocity.magnitude < moveToLedgeSpeed)
                rb.AddForce(directionToLedge.normalized * moveToLedgeSpeed * 1000f * Time.deltaTime);
        }

        else
        {
            if(!pm.freeze) pm.freeze = true;
            if (pm.unlimited) pm.unlimited = false; 
        }

        if(distanceToLedge > maxLedgeGrabDistance) ExitLedgeHold();




    }

    private void ExitLedgeHold()
    {
        exitingLedge = true; 
        exitLedgeTimer = exitLedgeTime;
        
        holding = false;
        timeOnLedge = 0f;

        pm.restricted = false;
        pm.freeze = false;

        rb.useGravity = true;

        StopAllCoroutines();
        Invoke(nameof(ResetLastLedge), 1f);
        
        


    }

    private void ResetLastLedge()
    {
        lastLedge = null; 
    }

}
