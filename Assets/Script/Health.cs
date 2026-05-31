using UnityEngine;

public class Health : MonoBehaviour
{
    public int maxHealth = 3;
    private int currentHealth;

    // Slot untuk memasukkan prefab ledakan besar di Inspector
    [Header("Death Visual Effect")]
    public GameObject deathExplosionPrefab; 

    // Slot untuk memasukkan file audio ledakan besar tank hancur (.mp3/.wav)
    [Header("Death Audio Effect")]
    public AudioClip deathSoundClip;

    // === BARIS BARU: SISTEM EKONOMI SHOP ===
    [Header("Loot Settings (Khusus Musuh)")]
    public GameObject coinPrefab; // Seret prefab Coin lo ke sini di Inspector
    [Range(0, 100)] public float dropChance = 100f; // Peluang koin muncul (100 = pasti muncul)

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

    // 2. Bunyikan suara ledakan hancur besar secara mandiri di posisi tank
    if (deathSoundClip != null)
    {
        AudioSource.PlayClipAtPoint(deathSoundClip, transform.position);
    }

    // === 3. LOGIKA DROP COIN DENGAN PERBEDAAN NILAI ===
    if (gameObject.CompareTag("Enemy") || gameObject.CompareTag("Boss"))
    {
        if (coinPrefab != null)
        {
            float randomRoll = Random.Range(0f, 100f);
            if (randomRoll <= dropChance)
            {
                // Spawn koin di posisi tank mati, dan simpan referensinya ke dalam variabel 'spawnedCoin'
                GameObject spawnedCoin = Instantiate(coinPrefab, transform.position, Quaternion.identity);
                
                // Ambil komponen CoinItem dari koin yang baru saja lahir
                CoinItem coinScript = spawnedCoin.GetComponent<CoinItem>();
                
                if (coinScript != null)
                {
                    // Cek Tag objek ini untuk menentukan jumlah koinnya
                    if (gameObject.CompareTag("Boss"))
                    {
                        coinScript.coinValue = 200; // Kalau Boss dapet 200
                        Debug.Log($"<color=#90EE90><b>[LOOT]</b> BOSS hancur! Menjatuhkan Koin bernilai: 200</color>");
                    }
                    else
                    {
                        coinScript.coinValue = 100; // Kalau Enemy biasa dapet 100
                        Debug.Log($"<color=#90EE90><b>[LOOT]</b> Enemy biasa hancur! Menjatuhkan Koin bernilai: 100</color>");
                    }
                }
            }
        }
    }

    // Hancurkan objek tank asli (Player / Enemy / Boss)
    Destroy(gameObject);
}
}