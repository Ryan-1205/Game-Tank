using UnityEngine;

public class Bullet : MonoBehaviour 
{
    public int damage = 1; // Besaran damage peluru

    // BARIS BARU: Slot untuk prefab ledakan kecil peluru player di Inspector
    [Header("Impact Effect")]
    public GameObject impactExplosionPrefab; 

    void Start() 
    {
        // Otomatis hancur setelah 3 detik jika tidak kena apa-apa
        Destroy(gameObject, 3f);
    }

    void OnTriggerEnter2D(Collider2D hitInfo) 
    {
        // 1. Jika kena objek dengan Tag "Enemy"
        if (hitInfo.CompareTag("Enemy")) 
        {
            Health enemyHealth = hitInfo.GetComponent<Health>();

            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage);
            }

            // KOREKSI: Munculkan efek ledakan kecil sebelum peluru hancur kena musuh
            if (impactExplosionPrefab != null)
            {
                Instantiate(impactExplosionPrefab, transform.position, Quaternion.identity);
            }

            Destroy(gameObject);
            return; // Keluar dari fungsi agar tidak mengecek if di bawahnya lagi
        }

        // 2. Jika menabrak rintangan peta (Layer "Obstacles")
        if (hitInfo.gameObject.layer == LayerMask.NameToLayer("Obstacles"))
        {
            // KOREKSI: Munculkan efek ledakan kecil sebelum peluru hancur kena batu
            if (impactExplosionPrefab != null)
            {
                Instantiate(impactExplosionPrefab, transform.position, Quaternion.identity);
            }

            Destroy(gameObject);
        }
    }
}