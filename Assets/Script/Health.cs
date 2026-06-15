using UnityEngine;
using Ilumisoft.HealthSystem;

public class Health : HealthComponent
{
    [Header("Base Health Settings (Aset Template)")]
    [SerializeField] private float maxHealth = 3.0f;
    [SerializeField, Range(0, 1)] private float initialRatio = 1.0f;

    public override float MaxHealth { get => maxHealth; set => maxHealth = value; }
    public override float CurrentHealth { get; set; } = 0.0f;
    public override bool IsAlive => CurrentHealth > 0.0f;

    [Header("Death Visual Effect")]
    public GameObject deathExplosionPrefab;

    [Header("Death Audio Effects (3 Variasi)")]
    public AudioClip normalExplosionSound;
    public AudioClip laserExplosionSound;
    public AudioClip rocketExplosionSound;

    private string lastDamageType = "normal";

    [Header("Loot Settings")]
    public GameObject coinPrefab;
    [Range(0, 100)] public float dropChance = 100f;

    private void Awake()
    {
        SetHealth(MaxHealth * initialRatio);
    }

    public override void SetHealth(float health)
    {
        float previousHealth = CurrentHealth;
        CurrentHealth = Mathf.Clamp(health, 0, MaxHealth);
        float difference = CurrentHealth - previousHealth;

        if (Mathf.Abs(difference) > 0.0f)
        {
            OnHealthChanged?.Invoke(difference);
        }
    }

    public override void AddHealth(float amount)
    {
        if (!IsAlive) return;

        float previousHealth = CurrentHealth;
        CurrentHealth = Mathf.Clamp(CurrentHealth + amount, 0, MaxHealth);
        float changeAmount = CurrentHealth - previousHealth;

        if (changeAmount > 0.0f)
        {
            OnHealthChanged?.Invoke(changeAmount);
        }
    }

    // KOREKSI: Semua peluru wajib lewat sini agar dibaca oleh sistem Ilumisoft
    public override void ApplyDamage(float damage)
    {
        if (!IsAlive) return;

        float previousHealth = CurrentHealth;
        CurrentHealth = Mathf.Clamp(CurrentHealth - damage, 0, MaxHealth);
        float changeAmount = CurrentHealth - previousHealth;

        Debug.Log($"{gameObject.name} terkena hit sebesar {damage}! Sisa nyawa: {CurrentHealth}");

        if (Mathf.Abs(changeAmount) > 0.0f)
        {
            OnHealthChanged?.Invoke(changeAmount);

            if (CurrentHealth <= 0.0f)
            {
                Die();
                OnHealthEmpty?.Invoke();
            }
        }
    }

    // Fungsi pembantu agar Bullet bisa ngasih tahu jenis senjatanya secara manual
    public void SetLastDamageType(string type)
    {
        lastDamageType = type.ToLower();
    }

    void Die()
    {
        if (deathExplosionPrefab != null)
        {
            Instantiate(deathExplosionPrefab, transform.position, Quaternion.identity);
        }

        AudioClip clipToPlay = normalExplosionSound;

        if (lastDamageType == "laser")
        {
            clipToPlay = laserExplosionSound;
        }
        else if (lastDamageType == "rocket" || lastDamageType == "misil")
        {
            clipToPlay = rocketExplosionSound;
        }

        if (clipToPlay != null)
        {
            AudioSource.PlayClipAtPoint(clipToPlay, transform.position);
        }

        // --- CODINGAN BARU: CEK APAPAKAH YANG MATI ITU PLAYER ---
        if (gameObject.CompareTag("Player"))
        {
            GameOverManager gameOverScript = FindObjectOfType<GameOverManager>();
            if (gameOverScript != null)
            {
                gameOverScript.MunculkanGameOver();
            }
        }
        // -------------------------------------------------------

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