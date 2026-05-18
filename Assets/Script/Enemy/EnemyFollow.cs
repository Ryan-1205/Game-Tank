using UnityEngine;

public class EnemyFollow : MonoBehaviour
{
    public float speed = 3f;
    public float bodyRotationSpeed = 360f; // Kecepatan putar badan tank musuh
    private Transform player;

    [Header("Component References")]
    public Transform bodyTransform;   // Masukkan objek anak 'Body' di sini
    public Transform turretTransform; // Masukkan objek anak 'Turret' di sini

    [Header("Shooting Settings")]
    public GameObject enemyBulletPrefab; // Prefab peluru khusus musuh
    public Transform firePoint;          // Titik muncul peluru musuh
    public float fireRate = 2f;          // Jeda waktu antar tembakan (detik)
    private float nextFireTime;          // Timer internal

    void Start()
    {
        GameObject playerObj = GameObject.Find("Player");
        if (playerObj != null) player = playerObj.transform;
    }

    void Update()
    {
        if (player != null)
        {
            // Hitung arah dari musuh menuju ke player
            Vector2 targetDirection = (player.position - transform.position).normalized;

            // --- 1. LOGIKA GERAKAN & ROTASI BADAN (ANTI-KEPITING) ---
            if (bodyTransform != null)
            {
                // Hitung sudut rotasi target untuk badan tank
                // Asumsi awal: Asset gambar tank musuh menghadap ke ATAS. Jika menghadap KANAN, hapus bagian "- 90f"
                float bodyAngle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg - 90f;
                Quaternion targetBodyRotation = Quaternion.Euler(0, 0, bodyAngle);

                // Putar badan tank musuh secara halus menuju posisi player
                bodyTransform.rotation = Quaternion.RotateTowards(bodyTransform.rotation, targetBodyRotation, bodyRotationSpeed * Time.deltaTime);
            }

            // Gerakkan objek utama musuh maju searah dengan arah hadap badannya saat ini
            // Menggunakan Vector2.MoveTowards ke posisi player agar tetap presisi
            transform.position = Vector2.MoveTowards(transform.position, player.position, speed * Time.deltaTime);


            // --- 2. LOGIKA MEMBIDIK (TURET) ---
            if (turretTransform != null)
            {
                // Hitung arah turet secara mandiri (tetap mengunci player)
                Vector2 turretDir = (player.position - turretTransform.position).normalized;
                float turretAngle = Mathf.Atan2(turretDir.y, turretDir.x) * Mathf.Rad2Deg - 90f;
                
                turretTransform.rotation = Quaternion.Euler(0, 0, turretAngle);
            }

            // --- 3. LOGIKA MENEMBAK ---
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
            rb.AddForce(firePoint.up * 10f, ForceMode2D.Impulse);
        }
    }
}