using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class EnemyWaveManager : MonoBehaviour
{
    public bool Victory = false;
    public bool SpawnAroundPlayer;
    public float distance;
    public float minDistance;
    public float TrickleTimer;
    [System.Serializable]
    public class Wave
    {
        public GameObject enemyPrefab;
        public int count;
        public Transform spawnPoint;
    }

    [System.Serializable]
    public class EnemyWave
    {
        public Wave[] enemies;
    }

    public EnemyWave[] waves;
    public float delayBetweenWaves = 5.0f;
    public TextMeshProUGUI uiTimerText; // Assign your TextMeshPro UI Text element to show the timer

    private int currentWaveIndex = 0;
    private bool isWaveInProgress = false;

    void Start()
    {
        StartCoroutine(ManageWaves());
        uiTimerText.gameObject.SetActive(false);
    }

    IEnumerator ManageWaves()
    {
        if(currentWaveIndex >= waves.Length && AllEnemiesCleared())
        {
            Victory = true;
        }
        while (currentWaveIndex < waves.Length)
        {
            if (!isWaveInProgress)
            {
                if (AllEnemiesCleared())
                {
                    // Start displaying the timer here since we're in between waves.
                    uiTimerText.gameObject.SetActive(true);
                    float timeToNextWave = delayBetweenWaves;

                    while (timeToNextWave > 0)
                    {
                        uiTimerText.text = $"Next wave in: {Mathf.CeilToInt(timeToNextWave)}";
                        timeToNextWave -= Time.deltaTime;
                        yield return null;
                    }

                    uiTimerText.gameObject.SetActive(false);
                    isWaveInProgress = true;
                    StartCoroutine(SpawnWave(waves[currentWaveIndex]));
                }
            }
            else
            {
                if (AllEnemiesCleared())
                {
                    // Once all enemies are cleared, we're no longer in a wave.
                    isWaveInProgress = false;
                    currentWaveIndex++;
                    if (currentWaveIndex >= waves.Length)
                    {
                        Victory = true;
                    }
                }
            }

            yield return null;
        }
        // Optional: Display something when all waves are complete.
    }


    IEnumerator SpawnWave(EnemyWave wave)
    {
        foreach (Wave enemy in wave.enemies)
        {
            for (int i = 0; i < enemy.count; i++)
            {
                SpawnEnemyAtPoint(enemy.enemyPrefab, enemy.spawnPoint);
                yield return new WaitForSeconds(TrickleTimer); // Slight delay to stagger spawns if necessary
            }
        }
    }

    void SpawnEnemyAtPoint(GameObject enemyPrefab, Transform spawnPoint)
    {
        Vector3 spawnPos;
        if (spawnPoint != null && SpawnAroundPlayer == false)
        {
            // Spawn the enemy at a random point around the spawn point to avoid overlaps.
            spawnPos = spawnPoint.position + Random.insideUnitSphere * 2; // 2 units radius
            spawnPos.y = spawnPoint.position.y; // Assuming y is the height, keep it at spawn point's height.
        }
        else
        {
            // If no specific spawn point is assigned, use a random position on the terrain.
            // This assumes you have a method to get a random position on the terrain.
            spawnPos = GetRandomPositionOnTerrain();
        }

        Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
    }

    bool AllEnemiesCleared()
    {
        // You would need to check for all enemies in the current wave. Here's a simplified check:
        // Return true if there are no enemies left with the tag "Enemy". You should assign this tag to your enemies.
        return GameObject.FindGameObjectsWithTag("Enemy").Length == 0;
    }

    Vector3 GetRandomPositionOnTerrain()
    {
        // Implement your logic to find a random position on the terrain where an enemy can be spawned.
        // Make sure this position is walkable and not overlapping with other objects.
        Vector3 point = new Vector3(0, 0, 0);
        float angle = Random.Range(0, 360) * Mathf.Deg2Rad;
        float spawnDistance = Random.Range(minDistance, distance);
        float x = point.x + spawnDistance * Mathf.Cos(angle);
        float z = point.z + spawnDistance * Mathf.Sin(angle);
        float y = 0f;
        Vector3 Syzygy = new Vector3(x, 0f, z);
        RaycastHit hit;
        if (Physics.Raycast(Syzygy + Vector3.up * 1000, Vector3.down, out hit, Mathf.Infinity))
        {
            y = hit.point.y; // Set y to the hit point's y coordinate
        }

        return new Vector3(x, y, z);
        //return new Vector3(Random.Range(-10f, 10f), 0, Random.Range(-10f, 10f)); // Placeholder values
    }
}
