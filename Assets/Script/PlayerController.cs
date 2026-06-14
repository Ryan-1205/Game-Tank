using UnityEngine;
using UnityEngine.InputSystem; // WAJIB: Menggunakan package Input System baru

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public Rigidbody2D rb;
    public Transform turret; 

    [Header("Shooting Settings")]
    public GameObject bulletPrefab; 
    public Transform firePoint;     
    public float bulletForce = 20f;
    public GameObject muzzleFlashPrefab; 

    [Header("Economy System")]
    public int totalCoins = 0; 

    [Header("Smooth Settings")]
    public float rotationSpeed = 10f; 

    // === VARIABEL BARU: Referensi Input Action dari Package ===
    [Header("Input Actions Reference")]
    public InputAction moveAction;
    public InputAction shootAction;

    Vector2 movement;
    Vector2 mousePos;

    private void OnEnable()
    {
        // Mengaktifkan input action saat objek aktif
        moveAction.Enable();
        shootAction.Enable();
    }

    private void OnDisable()
    {
        // Mematikan input action saat objek tidak aktif (mencegah memory leak)
        moveAction.Disable();
        shootAction.Disable();
    }

    void Update()
    {
        // 1. MENGOLAH INPUT JALAN (Value Vector2)
        // Membaca input hardware (WASD / Analog) menggunakan package baru
        movement = moveAction.ReadValue<Vector2>();

        // 2. MEMBACA POSISI MOUSE
        // Menggunakan Pointer bawaan Input System baru untuk akurasi koordinat layar
        if (Pointer.current != null)
        {
            Vector3 screenMousePos = Pointer.current.position.ReadValue();
            mousePos = Camera.main.ScreenToWorldPoint(screenMousePos);
        }

        // 3. DEMONSTRASI PEMAHAMAN INPUT STATE (Up, Down, Hold)
        DemonstrateInputStates();
    }

    // FUNGSI UTAMA: Demonstrasi kemampuan mengolah Input State dari Hardware
    // FUNGSI UTAMA: Mengolah Input State dari Hardware menggunakan InputAction secara benar
    void DemonstrateInputStates()
    {
        // A. STATE: DOWN (Dipicu tepat pada frame saat tombol ditekan klik kiri/space)
        if (shootAction.triggered && shootAction.ReadValue<float>() > 0f)
        {
            Debug.Log("<color=green><b>[INPUT STATE] DOWN:</b> Tombol tembak baru saja ditekan!</color>");
            Shoot(); // Tank menembak satu peluru
        }

        // B. STATE: HOLD (Membaca tombol jika sedang ditahan aktif oleh player)
        // Nilai float > 0 artinya tombol sedang amblas ditekan ke dalam
        if (shootAction.ReadValue<float>() > 0f)
        {
            // Sengaja di-comment biar Console lo ga penuh text spam setiap frame jalan
            // Debug.Log("<color=yellow><b>[INPUT STATE] HOLD:</b> Player sedang menahan tombol tembak...</color>");
        }

        // C. STATE: UP (Dipicu saat dilepas)
        // Dipicu saat action aktif (triggered) tetapi nilai value-nya sudah balik ke 0 (dilepas)
        if (shootAction.triggered && shootAction.ReadValue<float>() == 0f)
        {
            Debug.Log("<color=red><b>[INPUT STATE] UP:</b> Tombol tembak dilepas oleh player.</color>");
        }
    }

    void FixedUpdate()
    {
        // Gerak badan tank (Menggunakan hasil olahan Input System baru)
        rb.MovePosition(rb.position + movement.normalized * moveSpeed * Time.fixedDeltaTime);

        // Rotasi BADAN tank (Smooth Rotation)
        if (movement != Vector2.zero)
        {
            float targetAngle = Mathf.Atan2(movement.y, movement.x) * Mathf.Rad2Deg - 90f;
            float smoothAngle = Mathf.LerpAngle(rb.rotation, targetAngle, rotationSpeed * Time.fixedDeltaTime);
            rb.rotation = smoothAngle;
        }

        // Rotasi TURRET mengikuti Mouse
        Vector2 lookDir = mousePos - rb.position;
        float turretAngle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg - 90f;
        turret.rotation = Quaternion.Euler(0, 0, turretAngle);
    }

    void Shoot()
    {
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
        bulletRb.AddForce(firePoint.up * bulletForce, ForceMode2D.Impulse);

        if (muzzleFlashPrefab != null)
        {
            GameObject flash = Instantiate(muzzleFlashPrefab, firePoint.position, firePoint.rotation);
            flash.transform.SetParent(firePoint);
            Destroy(flash, 0.1f);
        }
    }

    public void AddCoins(int amount)
    {
        totalCoins += amount;
        Debug.Log($"<color=#FFD700><b>[WALLET]</b> Koin Bertambah! +{amount} | Total Dompet: {totalCoins} Koin</color>");
    }
}