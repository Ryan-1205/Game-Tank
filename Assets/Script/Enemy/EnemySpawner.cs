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

    [Header("Map Boundary Settings")]
    public Collider2D mapCollider; // Taruh CameraBounds atau Collider peta di sini

    [Header("Boss Settings")]
    public GameObject bossPrefab;

    [Header("3 Stage Settings")]
    public int currentStage = 1; // Melacak stage saat ini (1 sampai 3)
    public float difficultyMultiplier = 1.5f; // Pengali kesulitan per stage (1.5x lebih ramai/cepat)

    private int nextWave = 0;
    private bool isWaveActive = false;

    void Start()
    {
        if (player == null) player = GameObject.FindGameObjectWithTag("Player").transform;
        if (waves.Length > 0) StartCoroutine(PlayGameRoutine());
    }

    IEnumerator PlayGameRoutine()
    {
        // Bungkus keseluruhan loop dengan sistem 3 Stage
        while (currentStage <= 3)
        {
            // === LOG CONSOLE: MASUK STAGE BARU ===
            Debug.Log($"<color=#00FFFF><b>[STAGE SYSTEM]</b> ===================================</color>");
            Debug.Log($"<color=#00FFFF><b>[STAGE SYSTEM]</b> >>> MEMASUKI STAGE {currentStage} <<<</color>");
            Debug.Log($"<color=#00FFFF><b>[STAGE SYSTEM]</b> ===================================</color>");
            
            nextWave = 0; // Reset kembali ke Wave 1 tiap ganti stage

            while (nextWave < waves.Length)
            {
                yield return StartCoroutine(RunWave(waves[nextWave]));
                
                nextWave++;
                if (nextWave < waves.Length)
                {
                    // === LOG CONSOLE: JEDA ANTAR WAVE ===
                    Debug.Log($"<color=#FFA500><b>[WAVE TRANSITION]</b> {waves[nextWave - 1].waveName} Selesai! Menunggu {timeBetweenWaves} detik sebelum {waves[nextWave].waveName}...</color>");
                    yield return new WaitForSeconds(timeBetweenWaves);
                }
            }

            // Semua wave di stage ini selesai, panggil Boss dan TUNGGU sampai hancur
            yield return StartCoroutine(SpawnBossRoutine());

            // Boss hancur, naikkan stage
            currentStage++;
            if (currentStage <= 3)
            {
                // === LOG CONSOLE: STAGE CLEAR ===
                Debug.Log($"<color=#00FF00><b>[STAGE CLEAR]</b> Boss Stage {currentStage - 1} Berhasil Dihancurkan! Bersiap menuju Stage {currentStage}...</color>");
                yield return new WaitForSeconds(timeBetweenWaves);
            }
        }

        // === LOG CONSOLE: GAME TAMAT ===
        Debug.Log("<color=#FFD700><b>[VICTORY]</b> SELAMAT! Anda telah menyelesaikan seluruh Stage (Game Selesai)!</color>");
    }

    IEnumerator RunWave(Wave _wave)
    {
        // Hitung batas musuh yang sudah dikalikan tingkat kesulitan stage saat ini
        int finalMaxEnemies = Mathf.RoundToInt(_wave.maxEnemiesAtOnce * Mathf.Pow(difficultyMultiplier, currentStage - 1));
        float finalSpawnRate = _wave.spawnRate / Mathf.Pow(difficultyMultiplier, currentStage - 1);

        // === LOG CONSOLE: STARTING WAVE ===
        Debug.Log($"<color=#FFFF00><b>[WAVE GERAK]</b> Memulai Stage {currentStage} - {_wave.waveName} | Durasi: {_wave.duration}s | Max Enemies: {finalMaxEnemies} | Spawn Rate: {finalSpawnRate:F2}s</color>");
        
        float timer = _wave.duration;
        isWaveActive = true;

        while (timer > 0)
        {
            // Hitung jumlah musuh dengan tag "Enemy" saat ini
            int currentEnemyCount = GameObject.FindGameObjectsWithTag("Enemy").Length;

            // Jika belum mencapai batas maksimal wave ini (yang sudah disesuaikan stage), spawn musuh baru
            if (currentEnemyCount < finalMaxEnemies)
            {
                SpawnEnemy(_wave.enemyPrefab);
                // Kasih jeda yang sudah disesuaikan dengan stage saat ini
                yield return new WaitForSeconds(finalSpawnRate);
                timer -= finalSpawnRate;
            }
            else
            {
                timer -= Time.deltaTime;
                yield return null; // Tunggu ke frame berikutnya
            }
        }

        isWaveActive = false;
        
        // === LOG CONSOLE: WAVE SELESAI ===
        Debug.Log($"<color=#FF3333><b>[WAVE END]</b> {_wave.waveName} di Stage {currentStage} Habis Waktunya!</color>");
    }

    void SpawnEnemy(GameObject _enemy)
    {
        if (player == null || _enemy == null) return;

        // 1. Hitung posisi acak lingkaran di sekitar player
        Vector2 randomDir = Random.insideUnitCircle.normalized;
        float spawnDistance = Random.Range(minSpawnRadius, maxSpawnRadius);
        Vector3 spawnPos = player.position + (Vector3)(randomDir * spawnDistance);

        // 2. Batasi posisi berdasarkan batas Collider Peta jika ada
        if (mapCollider != null)
        {
            Bounds mapBounds = mapCollider.bounds;

            // Paksa koordinat agar tidak melewati batas minimal dan maximal collider
            float clampedX = Mathf.Clamp(spawnPos.x, mapBounds.min.x, mapBounds.max.x);
            float clampedY = Mathf.Clamp(spawnPos.y, mapBounds.min.y, mapBounds.max.y);

            spawnPos = new Vector3(clampedX, clampedY, 0f);
        }

        Instantiate(_enemy, spawnPos, Quaternion.identity);
    }

    IEnumerator SpawnBossRoutine()
    {
        // === LOG CONSOLE: PERSIAPAN BOSS ===
        Debug.Log($"<color=#FF0000><b>[BOSS WARNING]</b> Semua wave di Stage {currentStage} bersih! Mengunci pergerakan waktu, bersiap memanggil Boss...</color>");
        yield return new WaitForSeconds(3f);
        
        Vector2 randomDir = Random.insideUnitCircle.normalized;
        Vector3 spawnPos = player.position + (Vector3)(randomDir * 18f);

        // Batasi posisi Boss agar tidak spawn di luar batas peta
        if (mapCollider != null)
        {
            Bounds mapBounds = mapCollider.bounds;
            float clampedX = Mathf.Clamp(spawnPos.x, mapBounds.min.x, mapBounds.max.x);
            float clampedY = Mathf.Clamp(spawnPos.y, mapBounds.min.y, mapBounds.max.y);
            spawnPos = new Vector3(clampedX, clampedY, 0f);
        }

        GameObject bossInstance = Instantiate(bossPrefab, spawnPos, Quaternion.identity);
        
        // === LOG CONSOLE: BOSS MUNCUL ===
        Debug.Log($"<color=#FF0055><b>[BOSS SPAWNED]</b> BOSS STAGE {currentStage} MUNCUL DI ARENA!</color>");

        // KUNCI UTAMA: Tahan game di sini selama Boss masih ada (belum di-Destroy)
        while (bossInstance != null)
        {
            yield return null; 
        }
        
        // === LOG CONSOLE: DETEKSI BOSS MATI ===
        Debug.Log($"<color=#00FF88><b>[BOSS DEAD]</b> Sistem mendeteksi Boss Stage {currentStage} telah dihancurkan!</color>");
    }
}