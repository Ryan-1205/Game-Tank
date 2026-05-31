using UnityEngine;

public class EnemyBullet : MonoBehaviour 
{
    public int damage = 1;

    [Header("Visual Impact Effect")]
    public GameObject impactExplosionPrefab; 

    // Slot untuk memasukkan file audio ledakan peluru (.mp3/.wav)
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
            // PENCARIAN SMART 2D: Cari script Health di objek Player, atau di parent-nya
            Health playerHealth = hitInfo.GetComponent<Health>();
            
            if (playerHealth == null)
            {
                playerHealth = hitInfo.GetComponentInParent<Health>();
            }

            if (playerHealth != null)
            {
                // Memicu pengurangan darah Player sekaligus memicu animasi Health Bar Player menyusut
                playerHealth.ApplyDamage(damage);
            }

            // Eksekusi efek visual dan audio sebelum hancur
            PlayImpactEffects();

            Destroy(gameObject);
            return; 
        }
        
        // 2. Deteksi Rintangan Gabungan: Layer "Obstacles" ATAU Tag "Environment"
        if (hitInfo.gameObject.layer == LayerMask.NameToLayer("Obstacles") || hitInfo.CompareTag("Environment"))
        {
            // Eksekusi efek visual dan suara sebelum hancur
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

        // Mainkan suara ledakan secara mandiri agar tidak terputus saat peluru hancur
        if (impactSoundClip != null)
        {
            AudioSource.PlayClipAtPoint(impactSoundClip, transform.position);
        }
    }
}