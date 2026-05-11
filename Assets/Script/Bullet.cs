using UnityEngine;

public class Bullet : MonoBehaviour 
{
    public int damage = 1; // Besaran damage peluru

    void Start() 
    {
        // Otomatis hancur setelah 3 detik jika tidak kena apa-apa
        Destroy(gameObject, 3f);
    }

    void OnTriggerEnter2D(Collider2D hitInfo) 
    {
        // Jika kena objek dengan Tag "Enemy"
        if (hitInfo.CompareTag("Enemy")) 
        {
            // Ambil komponen Health dari objek yang ditabrak
            Health enemyHealth = hitInfo.GetComponent<Health>();

            if (enemyHealth != null)
            {
                // Kasih damage ke musuh
                enemyHealth.TakeDamage(damage);
            }

            // Hancurkan pelurunya sendiri
            Destroy(gameObject);
        }
    }
}