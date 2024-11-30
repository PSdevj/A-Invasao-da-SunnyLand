using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class BossHealth : MonoBehaviour
{
    [SerializeField] public int maxHealth = 10;
    public int currentHealth;

    private Animator animator;
    public bool isDead { get; private set; } = false; // Propriedade somente leitura

    public Slider barraDeVidaBoss;

    public SpriteRenderer spriteRenderer; // Refer�ncia ao SpriteRenderer

    private Color corOriginal;
    void Start()
    {
        if (barraDeVidaBoss == null)
        {
            barraDeVidaBoss = FindObjectOfType<Slider>();
            Debug.LogError("Barra de Vida do Boss n?o foi atribu?da no Inspector!");
            return;
        }

        // Inicializa o SpriteRenderer e salva a cor original
        spriteRenderer = GetComponent<SpriteRenderer>();
        corOriginal = spriteRenderer.color;

        currentHealth = maxHealth;
        barraDeVidaBoss.maxValue = currentHealth;
        barraDeVidaBoss.value = currentHealth;

        animator = GetComponent<Animator>(); // Refer�ncia ao Animator
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return; // Evita tomar dano ap�s a morte

        currentHealth -= damage;
        barraDeVidaBoss.value = currentHealth;
        StartCoroutine(MudarCorTemporariamente());
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
        animator.SetTrigger("Die"); // Garante a transi��o
        Debug.Log("Boss entrou no estado de morte");

        // Destruir o boss ap�s a anima��o
        StartCoroutine(DestroyAfterDeath());
    }
    private IEnumerator DestroyAfterDeath()
    {
        yield return new WaitForSeconds(1f); // Tempo para a anima��o de morte
        Destroy(gameObject); // Remove o boss da cena
    }
    private IEnumerator MudarCorTemporariamente()
    {
        // Muda a cor para vermelho
        spriteRenderer.color = Color.red;

        // Espera 0.2 segundos
        yield return new WaitForSeconds(0.2f);

        // Volta para a cor original
        spriteRenderer.color = corOriginal;
    }
}

