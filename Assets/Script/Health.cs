using UnityEngine;

public class Health : MonoBehaviour
{
    public int maxHealth = 3;
    private int currentHealth;

    // Slot untuk memasukkan prefab ledakan besar di Inspector (Punya Lu)
    [Header("Death Visual Effect")]
    public GameObject deathExplosionPrefab; 

    // BARIS BARU: Slot untuk memasukkan file audio ledakan besar tank hancur (.mp3/.wav)
    [Header("Death Audio Effect")]
    public AudioClip deathSoundClip;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log(gameObject.name + " kena hit! Sisa nyawa: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        // 1. Spawn efek visual ledakan besar lu sebelum tank hancur
        if (deathExplosionPrefab != null)
        {
            Instantiate(deathExplosionPrefab, transform.position, Quaternion.identity);
        }

        // 2. KOREKSI AUDIO: Bunyikan suara ledakan hancur besar secara mandiri di posisi tank
        if (deathSoundClip != null)
        {
            AudioSource.PlayClipAtPoint(deathSoundClip, transform.position);
        }

        // Hancurkan objek tank asli (Player / Enemy / Boss)
        Destroy(gameObject);
    }
}