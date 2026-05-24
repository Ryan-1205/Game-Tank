using UnityEngine;

public class DestroyAfterTime : MonoBehaviour
{
    // Waktu tunggu dalam detik sebelum objek ini dihapus
    public float timeToDestroy = 0.5f; 

    void Start()
    {
        // Hancurkan objek efek ini otomatis sesuai waktu di atas
        Destroy(gameObject, timeToDestroy);
    }
}