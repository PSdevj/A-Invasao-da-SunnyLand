using System.Collections;
using UnityEngine;
using UnityEngine.Video;

[RequireComponent(typeof(Animator), typeof(Rigidbody2D))]
public class TrollController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float moveSpeed = 3f;       // Velocidade do Boss
    [SerializeField] float attackRange = 2f;    // Dist�ncia para atacar o Player
    [SerializeField] float detectionRange = 10f; // Dist�ncia para come�ar a seguir o Player
    [SerializeField] float attackCooldown = 2f; // Tempo entre ataques
    [SerializeField] Transform player;          // Refer�ncia ao Player
    [SerializeField] float jumpForce = 10f;  // For�a do pulo
    [SerializeField] float jumpHorizontalForce = 5f;  // For�a horizontal ao pular

    public bool isGrounded = false; // Verifica se o boss est� no ch�o

    private Animator animator;
    private Rigidbody2D rb;
    private bool isAttacking = false;
    private float attackTimer = 0f;
    private BossHealth bossHealth; // Refer�ncia ao script BossHealth

    enum State { Idle, Chase, Attack }
    State currentState = State.Idle;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        bossHealth = GetComponent<BossHealth>(); // Busca o BossHealth no mesmo GameObject

        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform; // Procura o player automaticamente
        }
    }

    private void Update()
    {
        // Verifica se o boss est� morto
        if (bossHealth != null && bossHealth.isDead)
        {
            rb.velocity = Vector2.zero; // Para qualquer movimento
            return; // N�o executa mais l�gica
        }

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        switch (currentState)
        {
            case State.Idle:
                IdleState(distanceToPlayer);
                break;
            case State.Chase:
                ChaseState(distanceToPlayer);
                break;
            case State.Attack:
                AttackState(distanceToPlayer);
                break;
        }

        if (attackTimer > 0)
        {
            attackTimer -= Time.deltaTime;
        }
    }

    #region States

    private void IdleState(float distanceToPlayer)
    {
        animator.Play("idle1_Troll");

        if (distanceToPlayer <= detectionRange)
        {
            currentState = State.Chase;
        }
    }

    private void ChaseState(float distanceToPlayer)
    {
        animator.Play("walk_Troll");

        // Movimenta o boss em dire��o ao player
        Vector2 direction = (player.position - transform.position).normalized;
        rb.velocity = new Vector2(direction.x * moveSpeed, rb.velocity.y);

        // Flipar a sprite para olhar na dire��o do player
        if (direction.x > 0)
        {
            transform.localScale = new Vector3(10.62414f, 10.62414f, 10.62414f);
        }
        else
        {
            transform.localScale = new Vector3(-10.62414f, 10.62414f, 10.62414f);
        }

        // Verifica se est� na dist�ncia de ataque
        if (distanceToPlayer <= attackRange)
        {
            rb.velocity = Vector2.zero; // Para de se mover ao alcan�ar o player
            currentState = State.Attack;
        }
    }

    private void AttackState(float distanceToPlayer)
    {
        if (!isAttacking && attackTimer <= 0)
        {
            isAttacking = true;
            rb.velocity = Vector2.zero; // Garante que o boss fique parado durante o ataque
            animator.SetTrigger("Attack");

            // Calcula a dire��o do pulo em dire��o ao jogador
            Vector2 directionToPlayer = (player.position - transform.position).normalized;
            Vector2 jumpVelocity = new Vector2(directionToPlayer.x * jumpHorizontalForce, jumpForce);

            // Aplica o impulso ao Rigidbody2D
            rb.velocity = jumpVelocity;

            // Reduz a vida do jogador (chama o script do jogador diretamente)
            //if (player != null) // Confirma se o jogador existe
            //{
            //    //Forma de dano na anima��o
            //}

            attackTimer = attackCooldown;
            StartCoroutine(EndAttack());
        }
        else
        {
            attackTimer -= Time.deltaTime; // Decrementa o cooldown
        }

        // Volta a seguir o player se ele sair do alcance de ataque
        if (distanceToPlayer > attackRange)
        {
            currentState = State.Chase;
        }
    }

    #endregion

    #region Ataque

    private IEnumerator EndAttack()
    {
        yield return new WaitForSeconds(1f); // Tempo suficiente para o pulo/ataque completar
        isAttacking = false; // Permite novos ataques
        currentState = State.Chase; // Retorna para o estado de persegui��o
    }

    #endregion

    private void OnDrawGizmosSelected()
    {
        // Mostra as �reas de alcance no editor
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ch�o"))
        {
            isGrounded = true; // Define que est� no ch�o
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ch�o"))
        {
            isGrounded = false; // N�o est� mais no ch�o
        }
    }


    public void ExecuteJump()
    {
        Vector2 directionToPlayer = (player.position - transform.position).normalized;
        Vector2 jumpVelocity = new Vector2(directionToPlayer.x * jumpHorizontalForce, jumpForce);
        rb.velocity = jumpVelocity;
    }
}
