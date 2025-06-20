using UnityEngine;
using UnityEngine.AI;

public class KickerEnemy : MonoBehaviour
{
    public Transform player;
    public float detectionRange = 15f;
    public float knockbackForce = 8f;

    private NavMeshAgent agent;
    private bool playerInRange;

    private Animator animator;

    public float roamRadius = 10f;
    public float roamInterval = 5f;

    private float roamTimer;
    private Vector3 startPosition;



    void Start()
    {

        startPosition = transform.position;
        roamTimer = roamInterval;
        animator = GetComponent<Animator>();

        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        float speed = agent.velocity.magnitude;
        animator.SetFloat("Speed", speed); // это будет управлять Idle ↔ Walk

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
            Debug.Log("Roaming...");
            roamTimer -= Time.deltaTime;

            if (!agent.hasPath || agent.remainingDistance < 0.5f)
            {
                if (roamTimer <= 0f)
                {
                    Vector3 roamPos = GetRandomPoint(startPosition, roamRadius);
                    Debug.Log("Moving to: " + roamPos);
                    agent.SetDestination(roamPos);
                    roamTimer = roamInterval;
                }
            }
        }
    }


    void TryAttack()
    {
        KnockbackReceiver receiver = player.GetComponent<KnockbackReceiver>();
        if (receiver != null)
        {
            Vector3 knockDir = (player.position - transform.position);
            knockDir.y = 0;

            animator.SetTrigger("Attack"); // ⚠️ НЕ Play!

            if (knockDir == Vector3.zero)
                knockDir = transform.forward;

            knockDir.Normalize();
            knockDir += Vector3.up * 0.2f;

            receiver.ApplyKnockback(knockDir, knockbackForce);
        }
    }
    
    Vector3 GetRandomPoint(Vector3 center, float radius)
    {
        for (int i = 0; i < 30; i++)
        {
            Vector3 randomPos = center + Random.insideUnitSphere * radius;
            randomPos.y = center.y;

            if (NavMesh.SamplePosition(randomPos, out NavMeshHit hit, 2f, NavMesh.AllAreas))
            {
                return hit.position;
            }
        }

        return center; // fallback
    }


}
