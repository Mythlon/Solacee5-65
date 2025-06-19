using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grappling : MonoBehaviour
{
    [Header("References")]
    private PlayerMovement pm;
    public Transform cam;
    public Transform gunTip;
    public LayerMask whatIsGrappable;

    public LineRenderer lr;

    private Vector3 currentGrapplePosition;

    private LineRenderer lineRenderer;



    [Header("Grappling")]
    public float maxGrappleDistance;
    public float grappleDelayTime;
    public float OvershootYAxis;
    private Vector3 grapplePoint;

    [Header("Grapple Attempts")]
    public int maxAttemptsBeforeLanding = 1;
    private int remainingAttempts;


    [Header("Prediction")]
    public Transform predictionPoint;
    public float predictionSphereCastRadius = 1.0f;
    private RaycastHit predictionHit;


    [Header("Cooldown")]
    public float grapplingCd;
    private float grapplingCdTimer;

    [Header("Input")]
    public KeyCode grappleKey = KeyCode.Mouse1;
    private bool grappling;

    private void Start()
    {
        pm = GetComponent<PlayerMovement>();
        lineRenderer = GetComponent<LineRenderer>();
        remainingAttempts = maxAttemptsBeforeLanding;
    }

    private void Update()
    {
        if (Input.GetKeyDown(grappleKey) && remainingAttempts > 0)
        {
            StartGrapple();
            remainingAttempts--;
        }

        if (pm != null && pm.IsGrounded())
        {
            remainingAttempts = maxAttemptsBeforeLanding;
        }
        if (grapplingCdTimer > 0)
            grapplingCdTimer -= Time.deltaTime;

        CheckForGrapplePoint();
        UpdateGrappleRope();

    }

    private void LateUpdate()
    {
        if (grappling)
            lr.SetPosition(0, gunTip.position);

    }

    private void StartGrapple()
    {
        if (grapplingCdTimer > 0) return;

        //deactive active swinging
        GetComponent<PrimaryGrapple>().StopSwing();



        grappling = true;

        pm.freeze = true;

        if (predictionHit.point != Vector3.zero)
        {
            grapplePoint = predictionHit.point;
            currentGrapplePosition = gunTip.position;

            Invoke(nameof(ExecuteGrapple), grappleDelayTime);
        }
        else
        {
            grapplePoint = cam.position + cam.forward * maxGrappleDistance;

            Invoke(nameof(StopGrapple), grappleDelayTime);
        }

        lr.enabled = true;
        lr.SetPosition(1, grapplePoint);
    }

    private void ExecuteGrapple()
    {
        pm.freeze = false;

        Vector3 lowestPoint = new Vector3(transform.position.x, transform.position.y - 1f, transform.position.z);
        float grapplePointRelativeYPos = grapplePoint.y - lowestPoint.y;
        float highestPointOnArc = grapplePointRelativeYPos + OvershootYAxis;

        if (grapplePointRelativeYPos < 0) highestPointOnArc = OvershootYAxis;

        pm.JumpToPosition(grapplePoint, highestPointOnArc);

        Invoke(nameof(StopGrapple), 1f);

    }

    public void StopGrapple()
    {
        pm.freeze = false;

        grappling = false;

        grapplingCdTimer = grapplingCd;

        lr.enabled = false;

        currentGrapplePosition = Vector3.zero;
    }

    private void CheckForGrapplePoint()
    {
        RaycastHit sphereCastHit;
        Physics.SphereCast(cam.position, predictionSphereCastRadius, cam.forward,
                        out sphereCastHit, maxGrappleDistance, whatIsGrappable);

        RaycastHit raycastHit;
        Physics.Raycast(cam.position, cam.forward,
                        out raycastHit, maxGrappleDistance, whatIsGrappable);

        Vector3 realHitPoint;

        if (raycastHit.point != Vector3.zero)
            realHitPoint = raycastHit.point;
        else if (sphereCastHit.point != Vector3.zero)
            realHitPoint = sphereCastHit.point;
        else
            realHitPoint = Vector3.zero;

        if (realHitPoint != Vector3.zero)
        {
            predictionPoint.gameObject.SetActive(true);
            predictionPoint.position = realHitPoint;
        }
        else
        {
            predictionPoint.gameObject.SetActive(false);
        }

        predictionHit = raycastHit.point == Vector3.zero ? sphereCastHit : raycastHit;
    }
    
    private void UpdateGrappleRope()
{
    if (!grappling)
    {
        lineRenderer.enabled = false;
        return;
    }

    lineRenderer.enabled = true;

    // Плавное приближение троса к точке захвата
    currentGrapplePosition = Vector3.Lerp(currentGrapplePosition, grapplePoint, Time.deltaTime * 20f);
    lineRenderer.SetPosition(0, gunTip.position);
    lineRenderer.SetPosition(1, currentGrapplePosition);
}


    
}
