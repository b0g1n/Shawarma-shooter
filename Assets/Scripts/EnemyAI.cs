using UnityEngine;

public class EnemyAIAdvanced : MonoBehaviour
{
    [Header("Enemy Stats")]
    public float chaseRange = 10f;       // Start chasing when player is this close
    public float attackRange = 2f;       // Stop moving and attack when this close
    public float moveSpeed = 3f;
    public int damage = 10;
    public float attackCooldown = 1f;

    private Transform player;
    private float lastAttackTime;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        // Rotate to face player if within chase range
        if (distance <= chaseRange)
        {
            FacePlayer();
        }

        // Chase if player in range but not in attack range
        if (distance <= chaseRange && distance > attackRange)
        {
            MoveTowardsPlayer();
        }
        // Attack if player in attack range
        else if (distance <= attackRange)
        {
            TryAttack();
        }
        // Otherwise do nothing
    }

    void MoveTowardsPlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        transform.position += direction * moveSpeed * Time.deltaTime;
    }

    void TryAttack()
    {
        if (Time.time - lastAttackTime >= attackCooldown)
        {
            PlayerHealth hp = player.GetComponent<PlayerHealth>();
            if (hp != null)
            {
                hp.TakeDamage(damage);
            }
            lastAttackTime = Time.time;
        }
    }

    void FacePlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0; // prevent tilting up/down

        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation, Time.deltaTime * 10f);
        }
    }

    void OnDrawGizmosSelected()
    {
        // Visualize ranges in editor
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
