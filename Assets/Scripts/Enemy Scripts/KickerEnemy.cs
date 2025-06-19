using UnityEngine;
using UnityEngine.AI;

public class KickerEnemy : MonoBehaviour
{
    public Transform player;
    public float detectionRange = 15f;
    public float knockbackForce = 8f;

    private NavMeshAgent agent;
    private bool playerInRange;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        playerInRange = distance <= detectionRange;

        if (playerInRange)
        {
            agent.SetDestination(player.position);

            // Атака, если рядом
            if (distance <= agent.stoppingDistance + 0.5f)
            {
                TryAttack();
            }
        }
        else
        {
            agent.ResetPath(); // не преследует
        }
    }

    void TryAttack()
    {
        KnockbackReceiver receiver = player.GetComponent<KnockbackReceiver>();
        if (receiver != null)
        {
            Vector3 knockDir = (player.position - transform.position);
            knockDir.y = 0;

            if (knockDir == Vector3.zero)
                knockDir = transform.forward;

            knockDir.Normalize();
            knockDir += Vector3.up * 0.2f;

            receiver.ApplyKnockback(knockDir, knockbackForce);
        }
    }
}
