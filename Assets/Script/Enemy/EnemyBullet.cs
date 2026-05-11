using UnityEngine;

public class EnemyBullet : MonoBehaviour 
{
    public int damage = 1;

    void Start() 
    {
        // Supaya tidak menumpuk di memori, hancur dalam 3 detik
        Destroy(gameObject, 3f);
    }

    void OnTriggerEnter2D(Collider2D hitInfo) 
    {
        // Cek apakah yang ditabrak adalah Player
        if (hitInfo.CompareTag("Player")) 
        {
            // Ambil komponen Health dari Player
            Health playerHealth = hitInfo.GetComponent<Health>();

            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }

            // Peluru musuh hancur setelah kena player
            Destroy(gameObject);
        }
        
        // Opsional: Peluru hancur jika kena tembok/lingkungan (Tag "Environment")
        if (hitInfo.CompareTag("Environment"))
        {
            Destroy(gameObject);
        }
    }
}