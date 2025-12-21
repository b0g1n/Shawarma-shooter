using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public GameObject[] enemyPrefabs;
    public GameObject bossPrefab;
    public Transform[] spawnPoints;

    public int enemiesPerWave = 3;

    public TMP_Text enemyCounterText;
    public TMP_Text cashText;

    private int enemiesRemaining;
    private int currentWave = 0;
    private bool waveActive = false;
    private bool waitingForNextWave = false;

    private Color defaultTextColor;

    private int _playerCash = 0;
    public int playerCash
    {
        get { return _playerCash; }
        set
        {
            _playerCash = value;
            UpdateCashUI();
        }
    }

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        defaultTextColor = enemyCounterText.color;
        UpdateCashUI();
        StartCoroutine(StartWaveCountdown());
    }

    private void Update()
    {
        if (waitingForNextWave && Input.GetKeyDown(KeyCode.Q))
        {
            waitingForNextWave = false;
            StartCoroutine(StartWaveCountdown());
        }
    }

    IEnumerator StartWaveCountdown()
    {
        currentWave++;

        bool isBossWave = currentWave == 10;

        enemyCounterText.color = isBossWave ? Color.red : defaultTextColor;

        enemyCounterText.text = (isBossWave ? "Boss fight starting in 3" : "Starting wave " + currentWave + " in 3");
        yield return new WaitForSeconds(1f);

        enemyCounterText.text = (isBossWave ? "Boss fight starting in 2" : "Starting wave " + currentWave + " in 2");
        yield return new WaitForSeconds(1f);

        enemyCounterText.text = (isBossWave ? "Boss fight starting in 1" : "Starting wave " + currentWave + " in 1");
        yield return new WaitForSeconds(1f);

        StartNextWave();
    }

    void StartNextWave()
    {
        waveActive = true;

        if (currentWave == 10)
        {
            enemiesRemaining = 1;
            UpdateEnemyCounterUI();
            SpawnBoss();
        }
        else
        {
            enemiesRemaining = 3 * currentWave;
            UpdateEnemyCounterUI();
            StartCoroutine(SpawnWave());
        }
    }

    IEnumerator SpawnWave()
    {
        for (int i = 0; i < enemiesRemaining; i++)
        {
            SpawnEnemy();
            yield return null;
        }
    }

    void SpawnEnemy()
    {
        int randomEnemy = Random.Range(0, enemyPrefabs.Length);
        int randomSpawn = Random.Range(0, spawnPoints.Length);

        GameObject enemyObj = Instantiate(
            enemyPrefabs[randomEnemy],
            spawnPoints[randomSpawn].position,
            Quaternion.identity
        );

        Target enemyScript = enemyObj.GetComponent<Target>();
        if (enemyScript != null)
        {
            enemyScript.maxHealth += currentWave * 10;
        }
    }

    void SpawnBoss()
    {
        int randomSpawn = Random.Range(0, spawnPoints.Length);

        Instantiate(
            bossPrefab,
            spawnPoints[randomSpawn].position,
            Quaternion.identity
        );
    }

    public void EnemyKilled(int cashReward)
    {
        enemiesRemaining--;
        playerCash += cashReward;
        UpdateEnemyCounterUI();

        if (enemiesRemaining <= 0 && waveActive)
        {
            waveActive = false;

            if (currentWave == 10)
            {
                StartCoroutine(WinSequence());
            }
            else
            {
                waitingForNextWave = true;
                enemyCounterText.color = defaultTextColor;
                enemyCounterText.text = "Press Q to start next wave";
            }
        }
    }

    IEnumerator WinSequence()
    {
        enemyCounterText.color = Color.red;
        int nextLevel = SceneManager.GetActiveScene().buildIndex + 1;
        PlayerPrefs.SetInt("UnlockedLevel", nextLevel);
        PlayerPrefs.Save();
        for (int i = 20; i > 0; i--)
        {
            enemyCounterText.text = "YOU WIN!\nReturning to main menu in " + i;
            yield return new WaitForSeconds(1f);
        }

        SceneManager.LoadScene("Menu");
    }

    void UpdateEnemyCounterUI()
    {
        if (enemyCounterText != null && waveActive)
            enemyCounterText.text = "Enemies Remaining: " + enemiesRemaining;
    }

    void UpdateCashUI()
    {
        if (cashText != null)
            cashText.text = playerCash.ToString();
    }
}
