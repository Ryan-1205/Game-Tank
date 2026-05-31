using UnityEngine;

public class Bullet : MonoBehaviour 
{
    public int damage = 1; // Besaran damage peluru

    [Header("Visual Impact Effect")]
    public GameObject impactExplosionPrefab; 

    // BARIS BARU: Slot untuk memasukkan file audio ledakan peluru player (.mp3/.wav)
    [Header("Audio Impact Effect")]
    public AudioClip impactSoundClip; 

    void Start() 
    {
        // Otomatis hancur setelah 3 detik jika tidak kena apa-apa
        Destroy(gameObject, 3f);
    }

    void OnTriggerEnter2D(Collider2D hitInfo) 
    {
        // === KOREKSI UTAMA: Cek jika kena objek dengan Tag "Enemy" ATAU Tag "Boss" ===
        if (hitInfo.CompareTag("Enemy") || hitInfo.CompareTag("Boss")) 
        {
            Health enemyHealth = hitInfo.GetComponent<Health>();

            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage);
            }

            // Eksekusi efek visual dan suara sebelum peluru hancur
            PlayImpactEffects();

            Destroy(gameObject);
            return; // Keluar dari fungsi agar tidak mengecek if di bawahnya lagi
        }

        // 2. Jika menabrak rintangan peta (Layer "Obstacles")
        if (hitInfo.gameObject.layer == LayerMask.NameToLayer("Obstacles"))
        {
            // Eksekusi efek visual dan suara sebelum peluru hancur
            PlayImpactEffects();

            Destroy(gameObject);
        }
    }

    // Fungsi pembantu eksekusi efek visual + audio
    void PlayImpactEffects()
    {
        // Munculkan efek visual ledakan kecil lu
        if (impactExplosionPrefab != null)
        {
            Instantiate(impactExplosionPrefab, transform.position, Quaternion.identity);
        }

        // KOREKSI AUDIO: Mainkan suara ledakan peluru player secara mandiri di posisi 3D
        if (impactSoundClip != null)
        {
            AudioSource.PlayClipAtPoint(impactSoundClip, transform.position);
        }
    }
}