using UnityEngine;

public class EnemyBullet : MonoBehaviour 
{
    public int damage = 1;

    [Header("Visual Impact Effect")]
    public GameObject impactExplosionPrefab; 

    // BARIS BARU: Slot untuk memasukkan file audio ledakan peluru (.mp3/.wav)
    [Header("Audio Impact Effect")]
    public AudioClip impactSoundClip; 

    void Start() 
    {
        // Supaya tidak menumpuk di memori, hancur dalam 3 detik
        Destroy(gameObject, 3f);
    }

    void OnTriggerEnter2D(Collider2D hitInfo) 
    {
        // 1. Cek apakah yang ditabrak adalah Player
        if (hitInfo.CompareTag("Player")) 
        {
            Health playerHealth = hitInfo.GetComponent<Health>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }

            // Eksekusi efek visual dan audio sebelum hancur
            PlayImpactEffects();

            Destroy(gameObject);
            return; 
        }
        
        // 2. Deteksi Rintangan Gabungan: Layer "Obstacles" (Punya Lu) ATAU Tag "Environment" (Punya Fikri)
        if (hitInfo.gameObject.layer == LayerMask.NameToLayer("Obstacles") || hitInfo.CompareTag("Environment"))
        {
            // Eksekusi efek visual dan audio sebelum hancur
            PlayImpactEffects();

            Destroy(gameObject);
        }
    }

    // Fungsi pembantu agar tidak perlu menulis kode efek dua kali
    void PlayImpactEffects()
    {
        // Munculkan visual ledakan kecil lu
        if (impactExplosionPrefab != null)
        {
            Instantiate(impactExplosionPrefab, transform.position, Quaternion.identity);
        }

        // KOREKSI AUDIO: Mainkan suara ledakan secara mandiri agar tidak terputus saat peluru hancur
        if (impactSoundClip != null)
        {
            AudioSource.PlayClipAtPoint(impactSoundClip, transform.position);
        }
    }
}