using UnityEngine;
using System.Collections;

public class Boss1ai : MonoBehaviour
{
    [Header("Enemy Stats")]
    public float chaseRange = 15f;
    public float attackRange = 2f;
    public float normalMoveSpeed = 3f;
    public float chargeMoveSpeed = 20f;
    public int damage = 10;
    public float attackCooldown = 1f;

    [Header("Charge Timings")]
    public float stopBeforeChargeTime = 1f;
    public float chargeTime = 1f;
    public float minNormalTime = 10f;
    public float maxNormalTime = 15f;

    private Transform player;
    private float lastAttackTime;
    private float currentMoveSpeed;
    private bool canMove = true;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        currentMoveSpeed = normalMoveSpeed;
        StartCoroutine(BossBehaviorLoop());
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= chaseRange)
        {
            FacePlayer();
        }

        if (!canMove) return;

        if (distance <= chaseRange && distance > attackRange)
        {
            MoveTowardsPlayer();
        }
        else if (distance <= attackRange)
        {
            TryAttack();
        }
    }

    IEnumerator BossBehaviorLoop()
    {
        while (true)
        {
            // Stop before charge
            canMove = false;
            yield return new WaitForSeconds(stopBeforeChargeTime);

            // Charge
            canMove = true;
            currentMoveSpeed = chargeMoveSpeed;
            yield return new WaitForSeconds(chargeTime);

            // Back to normal movement
            currentMoveSpeed = normalMoveSpeed;
            float normalDuration = Random.Range(minNormalTime, maxNormalTime);
            yield return new WaitForSeconds(normalDuration);
        }
    }

    void MoveTowardsPlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        transform.position += direction * currentMoveSpeed * Time.deltaTime;
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
        direction.y = 0;

        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation, Time.deltaTime * 10f);
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
