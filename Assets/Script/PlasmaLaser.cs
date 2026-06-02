using UnityEngine;

// KOREKSI UTAMA: Wajib pakai ': MonoBehaviour' agar bisa ditempel ke prefab Unity
public class PlasmaLaser : MonoBehaviour
{
    public int damage = 1;
    public float laserRange = 20f; // Panjang maksimal sinar laser
    public LayerMask hitLayers;    // Pilih layer Enemy, Boss, dan Obstacles di Inspector

    [Header("Visual Effects")]
    public GameObject impactExplosionPrefab;
    public LineRenderer lineRenderer; // Pasang komponen LineRenderer di objek ini

    [Header("Audio Effects")]
    public AudioClip shootSoundClip; // <-- TAMBAHKAN INI (Isi dengan suara laser menembak)
    public AudioClip impactSoundClip;

    void Start()
    {
        // BIKIN BUNYI PAS LASER KELUAR
        if (shootSoundClip != null)
        {
            AudioSource.PlayClipAtPoint(shootSoundClip, transform.position);
        }

        if (lineRenderer == null) lineRenderer = GetComponent<LineRenderer>();
        ExecuteLaserCast();
        Destroy(gameObject, 0.1f);
    }

    void ExecuteLaserCast()
    {
        // Temukan arah depan berdasarkan rotasi objek
        Vector2 laserDirection = transform.up;
        Vector2 startPos = transform.position;
        Vector2 endPos = startPos + (laserDirection * laserRange);

        // Tembakkan garis imajiner instan untuk mendeteksi tabrakan
        RaycastHit2D hit = Physics2D.Raycast(startPos, laserDirection, laserRange, hitLayers);

        if (hit.collider != null)
        {
            // Jika kena sesuatu, batasi ujung visual laser tepat di titik tabrakan
            endPos = hit.point;

            // === CEK TARGET: Enemy atau Boss ===
            if (hit.collider.CompareTag("Enemy") || hit.collider.CompareTag("Boss"))
            {
                Health enemyHealth = hit.collider.GetComponent<Health>();
                if (enemyHealth == null) enemyHealth = hit.collider.GetComponentInParent<Health>();

                if (enemyHealth != null)
                {
                    // KOREKSI: Set tipe damage ke "laser" dulu baru kurangi darah agar audionya sesuai
                    enemyHealth.SetLastDamageType("laser");
                    enemyHealth.ApplyDamage(damage);
                }
            }

            // Munculkan efek ledakan di titik tabrakan laser
            if (impactExplosionPrefab != null)
            {
                Instantiate(impactExplosionPrefab, hit.point, Quaternion.identity);
            }

            // Mainkan suara ledakan di titik tabrakan
            if (impactSoundClip != null)
            {
                AudioSource.PlayClipAtPoint(impactSoundClip, hit.point);
            }
        }

        // Gambar visual garis lasernya dari moncong ke titik ujung/tabrakan
        if (lineRenderer != null)
        {
            lineRenderer.positionCount = 2;
            lineRenderer.SetPosition(0, startPos);
            lineRenderer.SetPosition(1, endPos);
        }
    }
}