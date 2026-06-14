using UnityEngine;

public class CoinItem : MonoBehaviour
{
    // Slot untuk memasukkan file audio (.mp3 / .wav) koin di Inspector Prefab
    public AudioClip suaraKoinCling;

    public int coinValue = 100; // Jumlah duit yang didapat per koin

    void Start()
    {
        // Fail-safe: Supaya kalau koin gak diambil, gak menumpuk bikin lag game. Hancur otomatis dalam 15 detik.
        Destroy(gameObject, 15f);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Cek apakah yang menabrak koin adalah Player
        if (other.CompareTag("Player"))
        {
            // Panggil fungsi tambah duit di Player Controller
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                player.AddCoins(coinValue);
                
                // BARU & LEBIH KENCENG: Putar suara 2D lewat AudioSource milik Tank Player
                AudioSource playerAudio = other.GetComponent<AudioSource>();
                if (playerAudio != null && suaraKoinCling != null)
                {
                    // Angka 2.0f di bawah ini adalah booster volumenya! (2 kali lipat lebih kenceng dari normal)
                    // Kalau masih kurang kenceng, lu bisa naikin jadi 2.5f atau 3.0f sesuai selera lu.
                    playerAudio.PlayOneShot(suaraKoinCling, 4.0f); 
                }
                
                Destroy(gameObject); // Hancurkan objek koin dari map
            }
        }
    }
}