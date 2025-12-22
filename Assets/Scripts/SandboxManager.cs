using UnityEngine;

public class SandboxSpawner : MonoBehaviour
{
    [Header("Enemies")]
    public GameObject[] enemyPrefabs; // index 0–5 mapped to keys 1–6
    public Transform[] spawnPoints;

    [Header("Cheats")]
    public bool godMode = false;
    public bool infiniteAmmo = false;
    public bool infiniteMoney = false;

    private PlayerHealth playerHealth;
    private Gun playerWeapon;
    private GameManager gameManager;

    void Start()
    {
        playerHealth = Object.FindFirstObjectByType<PlayerHealth>();
        playerWeapon = Object.FindFirstObjectByType<Gun>();
        gameManager = GameManager.instance;
    }

    void Update()
    {
        // ---- Enemy Spawning ----
        if (Input.GetKeyDown(KeyCode.Alpha1)) SpawnEnemy(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) SpawnEnemy(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) SpawnEnemy(2);
        if (Input.GetKeyDown(KeyCode.Alpha4)) SpawnEnemy(3);
        if (Input.GetKeyDown(KeyCode.Alpha5)) SpawnEnemy(4);
        if (Input.GetKeyDown(KeyCode.Alpha6)) SpawnEnemy(5);
        if (Input.GetKeyDown(KeyCode.Alpha7)) SpawnEnemy(6);
        // ---- Cheats ----
        if (Input.GetKeyDown(KeyCode.G)) ToggleGodMode();
        if (Input.GetKeyDown(KeyCode.H)) ToggleInfiniteAmmo();
    }

    void SpawnEnemy(int index)
    {
        if (index < 0 || index >= enemyPrefabs.Length) return;

        int spawnIndex = Random.Range(0, spawnPoints.Length);

        Instantiate(
            enemyPrefabs[index],
            spawnPoints[spawnIndex].position,
            Quaternion.identity
        );
    }

    void ToggleGodMode()
    {
        godMode = !godMode;

        if (playerHealth != null)
            playerHealth.invincible = godMode;
    }

    void ToggleInfiniteAmmo()
    {
        infiniteAmmo = !infiniteAmmo;

        if (playerWeapon != null)
            playerWeapon.infiniteAmmo = infiniteAmmo;
    }
}
