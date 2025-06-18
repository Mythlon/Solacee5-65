// KnockbackReceiver.cs
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class KnockbackReceiver : MonoBehaviour
{
    public float knockbackMultiplier = 10f;
    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void ApplyKnockback(Vector3 direction, float force)
    {
        rb.velocity = Vector3.zero;
        Vector3 knockDir = direction.normalized + Vector3.up * 0.3f;
        rb.AddForce(knockDir * force * knockbackMultiplier, ForceMode.Impulse);

        PlayerMovement movement = GetComponent<PlayerMovement>();
        if (movement != null)
        {
            movement.ApplyMovementBlock(0.9f); // Примерно 0.6 секунд отключения управления
        }
    }

}
