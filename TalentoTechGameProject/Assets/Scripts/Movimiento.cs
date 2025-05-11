using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
public class Movimiento : MonoBehaviour
{
    [Header("Configuración Básica")]
    [SerializeField] private float velocidad = 5f;
    [SerializeField] private float fuerzaSalto = 12f;
    [SerializeField] private string tagSuelo = "Suelo";

    private Rigidbody2D rb;
    private bool mirandoDerecha = true;
    private bool enSuelo = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (Input.GetButtonDown("Jump") && enSuelo)
        {
            Saltar();
        }
    }

    private void FixedUpdate()
    {
        float movimiento = Input.GetAxisRaw("Horizontal");
        rb.linearVelocity = new Vector2(movimiento * velocidad, rb.linearVelocity.y);

        if ((movimiento > 0 && !mirandoDerecha) || (movimiento < 0 && mirandoDerecha))
        {
            Voltear();
        }
    }

    private void Saltar()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, fuerzaSalto);
        enSuelo = false; // Evita dobles saltos
    }

    private void Voltear()
    {
        mirandoDerecha = !mirandoDerecha;
        Vector3 escala = transform.localScale;
        escala.x *= -1;
        transform.localScale = escala;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag(tagSuelo))
        {
            enSuelo = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag(tagSuelo))
        {
            enSuelo = false;
        }
    }
}
