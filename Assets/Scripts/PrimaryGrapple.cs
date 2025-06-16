using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrimaryGrapple : MonoBehaviour
{

    [Header("Input")]
    public KeyCode swingKey = KeyCode.Mouse0;

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

    void Update()
    {
        if (Input.GetKeyDown(swingKey)) StartSwing();
        if (Input.GetKeyUp(swingKey)) StopSwing();
        CheckForSwingPoints();
        if (joint != null) OdmGearMovement();
    }

    void StartSwing()
    {
        GetComponent<Grappling>().StopGrapple();
        pm.ResetRestrictions();
        pm.swinging = true;

        RaycastHit hit;
        if (Physics.Raycast(cam.position, cam.forward, out hit, maxSwingDistance, whatIsGrappable))
        {
            swingPoint = hit.point;
            joint = player.gameObject.AddComponent<SpringJoint>();
            joint.autoConfigureConnectedAnchor = false;
            joint.connectedAnchor = swingPoint;

            float distanceFromPoint = Vector3.Distance(player.position, swingPoint);

            //The distance grapple will try to keep from grapple point. 
            joint.maxDistance = distanceFromPoint * 0.8f;
            joint.minDistance = distanceFromPoint * 0.25f;

            //Adjust these values to fit your game.
            joint.spring = 4.5f;
            joint.damper = 7f;
            joint.massScale = 4.5f;
        }
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
            rb.AddForce(directionToPoint.normalized * forwardThrustForce * Time.deltaTime);

            float distanceFromPoint = Vector3.Distance(transform.position, swingPoint);

            joint.maxDistance = distanceFromPoint * 0.8f;
            joint.minDistance = distanceFromPoint * 0.25f;
        }
        // extend cable
        if (Input.GetKey(KeyCode.S))
        {
            float extendedDistanceFromPoint = Vector3.Distance(transform.position, swingPoint) + extendCableSpeed;

            joint.maxDistance = extendedDistanceFromPoint * 0.8f;
            joint.minDistance = extendedDistanceFromPoint * 0.25f;
        }
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
    public bool IsGrappling() {
        return joint != null;
    }

    public Vector3 GetGrapplePoint() {
        return swingPoint;
    }

}
