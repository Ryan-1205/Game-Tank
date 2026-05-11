using UnityEngine;

public class EnemyFollow : MonoBehaviour
{
    public float speed = 3f;
    private Transform player;

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
            // --- LOGIKA MENGEJAR ---
            Vector2 direction = (player.position - transform.position).normalized;
            transform.position = Vector2.MoveTowards(transform.position, player.position, speed * Time.deltaTime);

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
            transform.rotation = Quaternion.Euler(0, 0, angle);

            // --- LOGIKA MENEMBAK ---
            // Cek apakah sudah waktunya nembak
            if (Time.time >= nextFireTime)
            {
                Shoot();
                nextFireTime = Time.time + fireRate; // Reset timer jeda
            }
        }
    }

    void Shoot()
    {
        // Pastikan firePoint dan prefab sudah diisi di Inspector
        if (firePoint != null && enemyBulletPrefab != null)
        {
            GameObject bullet = Instantiate(enemyBulletPrefab, firePoint.position, firePoint.rotation);
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            rb.AddForce(firePoint.up * 10f, ForceMode2D.Impulse); // Kecepatan peluru musuh
        }
    }
}