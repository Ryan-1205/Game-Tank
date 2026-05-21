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

    [Header("Audio Settings (NPC)")]
    private AudioSource[] audioSources;  // Menampung dua Audio Source dari Inspector
    private AudioSource engineAudio;     // Audio Source ke-1 (Mesin)
    private AudioSource shootAudio;      // Audio Source ke-2 (Tembak)

    public float minEnginePitch = 0.7f;  // Nada mesin terendah (saat variasi acak)
    public float maxEnginePitch = 1.2f;  // Nada mesin tertinggi (saat variasi acak)

    void Start()
    {
        GameObject playerObj = GameObject.Find("Player");
        if (playerObj != null) player = playerObj.transform;

        // Otomatis mencari semua komponen Audio Source yang nempel di objek ini
        audioSources = GetComponents<AudioSource>();
        
        // Memastikan minimal ada 2 Audio Source terpasang agar tidak error
        if (audioSources.Length >= 2)
        {
            engineAudio = audioSources[0]; // Audio Source pertama otomatis jadi mesin
            shootAudio = audioSources[1];  // Audio Source kedua otomatis jadi tembakan
            
            // Aktifkan suara mesin diesel sejak musuh lahir
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
            // --- LOGIKA MENGEJAR (Kode Asli Timmu) ---
            Vector2 direction = (player.position - transform.position).normalized;
            transform.position = Vector2.MoveTowards(transform.position, player.position, speed * Time.deltaTime);

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
            transform.rotation = Quaternion.Euler(0, 0, angle);

            // --- LOGIKA AUDIO MESIN DINAMIS ---
            // Mengacak pitch secara halus biar suara gerombolan tank musuh bervariasi dan tidak monoton kaku
            if (engineAudio != null)
            {
                engineAudio.pitch = Mathf.MoveTowards(engineAudio.pitch, Random.Range(minEnginePitch, maxEnginePitch), Time.deltaTime * 0.5f);
            }

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
            
            if (rb != null)
            {
                rb.AddForce(firePoint.up * 10f, ForceMode2D.Impulse); // Kecepatan peluru musuh
            }

            // KOREKSI LOGIKA: Bunyikan suara tembakan 3D milik NPC
            if (shootAudio != null && shootAudio.clip != null)
            {
                shootAudio.PlayOneShot(shootAudio.clip);
            }
        }
    }
}