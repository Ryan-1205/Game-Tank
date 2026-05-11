using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    [System.Serializable]
    public class Wave
    {
        public string waveName;
        public GameObject enemyPrefab;
        public int count;
        public float rate;
    }

    public Wave[] waves;
    public float timeBetweenWaves = 5f;

    [Header("Dynamic Spawning Settings")]
    public Transform player;
    public float minSpawnRadius = 15f; 
    public float maxSpawnRadius = 20f; 

    [Header("Boss Settings")]
    public GameObject bossPrefab;      // Tarik prefab Boss ke sini
    public float timeBeforeBoss = 3f; // Jeda tambahan sebelum Boss muncul

    private int nextWave = 0;
    private bool allWavesDone = false;

    void Start()
    {
        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }

        if (waves.Length > 0)
        {
            StartCoroutine(SpawnWave(waves[nextWave]));
        }
    }

    IEnumerator SpawnWave(Wave _wave)
    {
        Debug.Log("Spawning Wave: " + _wave.waveName);

        for (int i = 0; i < _wave.count; i++)
        {
            SpawnEnemy(_wave.enemyPrefab);
            yield return new WaitForSeconds(1f / _wave.rate);
        }

        nextWave++;
        
        if (nextWave < waves.Length)
        {
            yield return new WaitForSeconds(timeBetweenWaves);
            StartCoroutine(SpawnWave(waves[nextWave]));
        }
        else
        {
            // Semua wave reguler selesai
            allWavesDone = true;
            Debug.Log("Semua Wave Selesai! Menunggu Boss...");
            StartCoroutine(SpawnBossRoutine());
        }
    }

    // Fungsi baru untuk memunculkan Boss
    IEnumerator SpawnBossRoutine()
    {
        // Jeda penutup setelah wave terakhir sebelum Boss muncul
        yield return new WaitForSeconds(timeBeforeBoss);

        if (bossPrefab != null && player != null)
        {
            Debug.Log("BOSS MUNCUL!");
            
            // Logika spawn radius yang sama agar Boss muncul dari luar layar
            Vector2 randomDir = Random.insideUnitCircle.normalized;
            float spawnDistance = (minSpawnRadius + maxSpawnRadius) / 2; // Ambil nilai tengah radius
            Vector3 spawnPos = player.position + (Vector3)(randomDir * spawnDistance);

            Instantiate(bossPrefab, spawnPos, Quaternion.identity);
        }
        else
        {
            Debug.LogWarning("Boss Prefab belum diisi di Inspector!");
        }
    }

    void SpawnEnemy(GameObject _enemy)
    {
        if (player == null || _enemy == null) return;

        Vector2 randomDir = Random.insideUnitCircle.normalized;
        float spawnDistance = Random.Range(minSpawnRadius, maxSpawnRadius);
        Vector3 spawnPos = player.position + (Vector3)(randomDir * spawnDistance);

        Instantiate(_enemy, spawnPos, Quaternion.identity);
    }
}