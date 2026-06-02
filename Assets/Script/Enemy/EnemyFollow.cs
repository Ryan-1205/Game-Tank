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

    public GameObject muzzleFlashPrefab; 

    [Header("AI Obstacle Avoidance")]
    public LayerMask obstacleLayer;      
    public float detectionDistance = 2f; 
    public float avoidanceForce = 2f;    

    [Header("Audio Settings (NPC)")]
    public float minEnginePitch = 0.7f;  
    public float maxEnginePitch = 1.2f;  
    private AudioSource[] audioSources;  
    private AudioSource engineAudio;     
    private AudioSource shootAudio;      

    private Transform player;

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }

        audioSources = GetComponents<AudioSource>();

        if (audioSources.Length >= 2)
        {
            engineAudio = audioSources[0];
            shootAudio = audioSources[1];

            engineAudio.loop = true;
            engineAudio.volume = 0.3f;
            engineAudio.Play();
        }
    }

    void Update()
    {
        if (player != null)
        {
            // 1. Hitung arah dasar menuju player
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

            // 2. Jalankan pergerakan badan
            transform.position = Vector2.MoveTowards(transform.position, (Vector2)transform.position + finalMoveDirection, speed * Time.deltaTime);

            if (bodyTransform != null)
            {
                float bodyAngle = Mathf.Atan2(finalMoveDirection.y, finalMoveDirection.x) * Mathf.Rad2Deg - 90f;
                Quaternion targetBodyRotation = Quaternion.Euler(0, 0, bodyAngle);
                bodyTransform.rotation = Quaternion.RotateTowards(bodyTransform.rotation, targetBodyRotation, bodyRotationSpeed * Time.deltaTime);
            }

            // 3. Integrasi Audio Mesin Dinamis
            if (engineAudio != null)
            {
                engineAudio.pitch = Mathf.MoveTowards(engineAudio.pitch, Random.Range(minEnginePitch, maxEnginePitch), Time.deltaTime * 0.5f);
            }

            // 4. Logika Membidik (Turret)
            if (turretTransform != null)
            {
                Vector2 turretDir = (player.position - turretTransform.position).normalized;
                float turretAngle = Mathf.Atan2(turretDir.y, turretDir.x) * Mathf.Rad2Deg - 90f;
                turretTransform.rotation = Quaternion.Euler(0, 0, turretAngle);
            }

            // 5. Logika Menembak
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
            GameObject bullet = Instantiate(enemyBulletPrefab, firePoint.position, firePoint.rotation);
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.AddForce(firePoint.up * 10f, ForceMode2D.Impulse);
            }

            // MUNCULKAN VISUAL EFEK CAHAYA
            if (muzzleFlashPrefab != null)
            {
                Vector2 forwardDirection = firePoint.up;
                float angle = Mathf.Atan2(forwardDirection.y, forwardDirection.x) * Mathf.Rad2Deg - 90f;
                Quaternion exactRotation = Quaternion.Euler(0, 0, angle);

                GameObject flash = Instantiate(muzzleFlashPrefab, firePoint.position, exactRotation);
                Destroy(flash, 0.1f);
            }

            // BUNYIKAN SUARA TEMBAKAN (Volume dikecilkan 0.4f)
            if (shootAudio != null && shootAudio.clip != null)
            {
                shootAudio.PlayOneShot(shootAudio.clip, 0.4f);
            }
        }
    }
}