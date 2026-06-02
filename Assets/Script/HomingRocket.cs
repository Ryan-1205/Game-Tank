using UnityEngine;

public class HomingRocket : MonoBehaviour
{
    [Header("Rocket Movement")]
    public float speed = 8f;
    public float rotateSpeed = 400f; // Bikin tinggi biar beloknya patah-patah (snapping)

    [Header("Targeting & Radar Settings")]
    public LayerMask enemyLayer;      // Di-set ke layer "Musuh"
    public LayerMask obstacleLayer;   // Di-set ke layer "Obstacles"
    public float radarRadius = 10f;   // Jarak pandang roket nyari musuh di layar
    public float sensorLength = 2.5f; // Panjang antena radar pendeteksi tembok

    [Header("Damage & Impact")]
    public int damage = 3;
    public GameObject impactExplosionPrefab;
    public AudioClip shootSoundClip;  // <-- TAMBAHKAN INI (Isi dengan suara roket meluncur)
    public AudioClip impactSoundClip;

    private Rigidbody2D rb;
    private Transform targetEnemy;
    private float snapTimer = 0f;
    private float snapInterval = 0.05f; // Interval waktu biar beloknya patah-patah per frame pixel

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.linearVelocity = transform.up * speed;

        // BIKIN BUNYI PAS ROKET MELUNCUR
        if (shootSoundClip != null)
        {
            AudioSource.PlayClipAtPoint(shootSoundClip, transform.position);
        }

        FindClosestEnemy();
        Destroy(gameObject, 5f);
    }

    void FixedUpdate()
    {
        snapTimer += Time.fixedDeltaTime;

        // Logika pergerakan patah-patah (hanya update arah setiap interval tertentu)
        if (snapTimer >= snapInterval)
        {
            Vector2 moveDirection = transform.up;

            // 1. CEK SENSOR ANTENA: Hindari tembok terlebih dahulu!
            bool obstacleAhead = CheckObstacleAvoidance(ref moveDirection);

            // 2. JIKA JALUR AMAN: Baru kejar musuh
            if (!obstacleAhead)
            {
                // Kalau target mati atau hilang, cari musuh baru yang masih hidup
                if (targetEnemy == null || !targetEnemy.gameObject.activeInHierarchy)
                {
                    FindClosestEnemy();
                }

                if (targetEnemy != null)
                {
                    moveDirection = ((Vector2)targetEnemy.position - rb.position).normalized;
                }
            }

            // 3. EKSEKUSI ROTASI PATAH-PATAH (Pixel Art Style)
            if (moveDirection != Vector2.zero)
            {
                float targetAngle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg - 90f;
                rb.rotation = Mathf.MoveTowardsAngle(rb.rotation, targetAngle, rotateSpeed * snapInterval);
            }

            snapTimer = 0f;
        }

        // Jaga agar roket selalu melaju kencang ke depan arah moncongnya
        rb.linearVelocity = transform.up * speed;
    }

    // LOGIKA RADAR SENSOR: Mendeteksi rintangan di depan dan banting setir patah-patah
    bool CheckObstacleAvoidance(ref Vector2 chosenDirection)
    {
        Vector2 startPos = transform.position;
        Vector2 forwardDir = transform.up;
        Vector2 leftDir = -transform.right;
        Vector2 rightDir = transform.right;

        // Tembakkan 3 Raycast di moncong roket (Tengah, Kiri, Kanan)
        RaycastHit2D hitCenter = Physics2D.Raycast(startPos, forwardDir, sensorLength, obstacleLayer);
        RaycastHit2D hitLeft = Physics2D.Raycast(startPos, (forwardDir + leftDir).normalized, sensorLength * 0.7f, obstacleLayer);
        RaycastHit2D hitRight = Physics2D.Raycast(startPos, (forwardDir + rightDir).normalized, sensorLength * 0.7f, obstacleLayer);

        // Visualisasi garis radar di mode Scene Editor Unity biar kelihatan pas tes
        Debug.DrawRay(startPos, forwardDir * sensorLength, Color.red);
        Debug.DrawRay(startPos, (forwardDir + leftDir).normalized * sensorLength * 0.7f, Color.yellow);
        Debug.DrawRay(startPos, (forwardDir + rightDir).normalized * sensorLength * 0.7f, Color.yellow);

        if (hitCenter.collider != null)
        {
            // Ada tembok di depan! Cari jalan keluar ke kiri atau kanan yang kosong
            if (hitLeft.collider == null) chosenDirection = leftDir;
            else if (hitRight.collider == null) chosenDirection = rightDir;
            else chosenDirection = -forwardDir; // Mentok semua, putar balik!
            
            return true;
        }
        else if (hitLeft.collider != null)
        {
            chosenDirection = rightDir; // Ada tembok di kiri, banting setir ke kanan
            return true;
        }
        else if (hitRight.collider != null)
        {
            chosenDirection = leftDir;  // Ada tembok di kanan, banting setir ke kiri
            return true;
        }

        return false; // Jalur bersih bebas hambatan
    }

    // LOGIKA NYARI MUSUH TERDEKAT DI AREA KAMERA
    void FindClosestEnemy()
    {
        // Cari semua objek yang ada di dalam radius radar roket
        Collider2D[] scannedObjects = Physics2D.OverlapCircleAll(transform.position, radarRadius, enemyLayer);
        
        float closestDistance = Mathf.Infinity;
        Transform nearestTarget = null;

        foreach (Collider2D obj in scannedObjects)
        {
            if (obj.CompareTag("Enemy") || obj.CompareTag("Boss"))
            {
                float distanceToEnemy = Vector2.Distance(transform.position, obj.transform.position);
                if (distanceToEnemy < closestDistance)
                {
                    closestDistance = distanceToEnemy;
                    nearestTarget = obj.transform;
                }
            }
        }

        targetEnemy = nearestTarget;
    }

    // PENDETEKSI TABRAKAN LANGSUNG (Disesuaikan dengan sistem Health baru)
    void OnTriggerEnter2D(Collider2D hitInfo)
    {
        if (hitInfo.CompareTag("Enemy") || hitInfo.CompareTag("Boss"))
        {
            Health enemyHealth = hitInfo.GetComponent<Health>();
            if (enemyHealth == null) enemyHealth = hitInfo.GetComponentInParent<Health>();

            if (enemyHealth != null)
            {
                // KOREKSI: Set tipe damage roket ke sistem baru, lalu panggil ApplyDamage resmi
                enemyHealth.SetLastDamageType("rocket");
                enemyHealth.ApplyDamage(damage);
            }

            PlayImpactEffects();
            Destroy(gameObject);
        }
        else if (hitInfo.gameObject.layer == LayerMask.NameToLayer("Obstacles"))
        {
            PlayImpactEffects();
            Destroy(gameObject);
        }
    }

    void PlayImpactEffects()
    {
        if (impactExplosionPrefab != null)
        {
            Instantiate(impactExplosionPrefab, transform.position, Quaternion.identity);
        }

        if (impactSoundClip != null)
        {
            AudioSource.PlayClipAtPoint(impactSoundClip, transform.position);
        }
    }
}