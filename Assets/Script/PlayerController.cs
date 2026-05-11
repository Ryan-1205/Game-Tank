using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public Rigidbody2D rb;
    public Transform turret; 

    [Header("Shooting Settings")]
    public GameObject bulletPrefab; // Tarik prefab peluru dari folder Project ke sini
    public Transform firePoint;     // Objek kosong di ujung laras tank
    public float bulletForce = 20f;

    Vector2 movement;
    Vector2 mousePos;

    void Update()
    {
        // Input jalan (WASD)
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        // Ambil posisi mouse
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // Deteksi klik kiri untuk nembak
        if (Input.GetButtonDown("Fire1"))
        {
            Shoot();
        }
    }

    
[Header("Smooth Settings")]
public float rotationSpeed = 10f; // Semakin besar, semakin cepat muternya

void FixedUpdate()
{
    // 1. Gerak badan tank
    rb.MovePosition(rb.position + movement.normalized * moveSpeed * Time.fixedDeltaTime);

    // 2. Rotasi BADAN tank (Smooth Rotation)
    if (movement != Vector2.zero)
    {
        float targetAngle = Mathf.Atan2(movement.y, movement.x) * Mathf.Rad2Deg - 90f;
        
        // Menggunakan LerpAngle supaya transisinya halus dan tidak patah-patah
        float smoothAngle = Mathf.LerpAngle(rb.rotation, targetAngle, rotationSpeed * Time.fixedDeltaTime);
        rb.rotation = smoothAngle;
    }

    // 3. Rotasi TURRET mengikuti Mouse
    Vector2 lookDir = mousePos - rb.position;
    float turretAngle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg - 90f;
    
    // Tips: Turret juga bisa di-lerp kalau mau terasa lebih realistis beratnya
    turret.rotation = Quaternion.Euler(0, 0, turretAngle);
}

    void Shoot()
    {
        // 1. Buat peluru di posisi dan rotasi FirePoint
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        
        // 2. Ambil Rigidbody2D peluru
        Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
        
        // 3. Dorong peluru ke depan (arah atas laras karena rotasi 2D Unity biasanya pake sumbu Y/Up)
        bulletRb.AddForce(firePoint.up * bulletForce, ForceMode2D.Impulse);
    }
}