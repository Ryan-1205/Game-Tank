using UnityEngine;

public class TankShooting : MonoBehaviour
{
    public GameObject bulletPrefab; 
    public Transform firePoint;     
    
    [Header("Visual & Force Settings")]
    public float bulletForce = 20f;
    public GameObject muzzleFlashPrefab; 

    [Header("Shop Integration")]
    public GameObject[] allBulletPrefabs; 

    [Header("Continuous Laser Settings")]
    public LineRenderer laserLineRenderer; 
    public float laserRange = 20f;
    public LayerMask laserHitLayers;       
    public int laserDamagePerFrame = 1;
    public float damageInterval = 0.2f;    
    private float nextDamageTime;

    [Header("Laser Audio")]
    public AudioClip laserLoopClip;

    private AudioSource sfxSource;
    private Collider2D[] allTankColliders; // Array untuk menyimpan semua collider milik tank dan child-nya

    void Start()
    {
        sfxSource = GetComponent<AudioSource>();
        
        // Ambil semua collider di badan, turret, dan moncong tank agar tidak kecolongan
        allTankColliders = GetComponentsInChildren<Collider2D>(); 

        // Ambil indeks tank yang aktif digunakan player
        int activeTankIndex = PlayerPrefs.GetInt("SelectedTank", 0);
        
        if (allBulletPrefabs != null && activeTankIndex < allBulletPrefabs.Length)
        {
            if (allBulletPrefabs[activeTankIndex] != null)
            {
                bulletPrefab = allBulletPrefabs[activeTankIndex];
            }
        }

        if (laserLineRenderer != null) laserLineRenderer.enabled = false;
    }

    void Update()
    {
        // 1. Ambil peluru aktif secara dinamis dari toko (WeaponManager) tiap frame
        GameObject peluruYangDipakai = bulletPrefab;

        if (WeaponManager.Instance != null && WeaponManager.Instance.currentBulletPrefab != null)
        {
            peluruYangDipakai = WeaponManager.Instance.currentBulletPrefab;
        }

        if (peluruYangDipakai == null) return;

        // 2. Cek tipe peluru secara real-time langsung di Update
        bool apakahLaser = peluruYangDipakai.GetComponent<PlasmaLaser>() != null || peluruYangDipakai.name.ToLower().Contains("laser");

        // 3. Atur eksekusi input berdasarkan tipe peluru
        if (apakahLaser)
        {
            if (Input.GetButton("Fire1")) 
            {
                ShootContinuousLaser();
            }
            if (Input.GetButtonUp("Fire1")) 
            {
                StopContinuousLaser();
            }
        }
        else
        {
            if (Input.GetButtonDown("Fire1"))
            {
                StopContinuousLaser(); 
                ShootPhysicalBullet(peluruYangDipakai);
            }
        }
    }

    void ShootPhysicalBullet(GameObject peluru)
    {
        if (firePoint == null) return;

        Vector3 spawnPosition = firePoint.position;
        Quaternion spawnRotation = firePoint.rotation;

        GameObject projectile = Instantiate(peluru, spawnPosition, spawnRotation);

        // Matikan tabrakan peluru dengan semua collider tank
        if (allTankColliders != null)
        {
            Collider2D projectileCollider = projectile.GetComponent<Collider2D>();
            if (projectileCollider != null)
            {
                foreach (Collider2D tankCol in allTankColliders)
                {
                    if (tankCol != null) Physics2D.IgnoreCollision(projectileCollider, tankCol);
                }
            }
        }

        // Berikan kecepatan awal untuk peluru
        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = firePoint.up * bulletForce;
        }

        // HUBUNGKAN DATA TOKO / RAMBO
        RamboBurstBullet ramboScript = projectile.GetComponent<RamboBurstBullet>();
        if (ramboScript != null)
        {
            // Beritahu peluru rambo di mana letak moncong tank kamu agar peluru berikutnya lahir di sana
            ramboScript.moncongTank = firePoint;
        }

        // EFEK VISUAL MUZZLE FLASH
        if (muzzleFlashPrefab != null)
        {
            GameObject flash = Instantiate(muzzleFlashPrefab, spawnPosition, spawnRotation);
            flash.transform.SetParent(firePoint);
            Destroy(flash, 0.1f);
        }

        // KOREKSI UTAMA FILTER AUDIO: 
        // Jika peluru yang ditembakkan BUKAN Rambo, langsung mainkan suara tembakan tank.
        // Tidak perlu mengecek 'peluru == bulletPrefab' lagi agar sinkron dengan Toko/WeaponManager.
        if (ramboScript == null)
        {
            if (sfxSource != null && sfxSource.clip != null)
            {
                // Tambahkan , 0.5f di dalam kurung PlayOneShot buat ngecilin suaranya
                sfxSource.PlayOneShot(sfxSource.clip, 0.5f);
            }
        }
    }

    void ShootContinuousLaser()
    {
        if (laserLineRenderer == null || firePoint == null) return;

        laserLineRenderer.enabled = true;

        if (sfxSource != null && laserLoopClip != null)
        {
            sfxSource.loop = true; 
            if (!sfxSource.isPlaying)
            {
                sfxSource.clip = laserLoopClip;
                sfxSource.volume = 0.2f;
                sfxSource.Play();
            }
        }

        Vector2 startPos = firePoint.position;
        Vector2 laserDirection = firePoint.up;
        Vector2 endPos = startPos + (laserDirection * laserRange);

        RaycastHit2D hit = Physics2D.Raycast(startPos, laserDirection, laserRange, laserHitLayers);

        if (hit.collider != null)
        {
            endPos = hit.point;

            if (Time.time >= nextDamageTime)
            {
                if (hit.collider.CompareTag("Enemy") || hit.collider.CompareTag("Boss"))
                {
                    Health enemyHealth = hit.collider.GetComponent<Health>();
                    if (enemyHealth == null) enemyHealth = hit.collider.GetComponentInParent<Health>();

                    if (enemyHealth != null)
                    {
                        enemyHealth.ApplyDamage(laserDamagePerFrame);
                        nextDamageTime = Time.time + damageInterval;
                    }
                }
            }
        }

        laserLineRenderer.positionCount = 2;
        laserLineRenderer.SetPosition(0, startPos);
        laserLineRenderer.SetPosition(1, endPos);
    }

    void StopContinuousLaser()
    {
        if (laserLineRenderer != null) laserLineRenderer.enabled = false;

        if (sfxSource != null && sfxSource.clip == laserLoopClip)
        {
            sfxSource.Stop();
            sfxSource.loop = false; 
            sfxSource.volume = 0.5f;
        }
    }

    public void ChangeBulletPrefab(GameObject newBullet)
    {
        if (newBullet == null) return;

        bulletPrefab = newBullet;
        Debug.Log("Weapon Changed via UI: " + newBullet.name);
    }
}