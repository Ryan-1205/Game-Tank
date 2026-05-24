using UnityEngine;

public class TankShooting : MonoBehaviour
{
    public GameObject bulletPrefab; 
    public Transform firePoint;     
    
    [Header("Visual & Force Settings (From Lu)")]
    public float bulletForce = 20f;
    public GameObject muzzleFlashPrefab; 

    private AudioSource sfxSource;

    void Start()
    {
        sfxSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            Shoot();
        }
    }

    void Shoot()
    {
        // KOREKSI UTAMA: Jika firePoint lupa diisi atau posisinya salah, 
        // kita amankan otomatis pake posisi Turret ini sendiri biar gak macet
        Vector3 spawnPosition = (firePoint != null) ? firePoint.position : transform.position;
        Quaternion spawnRotation = (firePoint != null) ? firePoint.rotation : transform.rotation;

        if (bulletPrefab != null)
        {
            GameObject bullet = Instantiate(bulletPrefab, spawnPosition, spawnRotation);
            Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
            
            if (bulletRb != null)
            {
                // PERBAIKAN LOGIKA: Dorong peluru searah hadap atas dari TURRET (transform.up),
                // bukan firePoint, biar gak ketipu sama arah objek anak yang melintir
                bulletRb.AddForce(transform.up * bulletForce, ForceMode2D.Impulse);
            }
        }

        // Visual Efek Muzzle Flash
        if (muzzleFlashPrefab != null)
        {
            GameObject flash = Instantiate(muzzleFlashPrefab, spawnPosition, spawnRotation);
            if (firePoint != null) flash.transform.SetParent(firePoint);
            Destroy(flash, 0.1f);
        }

        // Audio Efek Tembakan
        if (sfxSource != null && sfxSource.clip != null)
        {
            sfxSource.PlayOneShot(sfxSource.clip);
        }
    }
}