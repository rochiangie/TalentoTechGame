using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator), typeof(Collider2D))]
public class MovimientoJugador : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private float velocidadCaminata = 5f;
    [SerializeField] private float velocidadCarrera = 8f;
    [SerializeField] private float fuerzaSalto = 12f;
    [SerializeField] private string tagSuelo = "Suelo";
    [SerializeField] private string tagMuerte = "Muerte";

    private Rigidbody2D rb;
    private Animator animator;
    private bool enSuelo = false;
    private bool estaMuerto = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (estaMuerto) return;

        // Salto
        if (Input.GetButtonDown("Jump") && enSuelo)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, fuerzaSalto);
        }

        // Animación de salto
        animator.SetBool("isJumping", !enSuelo);
    }

    private void FixedUpdate()
    {
        if (estaMuerto) return;

        // Movimiento horizontal
        float inputX = Input.GetAxisRaw("Horizontal");
        bool corriendo = Input.GetKey(KeyCode.LeftShift);
        float velocidad = corriendo ? velocidadCarrera : velocidadCaminata;

        rb.linearVelocity = new Vector2(inputX * velocidad, rb.linearVelocity.y);

        // Animación de caminata
        animator.SetBool("isWalking", inputX != 0);

        // Voltear sprite
        if (inputX != 0)
        {
            transform.localScale = new Vector3(Mathf.Sign(inputX), 1f, 1f);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag(tagSuelo))
        {
            enSuelo = true;
        }

        if (collision.collider.CompareTag(tagMuerte))
        {
            Morir();
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag(tagSuelo))
        {
            enSuelo = false;
        }
    }

    private void Morir()
    {
        if (estaMuerto) return;

        estaMuerto = true;
        animator.SetBool("isDead", true);
        rb.linearVelocity = Vector2.zero;
        rb.isKinematic = true;
        GetComponent<Collider2D>().enabled = false;

        Invoke("ReiniciarNivel", 1.5f);
    }

    private void ReiniciarNivel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
