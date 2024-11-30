using UnityEngine;

public class BossManager : MonoBehaviour
{
    public BossHealth boss; // Refer�ncia ao script de vida do boss
    public GameObject winObject; // O GameObject de vit�ria

    private void Start()
    {
        // Garante que o objeto de vit�ria est� desativado no in�cio
        if (winObject != null)
        {
            winObject.SetActive(false);
        }
    }

    private void Update()
    {
        // Verifica se o boss est� morto
        if (boss != null && boss.isDead && winObject != null && !winObject.activeSelf)
        {
            // Ativa o GameObject de vit�ria
            winObject.SetActive(true);
            Debug.Log("O boss foi derrotado! Ativando o objeto de vit�ria.");
        }
    }
}
