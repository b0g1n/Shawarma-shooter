using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;
    public bool invincible = false;
    private int _currentHealth;
    public int currentHealth
    {
        get { return _currentHealth; }
        set
        {
            _currentHealth = Mathf.Clamp(value, 0, maxHealth); // Clamp between 0 and maxHealth
            UpdateHealthText();

            if (_currentHealth <= 0)
                Die();
        }
    }

    public TMP_Text healthText;

    private Color originalColor;
    private bool isFlashing = false;

    void Start()
    {
        originalColor = healthText.color;
        currentHealth = maxHealth; // Automatically updates UI via property
    }

    public void TakeDamage(int damage)
    {
        if (invincible) return;
        currentHealth -= damage;  // UI updates automatically
        StartCoroutine(FlashRed());
    }

    void UpdateHealthText()
    {
        if (healthText != null)
            healthText.text = currentHealth.ToString();
    }

    System.Collections.IEnumerator FlashRed()
    {
        if (isFlashing) yield break;

        isFlashing = true;
        healthText.color = Color.red;
        yield return new WaitForSeconds(0.3f);
        healthText.color = originalColor;
        isFlashing = false;
    }

    void Die()
    {
        Debug.Log("Player died");
        SceneManager.LoadScene("Gameoverscreen");
    }
}
