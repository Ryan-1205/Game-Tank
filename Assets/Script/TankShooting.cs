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
    public float damageInterval = 0.5f;
    private float nextDamageTime;

    // === FITUR BARU: OVERHEAT LASER ===
    [Header("Laser Overheat Settings")]
    public float maxLaserTime = 3f; // Maksimal ditahan (detik) sebelum kepanasan
    public float laserCooldown = 3f; // Lama hukuman nunggu pendinginan (detik)

    private float currentLaserHeat = 0f; // Menyimpan suhu laser saat ini
    private bool isOverheated = false; // Status apakah laser lagi rusak/kepanasan

    [Header("Laser Audio")]
    public AudioClip laserLoopClip;

    private PlayerController playerController;
    private AudioSource sfxSource;
    private Collider2D[] allTankColliders;

    void Start()
    {
        sfxSource = GetComponent<AudioSource>();
        playerController = GetComponentInParent<PlayerController>();
        allTankColliders = GetComponentsInChildren<Collider2D>();

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
        GameObject peluruYangDipakai = bulletPrefab;

        if (WeaponManager.Instance != null && WeaponManager.Instance.currentBulletPrefab != null)
        {
            peluruYangDipakai = WeaponManager.Instance.currentBulletPrefab;
        }

        if (peluruYangDipakai == null) return;

        bool apakahLaser = peluruYangDipakai.GetComponent<PlasmaLaser>() != null || peluruYangDipakai.name.ToLower().Contains("laser");

        if (apakahLaser)
        {
            // === LOGIKA OVERHEAT LASER ===
            if (isOverheated)
            {
                // Kalau lagi overheat, tombol ditekan pun ga ngaruh.
                // Turunkan suhu paksa sampai 0 dalam waktu 2 detik (laserCooldown)
                currentLaserHeat -= Time.deltaTime * (maxLaserTime / laserCooldown);

                if (currentLaserHeat <= 0f)
                {
                    currentLaserHeat = 0f;
                    isOverheated = false; // Udah dingin, siap tembak lagi!
                }
            }
            else
            {
                // Kalau GA overheat, baca klik player
                if (Input.GetButton("Fire1"))
                {
                    currentLaserHeat += Time.deltaTime; // Suhu naik seiring waktu ditekan

                    if (currentLaserHeat >= maxLaserTime)
                    {
                        // BATAS MAKSIMAL TERCAPAI (4 Detik)!
                        isOverheated = true;
                        StopContinuousLaser(); // Paksa matiin lasernya
                    }
                    else
                    {
                        ShootContinuousLaser(); // Masih aman, lanjut tembak!
                    }
                }
                else
                {
                    // Kalau player ngelepas kliknya sebelum 4 detik
                    StopContinuousLaser();

                    // Suhu laser turun pelan-pelan (nyicil pendinginan)
                    if (currentLaserHeat > 0f)
                    {
                        currentLaserHeat -= Time.deltaTime;
                    }
                }
            }
        }
        else
        {
            // === PELURU FISIK (Rambo, Roket, dll) TETAP PAKAI COOLDOWN SCRIPTABLE OBJECT ===
            if (Input.GetButtonDown("Fire1") || Input.GetButton("Fire1"))
            {
                if (playerController != null && playerController.CanShootNow())
                {
                    StopContinuousLaser();
                    ShootPhysicalBullet(peluruYangDipakai);
                }
            }
        }
    }

    void ShootPhysicalBullet(GameObject peluru)
    {
        if (firePoint == null) return;

        Vector3 spawnPosition = firePoint.position;
        Quaternion spawnRotation = firePoint.rotation;

        GameObject projectile = Instantiate(peluru, spawnPosition, spawnRotation);

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

        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = firePoint.up * bulletForce;
        }

        RamboBurstBullet ramboScript = projectile.GetComponent<RamboBurstBullet>();
        if (ramboScript != null)
        {
            ramboScript.moncongTank = firePoint;
        }

        if (muzzleFlashPrefab != null)
        {
            GameObject flash = Instantiate(muzzleFlashPrefab, spawnPosition, spawnRotation);
            flash.transform.SetParent(firePoint);
            Destroy(flash, 0.1f);
        }

        if (ramboScript == null)
        {
            if (sfxSource != null && sfxSource.clip != null)
            {
                sfxSource.PlayOneShot(sfxSource.clip);
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
        }
    }

    public void ChangeBulletPrefab(GameObject newBullet)
    {
        if (newBullet == null) return;

        bulletPrefab = newBullet;
    }
}