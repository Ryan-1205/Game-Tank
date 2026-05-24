using UnityEngine;

public class EnemyFollow : MonoBehaviour
{
    public float speed = 3f;
    public float bodyRotationSpeed = 360f;

    [Header("Component References")]
    public Transform bodyTransform;   
    public Transform turretTransform; 

    [Header("Shooting Settings")]
    public GameObject enemyBulletPrefab; 
    public Transform firePoint;          
    public float fireRate = 2f;          
    private float nextFireTime;          

    // Tempat memasukkan prefab cahaya di Inspector musuh (Punya Lu)
    public GameObject muzzleFlashPrefab; 

    [Header("AI Obstacle Avoidance")]
    public LayerMask obstacleLayer;      // Pilih Layer "Obstacles" di Inspector
    public float detectionDistance = 2f; // Jarak sensor mendeteksi batu
    public float avoidanceForce = 2f;    // Seberapa tajam musuh membelok menghindari batu

    // --- INTEGRASI AUDIO GABUNGAN (Punya Fikri) ---
    [Header("Audio Settings (NPC)")]
    public float minEnginePitch = 0.7f;  // Nada mesin terendah
    public float maxEnginePitch = 1.2f;  // Nada mesin tertinggi
    private AudioSource[] audioSources;  // Array penampung komponen
    private AudioSource engineAudio;     // Slot suara mesin (Audio Source 1)
    private AudioSource shootAudio;      // Slot suara tembak (Audio Source 2)

    private Transform player;

    void Start()
    {
        GameObject playerObj = GameObject.Find("Player");
        if (playerObj != null) player = playerObj.transform;

        // --- AMBIL AUDIO OTOMATIS (Punya Fikri) ---
        audioSources = GetComponents<AudioSource>();
        if (audioSources.Length >= 2)
        {
            engineAudio = audioSources[0]; // Audio Source pertama = mesin
            shootAudio = audioSources[1];  // Audio Source kedua = tembakan
            
            // Hidupkan suara mesin diesel saat musuh spawn
            engineAudio.loop = true;
            engineAudio.Play();
        }
        else
        {
            Debug.LogWarning("Peringatan: " + gameObject.name + " butuh 2 Audio Source di Inspector agar suara mesin & tembakan berfungsi!");
        }
    }

    void Update()
    {
        if (player != null)
        {
            // 1. Hitung arah dasar langsung menuju player
            Vector2 targetDirection = (player.position - transform.position).normalized;
            Vector2 finalMoveDirection = targetDirection;

            // --- LOGIKA SENSOR MATA (RAYCAST AVOIDANCE) ---
            RaycastHit2D hit = Physics2D.Raycast(transform.position, targetDirection, detectionDistance, obstacleLayer);
            
            Debug.DrawRay(transform.position, targetDirection * detectionDistance, hit.collider != null ? Color.red : Color.green);

            if (hit.collider != null)
            {
                Vector2 avoidanceDirection = Vector2.Perpendicular(hit.normal).normalized;
                finalMoveDirection = (targetDirection + avoidanceDirection * avoidanceForce).normalized;
            }

            // --- 2. JALANKAN PERGERAKAN BADAN ---
            transform.position = Vector2.MoveTowards(transform.position, (Vector2)transform.position + finalMoveDirection, speed * Time.deltaTime);

            if (bodyTransform != null)
            {
                float bodyAngle = Mathf.Atan2(finalMoveDirection.y, finalMoveDirection.x) * Mathf.Rad2Deg - 90f;
                Quaternion targetBodyRotation = Quaternion.Euler(0, 0, bodyAngle);
                bodyTransform.rotation = Quaternion.RotateTowards(bodyTransform.rotation, targetBodyRotation, bodyRotationSpeed * Time.deltaTime);
            }

            // --- INTEGRASI AUDIO MESIN DINAMIS (Punya Fikri) ---
            // Suara mesin bakal ngegas/berubah pitch secara acak halus biar kerasa hidup
            if (engineAudio != null)
            {
                engineAudio.pitch = Mathf.MoveTowards(engineAudio.pitch, Random.Range(minEnginePitch, maxEnginePitch), Time.deltaTime * 0.5f);
            }

            // --- 3. LOGIKA MEMBIDIK (TURET TETAP LOCK PLAYER) ---
            if (turretTransform != null)
            {
                Vector2 turretDir = (player.position - turretTransform.position).normalized;
                float turretAngle = Mathf.Atan2(turretDir.y, turretDir.x) * Mathf.Rad2Deg - 90f;
                turretTransform.rotation = Quaternion.Euler(0, 0, turretAngle);
            }

            // --- 4. LOGIKA MENEMBAK ---
            if (Time.time >= nextFireTime)
            {
                Shoot();
                nextFireTime = Time.time + fireRate;
            }
        }
    }

    void Shoot()
    {
        if (firePoint != null && enemyBulletPrefab != null)
        {
            // 1. Munculkan peluru musuh
            GameObject bullet = Instantiate(enemyBulletPrefab, firePoint.position, firePoint.rotation);
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.AddForce(firePoint.up * 10f, ForceMode2D.Impulse);
            }

            // 2. MUNCULKAN VISUAL EFEK CAHAYA (Punya Lu)
            if (muzzleFlashPrefab != null)
            {
                Vector2 forwardDirection = firePoint.up;
                float angle = Mathf.Atan2(forwardDirection.y, forwardDirection.x) * Mathf.Rad2Deg - 90f;
                Quaternion exactRotation = Quaternion.Euler(0, 0, angle);

                GameObject flash = Instantiate(muzzleFlashPrefab, firePoint.position, exactRotation);
                Destroy(flash, 0.1f);
            }

            // 3. BUNYIKAN SUARA TEMBAKAN (Punya Fikri)
            if (shootAudio != null && shootAudio.clip != null)
            {
                shootAudio.PlayOneShot(shootAudio.clip);
            }
        }
    }
}