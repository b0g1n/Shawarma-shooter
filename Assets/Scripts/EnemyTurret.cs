using UnityEngine;

public class EnemyTurret : MonoBehaviour
{
    [Header("Enemy Stats")]
    public float chaseRange = 10f;
    public float attackRange = 8f;
    public float moveSpeed = 3f;
    public float attackCooldown = 1f;
    public GameObject projectilePrefab;
    public Transform shootPoint;

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

        if (distance <= chaseRange)
        {
            FacePlayer();
        }

        if (distance > attackRange && distance <= chaseRange)
        {
            MoveTowardsPlayer();
        }

        if (distance <= attackRange)
        {
            TryShoot();
            MoveTowardsPlayer();
        }
    }

    void MoveTowardsPlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0;
        transform.position += direction * moveSpeed * Time.deltaTime;
    }

    void FacePlayer()
    {
        Vector3 direction = (player.position - transform.position);
        direction.y = 0;

        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation, Time.deltaTime * 10f);
        }
    }

    void TryShoot()
    {
        if (Time.time - lastAttackTime >= attackCooldown)
        {
            if (projectilePrefab != null && shootPoint != null)
            {
                GameObject proj = Instantiate(
                    projectilePrefab,
                    shootPoint.position,
                    Quaternion.identity
                );

                Projectile projectile = proj.GetComponent<Projectile>();
                if (projectile != null)
                {
                    projectile.SetTarget(player.position);
                }
            }

            lastAttackTime = Time.time;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
