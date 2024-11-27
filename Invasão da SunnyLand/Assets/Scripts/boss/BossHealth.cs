using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth = 10;
    private int currentHealth;

    private Animator animator;
    public bool isDead { get; private set; } = false; // Propriedade somente leitura

    private void Awake()
    {
        currentHealth = maxHealth; // Define a vida inicial
        animator = GetComponent<Animator>(); // Refer�ncia ao Animator
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return; // Evita tomar dano ap�s a morte

        currentHealth -= damage;
        Debug.Log($"Boss tomou {damage} de dano. Vida restante: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (isDead) return; // Evita m�ltiplas chamadas para morrer

        isDead = true;
        animator.Play("death_Wolf"); // Toca a anima��o de morte
        Debug.Log("Boss derrotado!");

        // Destruir o boss ap�s a anima��o
        StartCoroutine(DestroyAfterDeath());
    }
    private IEnumerator DestroyAfterDeath()
    {
        yield return new WaitForSeconds(1f); // Tempo para a anima��o de morte
        Destroy(gameObject); // Remove o boss da cena
    }
}

