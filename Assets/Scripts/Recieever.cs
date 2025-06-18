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
        Vector3 knockDir = direction.normalized + Vector3.up * 0.1f; // ← добавим вертикальный толчок
        rb.AddForce(knockDir.normalized * force * knockbackMultiplier, ForceMode.Impulse);
    }
}
