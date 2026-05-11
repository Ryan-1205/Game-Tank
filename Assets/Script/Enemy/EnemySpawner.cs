using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    [System.Serializable]
    public class Wave
    {
        public string waveName;
        public GameObject enemyPrefab;
        public int maxEnemiesAtOnce; // Batas musuh di layar untuk wave ini
        public float duration;       // Durasi wave ini (detik)
        public float spawnRate = 1f; // Jeda waktu antar spawn musuh baru
    }

    public Wave[] waves;
    public float timeBetweenWaves = 5f;

    [Header("Dynamic Spawning Settings")]
    public Transform player;
    public float minSpawnRadius = 15f;
    public float maxSpawnRadius = 20f;

    [Header("Boss Settings")]
    public GameObject bossPrefab;

    private int nextWave = 0;
    private bool isWaveActive = false;

    void Start()
    {
        if (player == null) player = GameObject.FindGameObjectWithTag("Player").transform;
        if (waves.Length > 0) StartCoroutine(PlayGameRoutine());
    }

    IEnumerator PlayGameRoutine()
    {
        while (nextWave < waves.Length)
        {
            yield return StartCoroutine(RunWave(waves[nextWave]));
            
            nextWave++;
            if (nextWave < waves.Length)
            {
                Debug.Log("Wave Selesai! Menunggu " + timeBetweenWaves + " detik...");
                yield return new WaitForSeconds(timeBetweenWaves);
            }
        }

        // Semua wave selesai, panggil Boss
        StartCoroutine(SpawnBossRoutine());
    }

    IEnumerator RunWave(Wave _wave)
    {
        Debug.Log("Memulai " + _wave.waveName + " | Durasi: " + _wave.duration + "s");
        float timer = _wave.duration;
        isWaveActive = true;

        while (timer > 0)
        {
            // Hitung jumlah musuh dengan tag "Enemy" saat ini
            int currentEnemyCount = GameObject.FindGameObjectsWithTag("Enemy").Length;

            // Jika belum mencapai batas maksimal wave ini, spawn musuh baru
            if (currentEnemyCount < _wave.maxEnemiesAtOnce)
            {
                SpawnEnemy(_wave.enemyPrefab);
                // Kasih jeda dikit biar nggak langsung "meledak" spawn barengan
                yield return new WaitForSeconds(_wave.spawnRate);
            }

            timer -= Time.deltaTime;
            yield return null; // Tunggu ke frame berikutnya
        }

        isWaveActive = false;
        Debug.Log(_wave.waveName + " Habis Waktunya!");
    }

    void SpawnEnemy(GameObject _enemy)
    {
        if (player == null || _enemy == null) return;

        Vector2 randomDir = Random.insideUnitCircle.normalized;
        float spawnDistance = Random.Range(minSpawnRadius, maxSpawnRadius);
        Vector3 spawnPos = player.position + (Vector3)(randomDir * spawnDistance);

        Instantiate(_enemy, spawnPos, Quaternion.identity);
    }

    IEnumerator SpawnBossRoutine()
    {
        Debug.Log("Persiapan Boss...");
        yield return new WaitForSeconds(3f);
        
        Vector2 randomDir = Random.insideUnitCircle.normalized;
        Vector3 spawnPos = player.position + (Vector3)(randomDir * 18f);
        Instantiate(bossPrefab, spawnPos, Quaternion.identity);
        Debug.Log("BOSS MUNCUL!");
    }
}