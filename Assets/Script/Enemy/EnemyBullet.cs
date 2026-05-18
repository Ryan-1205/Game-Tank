using UnityEngine;

public class EnemyBullet : MonoBehaviour 
{
    public int damage = 1;

    void Start() 
    {
        Destroy(gameObject, 3f);
    }

    void OnTriggerEnter2D(Collider2D hitInfo) 
    {
        // Cek apakah yang ditabrak adalah Player
        if (hitInfo.CompareTag("Player")) 
        {
            Health playerHealth = hitInfo.GetComponent<Health>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }

            Destroy(gameObject);
        }
        
        // KOREKSI LOGIKA: Peluru musuh juga hancur jika menabrak Layer "Obstacles"
        if (hitInfo.gameObject.layer == LayerMask.NameToLayer("Obstacles"))
        {
            Destroy(gameObject);
        }
    }
}