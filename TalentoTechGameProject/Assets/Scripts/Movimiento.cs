using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D), typeof(Animator))]
public class Movimiento : MonoBehaviour
{
    [Header("Configuración Básica")]
    [SerializeField] private float velocidad = 5f;
    [SerializeField] private float velocidadCorrer = 8f;
    [SerializeField] private float fuerzaSalto = 12f;
    [SerializeField] private string tagSuelo = "Suelo";

    private Rigidbody2D rb;
    private Animator animator;
    private bool mirandoDerecha = true;
    private bool enSuelo = false;
    private bool corriendo = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        float movimiento = Input.GetAxisRaw("Horizontal");

        // Detectar si está corriendo
        corriendo = Input.GetKey(KeyCode.LeftShift);

        // Calcular velocidad actual
        float velocidadActual = corriendo ? velocidadCorrer : velocidad;

        // Movimiento horizontal
        rb.linearVelocity = new Vector2(movimiento * velocidadActual, rb.linearVelocity.y);

        // Animaciones
        animator.SetBool("isWalking", movimiento != 0 && enSuelo && !corriendo);
        animator.SetBool("isRunning", movimiento != 0 && enSuelo && corriendo);
        animator.SetBool("isJumping", !enSuelo);

        // Voltear sprite
        if ((movimiento > 0 && !mirandoDerecha) || (movimiento < 0 && mirandoDerecha))
        {
            Voltear();
        }

        // Saltar
        if (Input.GetButtonDown("Jump") && enSuelo)
        {
            Saltar();
        }
    }

    private void Saltar()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, fuerzaSalto);
        enSuelo = false;
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
