using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjetilPlayer : MonoBehaviour
{
    public float speed;
    public float lifetime = 5f; // Tempo antes do proj�til desaparecer
    private void Start()
    {
        Destroy(gameObject, lifetime); // Destroi o proj�til ap�s o tempo definido
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector2.right * speed * Time.deltaTime); // Movimenta o proj�til
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // L�gica para impacto
        Debug.Log($"Proj�til atingiu: {collision.gameObject.name}");
        Destroy(gameObject); // Destroi o proj�til ap�s colidir
    }
}
