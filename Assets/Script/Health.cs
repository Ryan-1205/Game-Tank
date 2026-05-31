using UnityEngine;
using Ilumisoft.HealthSystem; // WAJIB: Biar bisa tersambung ke sistem UI aset template

public class Health : HealthComponent // KOREKSI: Sekarang mewarisi class bawaan aset
{
    [Header("Base Health Settings (Aset Template)")]
    [SerializeField] private float maxHealth = 3.0f;
    [SerializeField, Range(0, 1)] private float initialRatio = 1.0f;

    // Override properti wajib bawaan Ilumisoft Health System
    public override float MaxHealth { get => maxHealth; set => maxHealth = value; }
    public override float CurrentHealth { get; set; } = 0.0f;
    public override bool IsAlive => CurrentHealth > 0.0f;

    [Header("Death Visual Effect (Punya Lu)")]
    public GameObject deathExplosionPrefab; 

    [Header("Death Audio Effect (Punya Lu)")]
    public AudioClip deathSoundClip;

    [Header("Loot Settings (Sistem Belanja Lu)")]
    public GameObject coinPrefab; 
    [Range(0, 100)] public float dropChance = 100f; 

    private void Awake()
    {
        // Set darah awal sesuai dengan rasio di Inspector template
        SetHealth(MaxHealth * initialRatio);
    }

    // Fungsi bawaan template untuk mengatur nilai darah secara presisi
    public override void SetHealth(float health)
    {
        float previousHealth = CurrentHealth;
        CurrentHealth = Mathf.Clamp(health, 0, MaxHealth);
        float difference = CurrentHealth - previousHealth;

        if (Mathf.Abs(difference) > 0.0f)
        {
            OnHealthChanged?.Invoke(difference); // Mengirim sinyal data ke UI Health Bar
        }
    }

    // Fungsi bawaan template untuk menambah darah (Medkit/Heal)
    public override void AddHealth(float amount)
    {
        if (!IsAlive) return;

        float previousHealth = CurrentHealth;
        CurrentHealth = Mathf.Clamp(CurrentHealth + amount, 0, MaxHealth);
        float changeAmount = CurrentHealth - previousHealth;

        if (changeAmount > 0.0f)
        {
            OnHealthChanged?.Invoke(changeAmount); // Update grafik Health Bar ke kanan
        }
    }

    // === KOREKSI UTAMA: Pengganti Fungsi TakeDamage Lama ===
    public override void ApplyDamage(float damage)
    {
        if (!IsAlive) return;

        float previousHealth = CurrentHealth;
        CurrentHealth = Mathf.Clamp(CurrentHealth - damage, 0, MaxHealth);
        float changeAmount = CurrentHealth - previousHealth;

        Debug.Log($"{gameObject.name} terkena hit sebesar {damage}! Sisa nyawa: {CurrentHealth}");

        if (Mathf.Abs(changeAmount) > 0.0f)
        {
            OnHealthChanged?.Invoke(changeAmount); // Sinyal agar UI Health Bar berkurang berkala

            if (CurrentHealth <= 0.0f)
            {
                Die(); // Panggil fungsi meledak dan drop coin milik lo
                OnHealthEmpty?.Invoke(); // Sinyal tambahan opsional untuk sistem aset
            }
        }
    }

    // Fungsi kematian milik lo tetap dipertahankan seutuhnya
    void Die()
    {
        if (deathExplosionPrefab != null)
        {
            Instantiate(deathExplosionPrefab, transform.position, Quaternion.identity);
        }

        if (deathSoundClip != null)
        {
            AudioSource.PlayClipAtPoint(deathSoundClip, transform.position);
        }

        if (gameObject.CompareTag("Enemy") || gameObject.CompareTag("Boss"))
        {
            if (coinPrefab != null)
            {
                float randomRoll = Random.Range(0f, 100f);
                if (randomRoll <= dropChance)
                {
                    GameObject spawnedCoin = Instantiate(coinPrefab, transform.position, Quaternion.identity);
                    CoinItem coinScript = spawnedCoin.GetComponent<CoinItem>();
                    
                    if (coinScript != null)
                    {
                        if (gameObject.CompareTag("Boss")) coinScript.coinValue = 200;
                        else coinScript.coinValue = 100;
                    }
                }
            }
        }

        Destroy(gameObject);
    }
}