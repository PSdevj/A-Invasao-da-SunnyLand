using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAbilities : MonoBehaviour
{
    private bool canDoubleJump = false;
    private bool canShoot = false;
    private string currentProjectile = "Default";

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

            case "Proj�til Rastreador":
                currentProjectile = "Homing";
                Debug.Log("Proj�til rastreador desbloqueado!");
                break;

                /*case "":
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
        // Verifica se j� passou o tempo necess�rio para o pr�ximo tiro
        if (Time.time >= nextFireTime)
        {
            // Atualiza o tempo do pr�ximo tiro permitido
            nextFireTime = Time.time + fireRate;

            // Determina a dire��o do disparo com base na orienta��o do jogador
            float shootDirection = transform.localScale.x > 0 ? 1f : -1f;

            // Instancia o proj�til na posi��o correta
            GameObject projectile = Instantiate(projectilePrefab, shootPoint.position, Quaternion.identity);

            switch (currentProjectile)
            {
                case "Homing":
                    projectile = Instantiate(Resources.Load<GameObject>("HomingProjectile"), shootPoint.position, shootPoint.rotation);
                    break;
            }

            // Configura a velocidade do proj�til
            Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.velocity = new Vector2(shootDirection * projectileSpeed, 0); // Movimento horizontal
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
}
