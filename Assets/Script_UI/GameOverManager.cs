using UnityEngine;
using UnityEngine.SceneManagement; // Wajib untuk fitur restart/pindah scene

public class GameOverManager : MonoBehaviour
{
    [Header("UI Game Over")]
    public GameObject panelGameOver; // Tempat naruh Panel_GameOver dari Canvas lu

    void Start()
    {
        // Pas game baru mulai, pastiin panel Game Over-nya tersembunyi
        if (panelGameOver != null)
        {
            panelGameOver.SetActive(false);
        }
    }

    // Fungsi utama untuk memunculkan pop-up saat player mati
    public void MunculkanGameOver()
    {
        if (panelGameOver != null)
        {
            panelGameOver.SetActive(true); // Memunculkan design Photoshop lu
            Time.timeScale = 0f;           // Menghentikan waktu game (Freeze/Pause)
        }
    }

    // Fungsi untuk Tombol Restart
    public void TombolRestart()
    {
        Time.timeScale = 1f; // WAJIB! Mengembalikan waktu game ke normal sebelum restart
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Mengulang scene yang aktif sekarang
    }

    // Fungsi untuk Tombol Kembali ke Menu
    public void TombolMainMenu()
    {
        Time.timeScale = 1f; // WAJIB! Mengembalikan waktu game ke normal
        SceneManager.LoadScene("Menu"); // Ubah "MainMenu" sesuai nama scene menu lu
    }
}