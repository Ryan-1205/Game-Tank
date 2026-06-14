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
                
                // KOREKSI: Mainkan suara koin tepat di posisi koin berada sebelum objeknya hancur
                if (suaraKoinCling != null)
                {
                    AudioSource.PlayClipAtPoint(suaraKoinCling, transform.position);
                }
                
                Destroy(gameObject); // Hancurkan objek koin dari map
            }
        }
    }
}