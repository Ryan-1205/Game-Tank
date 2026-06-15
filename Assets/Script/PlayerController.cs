using UnityEngine;
using UnityEngine.InputSystem; // WAJIB: Masukkan package Input System baru

public class PlayerController : MonoBehaviour
{
    // === TAMBAHAN TUGAS ABSTRAKSI (SCRIPTABLE OBJECT) ===
    [Header("Data Tank (Scriptable Object)")]
    public TankData dataStatistikTank; // Slot untuk masukin file Tank_Biru

    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public Rigidbody2D rb;
    public Transform turret;

    [Header("Economy System")]
    public int totalCoins = 0;

    // === BALANCING: ANTI SPAM PELURU (ALA ENTER THE GUNGEON) ===
    [Header("Shooting Balancing")]
    public float fireRate = 0.5f; // Jeda waktu antar tembakan (dalam detik)
    private float nextFireTime = 0f; // Menghitung kapan player boleh nembak lagi

    // === VARIABEL BARU INTEGRASI NEW INPUT SYSTEM ===
    [Header("New Input System Reference")]
    public PlayerInput playerInput; // Tempat mendeteksi komponen Player Input
    private InputAction moveAction;

    Vector2 movement;
    Vector2 mousePos;

    void Awake()
    {
        // OTOMATIS: Mencari komponen PlayerInput yang nempel di badannya sendiri saat game di-Play
        if (playerInput == null)
        {
            playerInput = GetComponent<PlayerInput>();
        }

        // Hubungkan dengan Action bernama "Move" (WASD / Arrow Keys) dari Input Action Asset lo
        if (playerInput != null)
        {
            moveAction = playerInput.actions["Move"];
        }
    }

    void Start()
    {
        // === PENERAPAN ABSTRAKSI DATA KE DALAM GAME ===
        if (dataStatistikTank != null)
        {
            moveSpeed = dataStatistikTank.speedGerak;

            // Mengambil data Fire Rate dari ScriptableObject untuk balancing anti-spam!
            fireRate = dataStatistikTank.fireRate;

            Debug.Log($"[ABSTRAKSI] Data {dataStatistikTank.namaTank} dimuat! Speed: {moveSpeed} | Fire Rate: {fireRate}s");

            // Nyambungin darah ke script Health lu
            Ilumisoft.HealthSystem.Health healthSystem = GetComponent<Ilumisoft.HealthSystem.Health>();
            if (healthSystem != null)
            {
                healthSystem.MaxHealth = dataStatistikTank.maxHealth;
                healthSystem.SetHealth(dataStatistikTank.maxHealth);
            }
        }

        // PERBAIKAN: Load data koin yang tersimpan di memori saat game dimulai
        totalCoins = PlayerPrefs.GetInt("TotalKoin", 0);
    }

    void Update()
    {
        // INPUT JALAN (NEW INPUT SYSTEM)
        if (moveAction != null)
        {
            movement = moveAction.ReadValue<Vector2>();
        }
        else
        {
            movement = Vector2.zero;
        }

        // Ambil posisi mouse
        if (Camera.main != null)
        {
            mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
    }

    [Header("Smooth Settings")]
    public float rotationSpeed = 10f;

    void FixedUpdate()
    {
        if (rb == null) return;

        // 1. Gerak badan tank
        rb.MovePosition(rb.position + movement.normalized * moveSpeed * Time.fixedDeltaTime);

        // 2. Rotasi BADAN tank
        if (movement != Vector2.zero)
        {
            float targetAngle = Mathf.Atan2(movement.y, movement.x) * Mathf.Rad2Deg - 90f;
            float smoothAngle = Mathf.LerpAngle(rb.rotation, targetAngle, rotationSpeed * Time.fixedDeltaTime);
            rb.rotation = smoothAngle;
        }

        // 3. Rotasi TURRET mengikuti Mouse
        if (turret != null)
        {
            Vector2 lookDir = mousePos - rb.position;
            float turretAngle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg - 90f;
            turret.rotation = Quaternion.Euler(0, 0, turretAngle);
        }
    }

    // === FUNGSI BARU: CEK APAKAH PLAYER BOLEH NEMBAK (ANTI-SPAM) ===
    // Panggil fungsi ini di script menembak lu sebelum men-clone prefab peluru!
    public bool CanShootNow()
    {
        if (Time.time >= nextFireTime)
        {
            // Setel jadwal kapan player baru boleh nembak lagi ke depan
            nextFireTime = Time.time + fireRate;
            return true; // Boleh nembak
        }

        return false; // Masih cooldown, dilarang spam!
    }

    // === FUNCTION: MENAMBAH KOIN KE DOMPET (PERMANEN) ===
    public void AddCoins(int amount)
    {
        totalCoins += amount;
        PlayerPrefs.SetInt("TotalKoin", totalCoins);
        PlayerPrefs.Save();
    }
}