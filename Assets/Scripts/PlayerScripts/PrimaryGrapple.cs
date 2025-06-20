using System.Collections;
using System.Collections.Generic;
using Unity.PlasticSCM.Editor.WebApi;
using UnityEngine;

public class PrimaryGrapple : MonoBehaviour
{

    [Header("Input")]
    public KeyCode swingKey = KeyCode.Mouse0;

    [Header("Grapple Attempts")]
    public int maxAttemptsBeforeLanding = 1;
    private int remainingAttempts;

    [Header("References")]
    public LineRenderer lr;
    public Transform gunTip, cam, player;
    public LayerMask whatIsGrappable;
    public PlayerMovement pm;
    [Header("Swinging")]
    private float maxSwingDistance = 25f;
    private Vector3 swingPoint;
    private SpringJoint joint;

    [Header("OdmGear")]
    public Transform orientation;
    public Rigidbody rb;
    public float horizontalThrustForce;
    public float forwardThrustForce;
    public float extendCableSpeed;

    [Header("Prediction")]
    public RaycastHit predictionHit;
    public float predictionSphereCastRadius;
    public Transform predictionPoint;

    [Header("Rope Shortening")]
    public float shrinkSpeed = 5f;
    public float minClamp = 2f;

    public GrappleUIController ui;

    private bool wasGroundedLastFrame;

    [SerializeField] private float maxGrappleDistance = 40f;
    
    [SerializeField] private AudioClip grappleSound;
    [SerializeField] private AudioClip noChargeSound;
    private AudioSource audioSource;





    private void Update()
    {
        CheckForGrapplePoint(); // 🔴 Добавь в начало

        if (Input.GetKeyDown(swingKey))
        {
            if (remainingAttempts > 0)
            {
                if (!CanSwing()) return;

                StartSwing();
                remainingAttempts--;
                ui.UpdatePrimaryState(remainingAttempts);

                if (grappleSound != null)
                    audioSource.PlayOneShot(grappleSound);
            }
            else
            {
                if (noChargeSound != null)
                    audioSource.PlayOneShot(noChargeSound);
            }
        }

        if (Input.GetKeyUp(swingKey))
        {
            StopSwing();
        }

        bool groundedNow = pm != null && pm.IsGrounded();

        if (groundedNow && !wasGroundedLastFrame)
        {
            remainingAttempts = maxAttemptsBeforeLanding;
            ui.UpdatePrimaryState(remainingAttempts);
        }

        wasGroundedLastFrame = groundedNow;

        DrawRope();
    }

    private bool CanSwing()
    {
        return predictionHit.point != Vector3.zero;
    }



    private void Start()
    {
        remainingAttempts = maxAttemptsBeforeLanding;
        ui.UpdatePrimaryState(remainingAttempts);
        audioSource = GetComponent<AudioSource>();
    }

    void StartSwing()
    {
        // return if predictionHit not found
        if (predictionHit.point == Vector3.zero) return;

        // deactivate active grapple
        if (GetComponent<Grappling>() != null)
            GetComponent<Grappling>().StopGrapple();
        pm.ResetRestrictions();

        pm.swinging = true;

        swingPoint = predictionHit.point;
        joint = player.gameObject.AddComponent<SpringJoint>();
        joint.autoConfigureConnectedAnchor = false;
        joint.connectedAnchor = swingPoint;

        float distanceFromPoint = Vector3.Distance(player.position, swingPoint);

        // the distance grapple will try to keep from grapple point. 
        joint.maxDistance = distanceFromPoint * 0.8f;
        joint.minDistance = distanceFromPoint * 0.25f;

        // customize values as you like
        joint.spring = 4.5f;
        joint.damper = 7f;
        joint.massScale = 4.5f;

        lr.positionCount = 2;
        currentGrapplePosition = gunTip.position;

    }

    private void OdmGearMovement()
    {
        // right
        if (Input.GetKey(KeyCode.D)) rb.AddForce(orientation.right * horizontalThrustForce * Time.deltaTime);
        // left
        if (Input.GetKey(KeyCode.A)) rb.AddForce(-orientation.right * horizontalThrustForce * Time.deltaTime);

        // forward
        if (Input.GetKey(KeyCode.W)) rb.AddForce(orientation.forward * horizontalThrustForce * Time.deltaTime);

        // shorten cable
        if (Input.GetKey(KeyCode.Space))
        {
            Vector3 directionToPoint = swingPoint - transform.position;

            // Optional very small pull force to feel more natural
            rb.AddForce(directionToPoint.normalized * (forwardThrustForce * 0.1f) * Time.deltaTime);

            // Controlled rope shrinking
            joint.maxDistance = Mathf.Max(joint.maxDistance - shrinkSpeed * Time.deltaTime, minClamp);
            joint.minDistance = joint.maxDistance * 0.25f;

        }
        // extend cable
        if (Input.GetKey(KeyCode.S))
        {
            float extendedDistanceFromPoint = Vector3.Distance(transform.position, swingPoint) + extendCableSpeed;

            joint.maxDistance = extendedDistanceFromPoint * 0.8f;
            joint.minDistance = extendedDistanceFromPoint * 0.25f;
        }
    }

    private void LateUpdate()
    {
        DrawRope();
    }
    private void CheckForSwingPoints()
    {
        if (joint != null) return;

        RaycastHit sphereCastHit;
        Physics.SphereCast(cam.position, predictionSphereCastRadius, cam.forward,
                            out sphereCastHit, maxSwingDistance, whatIsGrappable);

        RaycastHit raycastHit;
        Physics.Raycast(cam.position, cam.forward,
                            out raycastHit, maxSwingDistance, whatIsGrappable);

        Vector3 realHitPoint;

        // Option 1 - Direct Hit
        if (raycastHit.point != Vector3.zero)
            realHitPoint = raycastHit.point;

        // Option 2 - Indirect (predicted) Hit
        else if (sphereCastHit.point != Vector3.zero)
            realHitPoint = sphereCastHit.point;

        // Option 3 - Miss
        else
            realHitPoint = Vector3.zero;

        // realHitPoint found
        if (realHitPoint != Vector3.zero)
        {
            predictionPoint.gameObject.SetActive(true);
            predictionPoint.position = realHitPoint;
        }
        // realHitPoint not found
        else
        {
            predictionPoint.gameObject.SetActive(false);
        }

        predictionHit = raycastHit.point == Vector3.zero ? sphereCastHit : raycastHit;
    }

    public void StopSwing()
    {
        pm.swinging = false;
        lr.positionCount = 0;
        Destroy(joint);
    }
    public bool IsGrappling()
    {
        return joint != null;
    }

    public Vector3 GetGrapplePoint()
    {
        return swingPoint;
    }

    private Vector3 currentGrapplePosition;

    private void DrawRope()
    {
        // if not grappling, don't draw rope
        if (!joint) return;

        currentGrapplePosition =
            Vector3.Lerp(currentGrapplePosition, swingPoint, Time.deltaTime * 8f);

        lr.SetPosition(0, gunTip.position);
        lr.SetPosition(1, currentGrapplePosition);
    }
    
    private void CheckForGrapplePoint()
    {
        RaycastHit hit;
        bool hasHit = Physics.SphereCast(cam.position, predictionSphereCastRadius, cam.forward, out hit, maxGrappleDistance, whatIsGrappable);

        predictionHit = hit;

        if (hasHit)
        {
            predictionPoint.gameObject.SetActive(true);
            predictionPoint.position = hit.point;
        }
        else
        {
            predictionPoint.gameObject.SetActive(false);
            predictionPoint.position = cam.position + cam.forward * maxGrappleDistance;
        }
    }

}
