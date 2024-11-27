using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjetilPlayer : MonoBehaviour
{
    public float speed;
    public float lifetime = 5f; // Tempo antes do proj�til desaparecer
    public int damage = 1; // Dano causado pelo proj�til
    private void Start()
    {
        Destroy(gameObject, lifetime); // Destroi o proj�til ap�s o tempo definido
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Verifica se atingiu o boss
        if (collision.CompareTag("Boss"))
        {
            BossHealth bossHealth = collision.GetComponent<BossHealth>();
            if (bossHealth != null)
            {
                bossHealth.TakeDamage(damage);
            }

            // Destroi o proj�til ap�s o impacto
            Destroy(gameObject);
        }
    }
}

