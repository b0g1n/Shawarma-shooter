using UnityEngine;
using TMPro;

public class Target : MonoBehaviour
{
    [Header("Health")]
    public int maxHealth = 100;
    public int currentHealth;
    public int cashReward = 50;
    private bool isDead = false;

    [Header("UI")]
    public TMP_Text healthText; // Drag your TMP object inside the enemy model here

    [Header("Drops")]
    public GameObject medkitPrefab; // Drag your medkit prefab here
    [Range(0f, 1f)]
    public float medkitDropChance = 0.1f; // 1 in 10 chance

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthText();
    }

    void Update()
    {
        // Make the text always face the camera
        if (healthText != null && Camera.main != null)
        {
            healthText.transform.rotation = Quaternion.LookRotation(healthText.transform.position - Camera.main.transform.position);
        }
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        currentHealth -= Mathf.RoundToInt(damage);
        if (currentHealth < 0) currentHealth = 0;

        UpdateHealthText();

            if (currentHealth <= 0)
            {
                EnemyDied();
            }
    }


    void UpdateHealthText()
    {
        if (healthText != null)
        {
            healthText.text = currentHealth + " / " + maxHealth + " HP";
        }
    }

    void EnemyDied()
    {
    if (isDead) return;
    isDead = true;

    // Drop medkit
    if (medkitPrefab != null && Random.value <= medkitDropChance)
    {
        Vector3 spawnPos = transform.position + Vector3.up * 1.0f;
        Instantiate(medkitPrefab, spawnPos, Quaternion.identity);
    }

    if (healthText != null)
        Destroy(healthText.gameObject);

    GameManager.instance.EnemyKilled(cashReward);
    Destroy(gameObject);
    }


}
