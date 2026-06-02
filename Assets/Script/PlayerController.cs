using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public Rigidbody2D rb;
    public Transform turret; 

    // KOREKSI LOGIKA: Pengaturan Shooting dihapus dari sini karena sudah dipindah 
    // ke script TankShooting.cs di objek Turret agar tidak dobel nembak!

    [Header("Economy System")]
    public int totalCoins = 0; 

    Vector2 movement;
    Vector2 mousePos;

    void Start()
    {
        // PERBAIKAN: Load data koin yang tersimpan di memori saat game dimulai
        totalCoins = PlayerPrefs.GetInt("TotalKoin", 0);
        Debug.Log("Koin berhasil di-load! Jumlah sekarang: " + totalCoins);
    }

    void Update()
    {
        // Input jalan (WASD)
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        // Ambil posisi mouse
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    [Header("Smooth Settings")]
    public float rotationSpeed = 10f; 

    void FixedUpdate()
    {
        // 1. Gerak badan tank
        rb.MovePosition(rb.position + movement.normalized * moveSpeed * Time.fixedDeltaTime);

        // 2. Rotasi BADAN tank (Smooth Rotation)
        if (movement != Vector2.zero)
        {
            float targetAngle = Mathf.Atan2(movement.y, movement.x) * Mathf.Rad2Deg - 90f;
            float smoothAngle = Mathf.LerpAngle(rb.rotation, targetAngle, rotationSpeed * Time.fixedDeltaTime);
            rb.rotation = smoothAngle;
        }

        // 3. Rotasi TURRET mengikuti Mouse
        Vector2 lookDir = mousePos - rb.position;
        float turretAngle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg - 90f;
        turret.rotation = Quaternion.Euler(0, 0, turretAngle);
    }

    // === FUNCTION: MENAMBAH KOIN KE DOMPET (PERMANEN) ===
    public void AddCoins(int amount)
    {
        totalCoins += amount;

        // PERBAIKAN: Kunci koin ke memori lokal laptop biar gak hilang saat pindah scene
        PlayerPrefs.SetInt("TotalKoin", totalCoins);
        PlayerPrefs.Save();

        Debug.Log($"<color=#FFD700><b>[WALLET]</b> Koin Tersimpan! +{amount} | Total di Memori: {totalCoins} Koin</color>");
    }
}