using UnityEngine;

public class TankEngineAudio : MonoBehaviour
{
    private AudioSource audioSource;

    [Header("Pengaturan Volume")]
    public float minVolume = 0.2f;  // Volume saat tank diam (idle)
    public float maxVolume = 0.6f;  // Volume saat tank jalan maksimal

    [Header("Pengaturan Nada (Pitch)")]
    public float minPitch = 0.8f;   // Suara berat saat diam
    public float maxPitch = 1.3f;   // Suara melengking saat ngebut

    [Header("Kecepatan Transisi Audio")]
    public float lerpSpeed = 5f;    // Seberapa cepat suara berubah (makin gede makin instan)

    private float targetRatio = 0f;
    private float currentRatio = 0f;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        // Pastikan Loop menyala agar suara tidak berhenti
        audioSource.loop = true;
        audioSource.volume = minVolume;
        audioSource.pitch = minPitch;
        audioSource.Play();
    }

    void Update()
    {
        // 1. Ambil input WASD / Panah (nilainya antara -1 sampai 1)
        float moveInputX = Input.GetAxisRaw("Horizontal"); // A/D atau Kiri/Kanan
        float moveInputY = Input.GetAxisRaw("Vertical");   // W/S atau Atas/Bawah

        // 2. Cek apakah player lagi mencet tombol arah atau enggak
        if (moveInputX != 0 || moveInputY != 0)
        {
            // Jika ditekan, target rasio ke 1 (maksimal)
            targetRatio = 1f;
        }
        else
        {
            // Jika dilepas, target rasio ke 0 (diam/idle)
            targetRatio = 0f;
        }

        // 3. Gabungkan transisi halus menggunakan Lerp dari rasio sekarang ke target rasio
        currentRatio = Mathf.MoveTowards(currentRatio, targetRatio, lerpSpeed * Time.deltaTime);

        // 4. Terapkan nilai ke Volume dan Pitch
        audioSource.volume = Mathf.Lerp(minVolume, maxVolume, currentRatio);
        audioSource.pitch = Mathf.Lerp(minPitch, maxPitch, currentRatio);
    }
}