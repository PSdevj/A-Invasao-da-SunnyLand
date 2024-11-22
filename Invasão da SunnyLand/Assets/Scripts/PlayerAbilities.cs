using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAbilities : MonoBehaviour
{
    private bool canDoubleJump = false;
    private bool canShoot = false;

    public GameObject projectilePrefab;

    public Transform shootPoint;
    public float projectileSpeed;

    public float fireRate = 0.5f; // Tempo entre os tiros (em segundos)
    private float nextFireTime = 0f; // Controle de quando o pr�ximo tiro pode acontecer
    // Update is called once per frame
    void Update()
    {
        // Verifica se o bot�o esquerdo do mouse foi pressionado
        if (canShoot && Input.GetMouseButtonDown(0)) // 0 = bot�o esquerdo
        {
            Atirar();
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

                /*case "":
                     = true;
                    Debug.Log("Habilidade de  desbloqueada!");
                    break;

                case "":
                     = true;
                    Debug.Log("Habilidade de  desbloqueada!");
                    break;

                case "":
                     = true;
                    Debug.Log("Habilidade de  desbloqueada!");
                    break;

                default:
                    Debug.LogWarning("Habilidade desconhecida: " + abilityName);
                    break;*/
        }
    }
    private void Atirar()
    {
        //Verifica se o player est� virado para a direita
        if(transform.localScale.x > 0)
        {
            // Verifica se j� passou o tempo necess�rio para o pr�ximo tiro
            if (Time.time >= nextFireTime)
            {
                // Atualiza o tempo do pr�ximo tiro permitido
                nextFireTime = Time.time + fireRate;

                // Instanciar o proj�til
                GameObject projectile = Instantiate(projectilePrefab, shootPoint.position, shootPoint.rotation);

                // Adicionar movimento ao proj�til
                Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    Vector2 shootDirection = shootPoint.right; // Dire��o do disparo
                    rb.velocity = shootDirection * projectileSpeed;
                }
                else
                {
                    Debug.LogWarning("O proj�til n�o possui um Rigidbody2D!");
                }
            }
            else
            {
                Debug.Log("Esperando cooldown para atirar novamente!");
            }
        }
        else
        {
            Debug.Log("N�o pode atirar porque o jogador est� virado para a esquerda");
        }
    }
    public bool CanDoubleJump()
    {
        return canDoubleJump; // Retorna se o pulo duplo est� desbloqueado
    }
}
