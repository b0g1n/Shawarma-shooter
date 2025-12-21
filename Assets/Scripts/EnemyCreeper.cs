using UnityEngine;
using System.Collections.Generic;

public class EnemyCreeper : MonoBehaviour
{
    [Header("Enemy Stats")]
    public float chaseRange = 10000f;
    public float moveSpeed = 3f;
    public int explosionDamage = 20;
    public float explosionRadius = 4f;

    [Header("Rewards")]
    public int cashReward = 0; // creeper gives no cash

    [Header("Explosion Effects")]
    public GameObject explosionVFX;
    public AudioClip explosionSFX;

    private Transform player;
    private bool hasExploded;
    private AudioSource audioSource;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (player == null || hasExploded) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= chaseRange)
        {
            FacePlayer();
            MoveTowardsPlayer();
        }
    }

    void MoveTowardsPlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        transform.position += direction * moveSpeed * Time.deltaTime;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (hasExploded) return;

        if (collision.gameObject.CompareTag("Player"))
        {
            Explode();
        }
    }

    void Explode()
{
    if (hasExploded) return; // double safety
    hasExploded = true;

    // Count the kill, no cash
    if (GameManager.instance != null)
    {
        GameManager.instance.EnemyKilled(cashReward);
    }

    if (explosionVFX != null)
    {
        Instantiate(explosionVFX, transform.position, Quaternion.identity);
    }

    if (explosionSFX != null)
    {
        AudioSource.PlayClipAtPoint(explosionSFX, transform.position);
    }

    // Damage players in radius
    Collider[] hits = Physics.OverlapSphere(transform.position, explosionRadius);
    HashSet<PlayerHealth> damagedPlayers = new HashSet<PlayerHealth>();

    foreach (Collider hit in hits)
    {
        if (hit.CompareTag("Player"))
        {
            PlayerHealth hp = hit.GetComponent<PlayerHealth>();
            if (hp != null && !damagedPlayers.Contains(hp))
            {
                hp.TakeDamage(explosionDamage);
                damagedPlayers.Add(hp); // make sure we only damage once
            }
        }
    }

    Destroy(gameObject);
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
}
