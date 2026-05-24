using UnityEngine;

public class Health : MonoBehaviour
{
    public int maxHealth = 3;
    private int currentHealth;

    // Slot untuk memasukkan prefab ledakan besar di Inspector
    [Header("Death Visual Effect")]
    public GameObject deathExplosionPrefab; 

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log(gameObject.name + " kena hit! Sisa nyawa: " + currentHealth);

        // KOREKSI UTAMA: Memperbaiki tanda kurung dan menghilangkan pengecekan ganda
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        // Spawn efek ledakan sebelum tank hancur murni
        if (deathExplosionPrefab != null)
        {
            Instantiate(deathExplosionPrefab, transform.position, Quaternion.identity);
        }

        // Hancurkan objek tank asli
        Destroy(gameObject);
    }
}