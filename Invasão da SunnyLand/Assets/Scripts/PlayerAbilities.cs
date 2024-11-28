using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAbilities : MonoBehaviour
{
    [SerializeField] private float dashSpeed = 15f; // Velocidade do dash
    [SerializeField] private float dashDuration = 0.2f; // Dura��o do dash
    [SerializeField] private float dashCooldown = 1f; // Tempo de espera entre dashes

    private bool canDash = false;
    public bool isDashing = false; // Indica se est� no meio de um dash
    public bool isCrouching = false; // Indica se o jogador est� agachado

    private bool canDoubleJump = false;
    private bool canShoot = false;
    private string currentProjectile = "Default";
    private float nextDashTime = 0f; // Tempo para permitir o pr�ximo dash

    public GameObject projectilePrefab;
    public GameObject projectile;

    public GameObject homingprojectile;
    public ControllPlayer controllPlayer;

    public Transform shootPoint;
    public float projectileSpeed;

    public float fireRate = 0.5f; // Tempo entre os tiros (em segundos)
    private float nextFireTime = 0f; // Controle de quando o pr�ximo tiro pode acontecer
    private void Awake()
    {
        controllPlayer = GetComponent<ControllPlayer>();
    }

    void Update()
    {
        // Verifica se o bot�o esquerdo do mouse foi pressionado
        if (canShoot && Input.GetMouseButtonDown(0)) // 0 = bot�o esquerdo
        {
            Atirar();
        }
        // Verifica se o Shift esquerdo foi pressionado
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            Dash();
        }
        // Verifica se o bot�o de agachar foi pressionado
        if (Input.GetKeyDown(KeyCode.C))
        {
            StartCrouch();
        }
        else if (Input.GetKeyUp(KeyCode.C))
        {
            StopCrouch();
        }
    }
    public void UnlockAbility(string abilityName)
    {
        switch (abilityName)
        {
            case "Atirar":
                canShoot = true;
                Debug.Log("Habilidade de atirar desbloqueada!");
                break;

            case "Pulo Duplo":
                canDoubleJump = true;
                Debug.Log("Habilidade de pulo duplo desbloqueada!");
                break;

            case "Proj�til Rastreador":
                currentProjectile = "Homing";
                canShoot = true;
                Debug.Log("Proj�til rastreador desbloqueado!");
                break;

            case "Dash":
                canDash = true;
                Debug.Log("Habilidade de Dash desbloqueada!");
                break;

                //case "":
                //     = true;
                //    Debug.Log("Habilidade de  desbloqueada!");
                //    break;

                //default:
                //    Debug.LogWarning("Habilidade desconhecida: " + abilityName);
                //    break;
        }
    }
    private void Atirar()
    {
        // Verifica se j� passou o tempo necess�rio para o pr�ximo tiro
        if (Time.time >= nextFireTime)
        {
            // Atualiza o tempo do pr�ximo tiro permitido
            nextFireTime = Time.time + fireRate;

            // Determina a dire��o do disparo com base na orienta��o do jogador
            float shootDirection = transform.localScale.x > 0 ? 1f : -1f;

            switch (currentProjectile)
            {
                case "Homing":
                    projectile = Instantiate(homingprojectile, shootPoint.position, shootPoint.rotation);

                    if (controllPlayer.esquerda)
                    {
                        projectile.GetComponent<SpriteRenderer>().flipX = true;
                    }
                    //else
                    //{
                    //    projectile.GetComponent<SpriteRenderer>().flipX = false;
                    //}
                    break;
                default:
                    // Instancia o proj�til na posi��o correta
                    projectile = Instantiate(projectilePrefab, shootPoint.position, Quaternion.identity);
                    break;
            }

            // Configura a velocidade do proj�til
            Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
            if (rb != null && currentProjectile != "Homing")
            {
                rb.velocity = new Vector2(shootDirection * projectileSpeed, 0); // Movimento horizontal
                Debug.Log(projectileSpeed);
            }

            // Ajusta a escala do proj�til para a dire��o correta
            projectile.transform.localScale = new Vector3(
                Mathf.Abs(projectile.transform.localScale.x) * shootDirection,
                projectile.transform.localScale.y,
                projectile.transform.localScale.z
            );
        }
        else
        {
            Debug.Log("Esperando cooldown para atirar novamente!");
        }
    }

    public bool CanDoubleJump()
    {
        return canDoubleJump; // Retorna se o pulo duplo est� desbloqueado
    }

    private void Dash()
    {
        if (!canDash || Time.time < nextDashTime) return;

        // Marca que o Dash est� em execu��o
        isDashing = true;

        // Define o pr�ximo Dash permitido
        nextDashTime = Time.time + dashCooldown;

        // Salva o valor atual da gravidade e desativa temporariamente
        float originalGravity = controllPlayer.corpoPlayer.gravityScale;
        controllPlayer.corpoPlayer.gravityScale = 0f;

        // Movimenta��o direta (simula Dash sem usar f�sica)
        float dashDirection = controllPlayer.esquerda ? -1f : 1f;
        Vector3 dashVector = new Vector3(dashDirection * dashSpeed * dashDuration, 0, 0);
        transform.position += dashVector;

        // Ativa a anima��o de Dash
        controllPlayer.animacaoPlayer.SetTrigger("rolar");

        // Espera a anima��o come�ar antes de aplicar a for�a
        StartCoroutine(EndDash(originalGravity));
    }

    private bool IsGrounded()
    {
        // Substitua por sua l�gica de verifica��o de ch�o
        return controllPlayer.corpoPlayer.velocity.y == 0 || Physics2D.Raycast(transform.position, Vector2.down, 0.1f);
    }

    private IEnumerator EndDash(float originalGravity)
    {
        yield return new WaitForSeconds(dashDuration);

        isDashing = false; // Reseta o estado de Dash
        controllPlayer.corpoPlayer.gravityScale = originalGravity; // Restaura gravidade
        controllPlayer.animacaoPlayer.SetBool("Andando", Mathf.Abs(controllPlayer.velocidadePlayer) > 0);
    }
    private void StartCrouch()
    {
        if (isCrouching) return; // Evita que o m�todo seja chamado repetidamente

        isCrouching = true;
        controllPlayer.animacaoPlayer.SetBool("Crouching", true); // Ativa a anima��o de agachamento

        // Corrigir a escala do personagem
        transform.localScale = new Vector3(1, 1, 1); 
        Debug.Log("Jogador agachou!");
    }

    private void StopCrouch()
    {
        if (!isCrouching) return; // Evita que o m�todo seja chamado repetidamente

        isCrouching = false;
        controllPlayer.animacaoPlayer.SetBool("Crouching", false); // Desativa a anima��o de agachamento
                                                                 
        transform.localScale = new Vector3(1, 1, 1); 
        Debug.Log("Jogador parou de agachar!");
    }
}
