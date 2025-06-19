// KnockbackRelay.cs
using UnityEngine;

public class KnockbackRelay : MonoBehaviour
{
    public KnockbackReceiver receiver; // Сюда в инспекторе перетащи родителя

    public void ApplyKnockback(Vector3 direction, float force)
    {
        if (receiver != null)
        {
            receiver.ApplyKnockback(direction, force);
        }
    }
}
