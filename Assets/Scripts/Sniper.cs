// Sniper.cs
using UnityEngine;

public class Sniper : MonoBehaviour
{
    public Transform player;
    public float range = 50f;
    public float cooldown = 3f;
    public float knockbackForce = 5f;
    private float lastShotTime;

    void Update()
    {
        if (Time.time - lastShotTime >= cooldown)
        {
            Vector3 direction = player.position - transform.position;
            if (Physics.Raycast(transform.position, direction.normalized, out RaycastHit hit, range))
            {
                if (hit.collider.CompareTag("Player")) // проверка на тег
                {
                    KnockbackRelay relay = hit.collider.GetComponent<KnockbackRelay>();
                    if (relay != null)
                    {
                        relay.ApplyKnockback(direction, knockbackForce);
                        Debug.DrawRay(transform.position, direction.normalized * hit.distance, Color.red, 1f);
                        lastShotTime = Time.time;
                    }
                }
            }
        }
    }
}
