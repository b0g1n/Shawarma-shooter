using UnityEngine;

public class Medkit : MonoBehaviour
{
    [Header("Heal Settings")]
    public float rotationSpeed = 100f; // Spin speed on Y axis

    [Header("Floating Settings")]
    public float floatAmplitude = 0.25f; // How high it floats up/down
    public float floatFrequency = 1f;    // How fast it floats

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position; // Save starting position
    }

    void Update()
    {
        // Spin the medkit
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime, Space.World);

        // Floating effect
        float newY = startPos.y + Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.currentHealth = playerHealth.maxHealth; // Full heal
            }

            Destroy(gameObject); // Destroy the medkit
        }
    }
}
