using UnityEngine;
using System.Collections;

public class RamboBurstBullet : MonoBehaviour
{
    [Header("Rambo Ramboan Settings")]
    public int jumlahTembakanBeruntun = 4; 
    public float jedaAntarPeluru = 0.05f;  
    public float speedPeluru = 12f;        

    [Header("Audio Effects")]
    public AudioClip shootSoundClip;

    private Rigidbody2D rb;
    [HideInInspector] public bool isClone = false; 
    [HideInInspector] public Transform moncongTank; // Tempat lahir peluru anak berikutnya

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        
        if (rb != null)
        {
            rb.linearVelocity = transform.up * speedPeluru;
        }

        // Hanya peluru utama yang memicu rentetan peluru anak di moncong
        if (!isClone)
        {
            PlayShootSound();
            StartCoroutine(MulaiAksiRambo());
        }

        Destroy(gameObject, 3f);
    }

    private IEnumerator MulaiAksiRambo()
    {
        // Mulai dari i = 1 karena peluru pertama (dirinya sendiri) sudah keluar
        for (int i = 1; i < jumlahTembakanBeruntun; i++)
        {
            yield return new WaitForSeconds(jedaAntarPeluru);

            // Tentukan posisi lahir. Jika tank bergerak, ikuti moncongnya. Jika hancur, pakai posisi terakhir.
            Vector3 spawnPos = (moncongTank != null) ? moncongTank.position : transform.position;
            Quaternion spawnRot = (moncongTank != null) ? moncongTank.rotation : transform.rotation;

            // Gandakan prefab asli ini di moncong tank
            GameObject peluruAnak = Instantiate(gameObject, spawnPos, spawnRot);
            
            RamboBurstBullet scriptAnak = peluruAnak.GetComponent<RamboBurstBullet>();
            if (scriptAnak != null)
            {
                scriptAnak.isClone = true; 
            }

            PlayShootSound();

            Rigidbody2D rbAnak = peluruAnak.GetComponent<Rigidbody2D>();
            if (rbAnak != null)
            {
                // Gunakan arah hadap moncong terbaru agar peluru mengikuti rotasi tank
                Vector2 arahTembak = (moncongTank != null) ? moncongTank.up : transform.up;
                rbAnak.linearVelocity = arahTembak * speedPeluru;
            }
        }
    }

    void PlayShootSound()
    {
        if (shootSoundClip != null)
        {
            // Tambahkan , 0.4f di ujung kurung buat ngecilin rentetan suara rambo
            AudioSource.PlayClipAtPoint(shootSoundClip, transform.position, 0.4f);
        }
    }

    void OnTriggerEnter2D(Collider2D hitInfo)
    {
        if (hitInfo.CompareTag("Player") || hitInfo.CompareTag("Bullet")) return;

        if (hitInfo.CompareTag("Enemy") || hitInfo.CompareTag("Boss"))
        {
            Health enemyHealth = hitInfo.GetComponent<Health>();
            if (enemyHealth == null) enemyHealth = hitInfo.GetComponentInParent<Health>();

            if (enemyHealth != null)
            {
                enemyHealth.SetLastDamageType("normal");
                enemyHealth.ApplyDamage(1); 
            }

            Destroy(gameObject); 
        }
        else if (hitInfo.gameObject.layer == LayerMask.NameToLayer("Obstacles"))
        {
            Destroy(gameObject); 
        }
    }
}