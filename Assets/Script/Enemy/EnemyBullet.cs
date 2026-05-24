using UnityEngine;

public class EnemyBullet : MonoBehaviour 
{
    public int damage = 1;

    // BARIS BARU: Slot untuk prefab ledakan kecil peluru musuh di Inspector
    [Header("Impact Effect")]
    public GameObject impactExplosionPrefab; 

    void Start() 
    {
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

            // KOREKSI: Munculkan efek ledakan kecil sebelum peluru hancur kena player
            if (impactExplosionPrefab != null)
            {
                Instantiate(impactExplosionPrefab, transform.position, Quaternion.identity);
            }

            Destroy(gameObject);
            return; // Keluar dari fungsi
        }
        
        // 2. Jika menabrak Layer "Obstacles"
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