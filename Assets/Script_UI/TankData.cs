using UnityEngine;

// Baris ini yang bikin kita bisa bikin file datanya langsung lewat klik kanan di Unity
[CreateAssetMenu(fileName = "TankBaru", menuName = "Data Game/Data Tank")]
public class TankData : ScriptableObject
{
    [Header("Info Dasar")]
    public string namaTank;
    public int hargaBeli;
    public Sprite gambarTank; // Buat nyimpen gambar tanknya di Shop

    [Header("Statistik In-Game")]
    public float maxHealth;
    public float speedGerak;
    public float fireRate;
}