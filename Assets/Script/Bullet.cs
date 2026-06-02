using UnityEngine;

public class Bullet : MonoBehaviour 
{
    public int damage = 1; 

    [Header("Visual Impact Effect")]
    public GameObject impactExplosionPrefab; 

    [Header("Audio Impact Effect")]
    public AudioClip impactSoundClip; 

    void Start() 
    {
        Destroy(gameObject, 3f);
    }

    void OnTriggerEnter2D(Collider2D hitInfo) 
    {
        // Abaikan jika mendeteksi badannya sendiri, tank player, atau sesama peluru
        if (hitInfo.CompareTag("Player") || hitInfo.CompareTag("Bullet")) return;

        if (hitInfo.CompareTag("Enemy") || hitInfo.CompareTag("Boss")) 
        {
            Health enemyHealth = hitInfo.GetComponent<Health>();
            
            if (enemyHealth == null)
            {
                enemyHealth = hitInfo.GetComponentInParent<Health>();
            }

            if (enemyHealth != null)
            {
                if (gameObject.name.ToLower().Contains("rocket") || gameObject.name.ToLower().Contains("misil"))
                {
                    enemyHealth.SetLastDamageType("rocket");
                }
                else
                {
                    enemyHealth.SetLastDamageType("normal");
                }

                enemyHealth.ApplyDamage(damage); 
            }

            PlayImpactEffects();
            Destroy(gameObject);
            return; 
        }

        if (hitInfo.gameObject.layer == LayerMask.NameToLayer("Obstacles") || hitInfo.CompareTag("Wall"))
        {
            PlayImpactEffects();
            Destroy(gameObject);
        }
    }

    void PlayImpactEffects()
    {
        if (impactExplosionPrefab != null)
        {
            Instantiate(impactExplosionPrefab, transform.position, Quaternion.identity);
        }

        if (impactSoundClip != null)
        {
            // Menggunakan PlayClipAtPoint agar suara meledak muncul di koordinat posisi musuh hancur
            AudioSource.PlayClipAtPoint(impactSoundClip, transform.position);
        }
    }
}