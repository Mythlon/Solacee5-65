using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BloodTrail : MonoBehaviour
{
    public GameObject bloodPuddlePrefab;
    public float spawnInterval = 0.2f;
    public float puddleLifetime = 3f;

    private float timer;

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            SpawnPuddle();
            timer = 0f;
        }
    }

   void SpawnPuddle()
    {
        RaycastHit hit;
        Vector3 rayOrigin = transform.position;
        Vector3 rayDirection = -transform.up;

        if (Physics.Raycast(rayOrigin, rayDirection, out hit, 2f))
        {
            Quaternion puddleRotation = Quaternion.FromToRotation(Vector3.up, hit.normal);

            GameObject puddle = Instantiate(bloodPuddlePrefab, hit.point + hit.normal * 0.01f, puddleRotation);
            Destroy(puddle, puddleLifetime);
        }
    }
}
