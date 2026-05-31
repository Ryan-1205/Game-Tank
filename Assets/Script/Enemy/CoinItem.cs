using UnityEngine;

public class CoinItem : MonoBehaviour
{
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
                
                // Efek suara atau partikel bisa ditaruh di sini nanti
                
                Destroy(gameObject); // Hancurkan objek koin dari map
            }
        }
    }
}