using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    [System.Serializable]
    public class Wave
    {
        public string waveName;
        public GameObject enemyPrefab;
        public int maxEnemiesAtOnce; 
        public float duration;       
        public float spawnRate = 1f; 
    }

    public Wave[] waves;
    public float timeBetweenWaves = 5f;

    [Header("Dynamic Spawning Settings")]
    public Transform player;
    public float minSpawnRadius = 5f; 
    public float maxSpawnRadius = 8f;

    [Header("Map Boundary Settings")]
    public Collider2D mapCollider; 

    [Header("Boss Settings")]
    public GameObject bossPrefab;

    [Header("3 Stage Settings")]
    public int currentStage = 1; 
    public float difficultyMultiplier = 1.5f; 

    [Header("Anti-Bug Obstacle Detection")]
    // KOREKSI WAJIB: Masukkan layer rintangan/batu/dinding kamu di Inspector Unity
    public LayerMask obstacleLayer; 
    // Jari-jari lingkaran cek fisik (sesuaikan dengan ukuran badan musuh)
    public float checkRadius = 0.5f; 
    // Batas maksimal percobaan acak mencari titik kosong agar game tidak hang
    private int maxSpawnAttempts = 10; 

    private int nextWave = 0;
    private bool isWaveActive = false; 

    void Start()
    {
        if (TankSpawner.Instance != null && TankSpawner.Instance.spawnedPlayer != null)
        {
            player = TankSpawner.Instance.spawnedPlayer.transform;
        }

        if (waves.Length > 0) StartCoroutine(PlayGameRoutine());
    }

    IEnumerator PlayGameRoutine()
    {
        while (currentStage <= 3)
        {
            Debug.Log($"[STAGE SYSTEM] >>> MEMASUKI STAGE {currentStage} <<<");
            
            Debug.Log($"[SPAWNER] Bersiap! Musuh pertama akan muncul dalam {timeBetweenWaves} detik...");
            yield return new WaitForSeconds(timeBetweenWaves);

            nextWave = 0; 

            while (nextWave < waves.Length)
            {
                yield return StartCoroutine(RunWave(waves[nextWave]));
                
                nextWave++;
                if (nextWave < waves.Length)
                {
                    Debug.Log($"[WAVE TRANSITION] {waves[nextWave - 1].waveName} Selesai! Menunggu {timeBetweenWaves} detik...");
                    yield return new WaitForSeconds(timeBetweenWaves);
                }
            }

            yield return StartCoroutine(SpawnBossRoutine());

            currentStage++;
            if (currentStage <= 3)
            {
                yield return new WaitForSeconds(timeBetweenWaves);
            }
        }
        Debug.Log("[VICTORY] GAME SELESAI!");
    }

    IEnumerator RunWave(Wave _wave)
    {
        int finalMaxEnemies = Mathf.RoundToInt(_wave.maxEnemiesAtOnce * Mathf.Pow(difficultyMultiplier, currentStage - 1));
        float finalSpawnRate = _wave.spawnRate / Mathf.Pow(difficultyMultiplier, currentStage - 1);

        float timer = _wave.duration;
        isWaveActive = true;

        while (timer > 0)
        {
            int currentEnemyCount = GameObject.FindGameObjectsWithTag("Enemy").Length;

            if (currentEnemyCount < finalMaxEnemies)
            {
                SpawnEnemy(_wave.enemyPrefab);
                yield return new WaitForSeconds(finalSpawnRate);
                timer -= finalSpawnRate;
            }
            else
            {
                timer -= Time.deltaTime;
                yield return null; 
            }
        }

        isWaveActive = false;
    }

    void SpawnEnemy(GameObject _enemy)
    {
        if (player == null)
        {
            if (TankSpawner.Instance != null && TankSpawner.Instance.spawnedPlayer != null)
            {
                player = TankSpawner.Instance.spawnedPlayer.transform;
            }
            else
            {
                return; 
            }
        }

        if (_enemy == null) return;

        Vector3 finalSpawnPos = Vector3.zero;
        bool posisiAmanDitemukan = false;

        // Lakukan perulangan untuk mencari koordinat yang bebas dari rintangan
        for (int i = 0; i < maxSpawnAttempts; i++)
        {
            Vector2 randomDir = Random.insideUnitCircle.normalized;
            float spawnDistance = Random.Range(minSpawnRadius, maxSpawnRadius);
            Vector3 targetPos = player.position + (Vector3)(randomDir * spawnDistance);

            // Terapkan batasan map collider jika ada
            if (mapCollider != null)
            {
                Bounds mapBounds = mapCollider.bounds;
                float clampedX = Mathf.Clamp(targetPos.x, mapBounds.min.x, mapBounds.max.x);
                float clampedY = Mathf.Clamp(targetPos.y, mapBounds.min.y, mapBounds.max.y);
                targetPos = new Vector3(clampedX, clampedY, 0f);
            }

            // KOREKSI LOGIKA: Cek apakah di posisi koordinat ini menabrak obstacle
            Collider2D hitObstacle = Physics2D.OverlapCircle(targetPos, checkRadius, obstacleLayer);

            if (hitObstacle == null)
            {
                // Jika bersih tidak ada rintangan, kunci posisi ini dan keluar dari loop pencarian
                finalSpawnPos = targetPos;
                posisiAmanDitemukan = true;
                break;
            }
        }

        // Jalankan eksekusi spawn berdasarkan verifikasi kelayakan posisi
        if (posisiAmanDitemukan)
        {
            Instantiate(_enemy, finalSpawnPos, Quaternion.identity);
        }
        else
        {
            // Jika sekeliling player penuh obstacle, spawn di titik terjauh yang aman dari obstacle
            Debug.LogWarning("[Spawner] Gagal menemukan posisi kosong di sekitar player karena padat rintangan.");
        }
    }

    IEnumerator SpawnBossRoutine()
    {
        yield return new WaitForSeconds(3f);
        
        if (player == null) yield break;

        Vector3 bossSpawnPos = Vector3.zero;
        bool bossPosisiAman = false;

        // Lakukan pengecekan posisi kosong juga untuk boss agar tidak terjebak di batu
        for (int i = 0; i < maxSpawnAttempts; i++)
        {
            Vector2 randomDir = Random.insideUnitCircle.normalized;
            Vector3 targetPos = player.position + (Vector3)(randomDir * 6f);

            if (mapCollider != null)
            {
                Bounds mapBounds = mapCollider.bounds;
                float clampedX = Mathf.Clamp(targetPos.x, mapBounds.min.x, mapBounds.max.x);
                float clampedY = Mathf.Clamp(targetPos.y, mapBounds.min.y, mapBounds.max.y);
                targetPos = new Vector3(clampedX, clampedY, 0f);
            }

            // Gunakan radius agak besar karena ukuran tubuh Boss biasanya lebih lebar dari kroco
            Collider2D hitObstacle = Physics2D.OverlapCircle(targetPos, checkRadius * 1.5f, obstacleLayer);

            if (hitObstacle == null)
            {
                bossSpawnPos = targetPos;
                bossPosisiAman = true;
                break;
            }
        }

        // Jika gagal dapat tempat kosong, paksa spawn di koordinat player + offset minor yang aman
        if (!bossPosisiAman) bossSpawnPos = player.position + new Vector3(3f, 3f, 0f);

        GameObject bossInstance = Instantiate(bossPrefab, bossSpawnPos, Quaternion.identity);
        
        while (bossInstance != null)
        {
            yield return null; 
        }
    }

    // Menampilkan jangkauan deteksi titik aman di Unity Scene View berupa lingkaran merah (opsional)
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, checkRadius);
    }
}