using UnityEngine;

public class ChasingEnemy : MonoBehaviour
{
    public Transform player;
    public float detectionRange = 15f;
    public float attackRange = 2.5f;
    public float moveSpeed = 5f;
    public float knockbackForce = 8f;

    private bool playerInRange;

    void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= detectionRange)
        {
            playerInRange = true;
        }
        else
        {
            playerInRange = false;
        }

        if (playerInRange)
        {
            ChasePlayer();
        }
    }

    void ChasePlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0; // чтобы не взлетал

        transform.position += direction * moveSpeed * Time.deltaTime;
        transform.rotation = Quaternion.LookRotation(direction);

        float distance = Vector3.Distance(transform.position, player.position);
        if (distance <= attackRange)
        {
            TryAttack();
        }
    }
    void TryAttack()
    {
        KnockbackReceiver receiver = player.GetComponent<KnockbackReceiver>();
        if (receiver != null)
        {
            Vector3 knockDir = (player.position - transform.position);
            knockDir.y = 0; // убрать вертикальный перекос
            knockDir = knockDir.normalized;

                // Добавим чуть вверх, если нужно
            knockDir += Vector3.up * 0.10f;

            receiver.ApplyKnockback(knockDir, knockbackForce);
        }
    }


}
