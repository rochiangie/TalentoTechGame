using UnityEngine;
using TMPro;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator))]
public class InteraccionJugador : MonoBehaviour
{
    [Header("Movimiento")]
    public float velocidadMovimiento = 5f;
    public float velocidadCorrer = 8f;
    public KeyCode teclaCorrer = KeyCode.LeftShift;

    [Header("Interacción")]
    public float rango = 1.5f;
    public LayerMask capaInteractuable;
    public KeyCode teclaInteraccion = KeyCode.E;

    [Header("UI")]
    public TextMeshProUGUI mensajeUI;

    private Rigidbody2D rb;
    private Animator animator;
    private Vector2 input;
    private ControladorEstados objetoInteractuable;

    private InteraccionSilla sillaCercana;
    private bool estaDeslizandoEscalon = false;
    public float velocidadEscalon = 2f;
    public Vector2 direccionEscalon = new Vector2(1f, 1f).normalized;
    private bool enSuelo = true;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // Movimiento
        input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        if (estaDeslizandoEscalon)
        {
            rb.linearVelocity = direccionEscalon * velocidadEscalon;
            animator.SetTrigger("Subir");
            return;
        }

        // Flip de sprite
        if (input.x != 0)
        {
            Vector3 escala = transform.localScale;
            escala.x = Mathf.Sign(input.x) * Mathf.Abs(escala.x);
            transform.localScale = escala;
        }

        // Animaciones de caminar/correr
        bool corriendo = Input.GetKey(teclaCorrer);
        animator.SetBool("isRunning", corriendo && input != Vector2.zero);
        animator.SetBool("isWalking", !corriendo && input != Vector2.zero);

        // Buscar objeto interactuable cercano
        DetectarObjetoInteractuable();

        // Interactuar
        if (Input.GetKeyDown(teclaInteraccion))
        {
            if (objetoInteractuable != null)
            {
                objetoInteractuable.AlternarEstado();
                ActualizarUI(); // Refrescar el texto tras el cambio
            }

            /*if (sillaCercana != null)
            {
                sillaCercana.EjecutarAccion(gameObject);
                Debug.Log("EjecutarAccion llamado desde el jugador");
            }*/
        }
    }

    void FixedUpdate()
    {
        bool corriendo = Input.GetKey(teclaCorrer);
        float velocidadFinal = corriendo ? velocidadCorrer : velocidadMovimiento;
        rb.linearVelocity = input.normalized * velocidadFinal;
    }

    void DetectarObjetoInteractuable()
    {
        objetoInteractuable = null; // ¡Clave! Borra la referencia anterior

        Collider2D[] objetos = Physics2D.OverlapCircleAll(transform.position, rango, capaInteractuable);

        foreach (var col in objetos)
        {
            var candidato = col.GetComponentInParent<ControladorEstados>();
            if (candidato != null)
            {
                objetoInteractuable = candidato;
                break;
            }
        }

        ActualizarUI();
    }

    void ActualizarUI()
    {
        if (mensajeUI == null) return;

        if (objetoInteractuable != null)
        {
            string nombre = objetoInteractuable.ObtenerNombreEstado();
            //mensajeUI.text = $"Presiona {teclaInteraccion} para usar {nombre}";
            mensajeUI.gameObject.SetActive(true);
        }
        else
        {
            mensajeUI.gameObject.SetActive(false);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, rango);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Escalon"))
        {
            Vector3 nuevaPosicion = transform.position + new Vector3(0, 0.5f, 0); // Subir medio unidad (ajustá según el tamaño del escalón)
            transform.position = nuevaPosicion;

            animator.SetTrigger("Subir");
            Debug.Log("Se desliza al subir el escalón");
        }

        if (collision.collider.CompareTag("Suelo") || collision.collider.CompareTag("Piso"))
        {
            enSuelo = true;
            animator.SetBool("isJumping", false); // vuelve a estado normal
        }
        Debug.Log($"💥 Colisionó con: {collision.collider.name}, Tag: {collision.collider.tag}");

        if (collision.collider.CompareTag("Suelo") || collision.collider.CompareTag("Piso"))
        {
            Debug.Log("✅ Detectado suelo correctamente → se desactiva salto");
            animator.SetBool("isJumping", false);
        }
        else
        {
            Debug.Log("⚠️ No es suelo ni piso");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Entró en trigger con: " + other.name);

        if (other.CompareTag("Cama"))
        {
            animator.SetTrigger("Rodar");
            Debug.Log("Entró en trigger con cama");
        }
        if (other.CompareTag("Escalon"))
        {
            animator.SetTrigger("Subir");
            Debug.Log("Entró en trigger con escalon");
            estaDeslizandoEscalon = true;
        }
        if (other.CompareTag("Tapete"))
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 10f); // ajustá la fuerza del salto
            animator.SetBool("isJumping", true); // o SetTrigger("Salto") si usás trigger
            enSuelo = false;
            Debug.Log("Tocó el tapete y saltó");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Escalon"))
        {
            estaDeslizandoEscalon = false;
            Debug.Log("Salió del escalón");
        }
    }
}
