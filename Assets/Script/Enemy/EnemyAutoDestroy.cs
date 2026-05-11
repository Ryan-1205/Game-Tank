using UnityEngine;

public class EnemyAutoDestroy : MonoBehaviour
{
    private Transform player;
    public float despawnDistance = 40f; // Hapus jika jaraknya lebih dari 40 unit

    void Start()
    {
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;
    }

    void Update()
    {
        if (player != null)
        {
            float distance = Vector2.Distance(transform.position, player.position);
            if (distance > despawnDistance)
            {
                Destroy(gameObject);
            }
        }
    }
}