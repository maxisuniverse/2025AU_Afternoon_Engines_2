using UnityEngine;
using System.Collections;

public class waveSpawner : MonoBehaviour
{
    [SerializeField] private float countdown;
    [SerializeField] private Transform[] spawnPoints;

    public Wave[] waves;

    public int currentWaveIndex = 0;
    private bool spawning = false;

    private bool readyToCountDown;

    private void Start()
    {
        readyToCountDown = true;

        for (int i = 0; i < waves.Length; i++)
        {
            waves[i].enemiesLeft = waves[i].enemies.Length;
        }
    }

    private void Update()
    {
        if (readyToCountDown && !spawning)
        {
            countdown -= Time.deltaTime;
        }

        if (spawning) return;

        if (currentWaveIndex >= waves.Length)
        {
            return;
        }

        if (countdown <= 0)
        {
            readyToCountDown = false;
            StartCoroutine(SpawnWave());
        }
    }

    private IEnumerator SpawnWave()
    {
        spawning = true;

        Wave currentWave = waves[currentWaveIndex];

        for (int i = 0; i < currentWave.enemies.Length; i++)
        {
            if (spawnPoints.Length == 0)
            {
                Debug.LogError("No spawn points assigned");
                spawning = false;
                yield break;
            }

            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

            // refernce to spawn enemy
            Enemy newEnemy = Instantiate(currentWave.enemies[i], spawnPoint.position, spawnPoint.rotation);

            ZombieHealth zh = newEnemy.GetComponent<ZombieHealth>();
            if (zh != null)
            {
                zh.waveSpawner = this;
                zh.myWave = currentWave;
            }

            // play zombie sound
            ZombieSound zs = newEnemy.GetComponent<ZombieSound>();
            if (zs != null)
            {
                zs.PlayMoanLoop();
            }

            yield return new WaitForSeconds(currentWave.timeToNextEnemy);
        }

        while (currentWave.enemiesLeft > 0)
            yield return null;

        currentWaveIndex++;

        if (currentWaveIndex < waves.Length)
        {
            countdown = waves[currentWaveIndex].timeToNextWave;
            readyToCountDown = true;
        }

        if (currentWaveIndex >= waves.Length)
        {
            Debug.Log("All waves complete");
        }

        spawning = false;
    }
}

[System.Serializable]
public class Wave
{
    public Enemy[] enemies;
    public float timeToNextEnemy;
    public float timeToNextWave;

    [HideInInspector] public int enemiesLeft;
}