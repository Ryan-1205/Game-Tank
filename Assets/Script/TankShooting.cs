using UnityEngine;

public class TankShooting : MonoBehaviour
{
    public GameObject bulletPrefab; 
    public Transform firePoint;     

    private AudioSource sfxSource;

    void Start()
    {
        // Otomatis mengambil komponen Audio Source yang nempel di Turret
        sfxSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        // Jika klik kiri mouse ditekan
        if (Input.GetButtonDown("Fire1"))
        {
            Shoot();
        }
    }

    void Shoot()
    {
        // 1. Memunculkan peluru
        if (bulletPrefab != null && firePoint != null)
        {
            Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        }

        // 2. KOREKSI LOGIKA: Bunyikan suara tembakan menggunakan PlayOneShot
        // Menggunakan PlayOneShot agar kalau kamu klik kiri dengan cepat, suaranya bertumpuk rapi dan tidak saling memotong
        if (sfxSource != null && sfxSource.clip != null)
        {
            sfxSource.PlayOneShot(sfxSource.clip);
        }
    }
}