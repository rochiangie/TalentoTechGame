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

    [Header("Transporte Objetos")]
    public Transform puntoDeCarga;
    [SerializeField] private string[] tagsRecogibles = { "Platos", "RopaSucia", "Tarea" };

    private Rigidbody2D rb;
    private Animator animator;
    private Vector2 input;
    private ControladorEstados objetoInteractuable;
    private InteraccionSilla sillaCercana;

    private bool estaDeslizandoEscalon = false;
    public float velocidadEscalon = 2f;
    public Vector2 direccionEscalon = new Vector2(1f, 1f).normalized;
    private bool enSuelo = true;

    private GameObject objetoCercano;
    private GameObject objetoTransportado;
    private bool llevaObjeto = false;

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
            if (llevaObjeto)
            {
                SoltarObjeto();
                return;
            }

            if (!llevaObjeto && objetoCercano != null && EsRecogible(objetoCercano.tag))
            {
                RecogerObjeto();
                return;
            }

            if (objetoInteractuable != null)
            {
                objetoInteractuable.AlternarEstado();
                ActualizarUI(); // Refrescar el texto tras el cambio
            }

            if (sillaCercana != null)
            {
                sillaCercana.EjecutarAccion(gameObject);
            }
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
        objetoInteractuable = null;
        objetoCercano = null;

        Collider2D[] objetos = Physics2D.OverlapCircleAll(transform.position, rango, capaInteractuable);

        foreach (var col in objetos)
        {
            // Buscar objetos interactuables
            var candidato = col.GetComponentInParent<ControladorEstados>();
            if (candidato != null)
            {
                objetoInteractuable = candidato;
            }

            // Buscar objetos recogibles
            if (!llevaObjeto && EsRecogible(col.tag))
            {
                objetoCercano = col.gameObject;
            }

            // Buscar sillas
            var silla = col.GetComponent<InteraccionSilla>();
            if (silla != null)
            {
                sillaCercana = silla;
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
            mensajeUI.text = $"Presiona {teclaInteraccion} para usar {nombre}";
            mensajeUI.gameObject.SetActive(true);
        }
        else if (objetoCercano != null && !llevaObjeto)
        {
            mensajeUI.text = $"Presiona {teclaInteraccion} para recoger {objetoCercano.tag}";
            mensajeUI.gameObject.SetActive(true);
        }
        else
        {
            mensajeUI.gameObject.SetActive(false);
        }
    }

    private bool EsRecogible(string tag)
    {
        foreach (string recogible in tagsRecogibles)
        {
            if (tag == recogible) return true;
        }
        return false;
    }

    private void RecogerObjeto()
    {
        if (objetoCercano == null) return;

        llevaObjeto = true;
        objetoTransportado = objetoCercano;

        // Configuración de transformación
        objetoTransportado.transform.SetParent(puntoDeCarga);
        objetoTransportado.transform.localPosition = Vector3.zero;
        objetoTransportado.transform.localRotation = Quaternion.identity;
        objetoTransportado.transform.localScale = objetoCercano.transform.localScale; // Mantener escala original

        // Asegurar renderizado correcto
        SpriteRenderer srJugador = GetComponent<SpriteRenderer>();
        SpriteRenderer srObjeto = objetoTransportado.GetComponent<SpriteRenderer>();

        if (srJugador != null && srObjeto != null)
        {
            srObjeto.sortingLayerName = srJugador.sortingLayerName;
            srObjeto.sortingOrder = srJugador.sortingOrder + 1; // Renderizar encima
        }

        // Desactivar componentes físicos
        Collider2D col = objetoTransportado.GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        Rigidbody2D rbItem = objetoTransportado.GetComponent<Rigidbody2D>();
        if (rbItem != null) rbItem.simulated = false;

        // Actualizar estado
        objetoCercano = null;
        ActualizarUI();
    }

    private void SoltarObjeto()
    {
        if (objetoTransportado == null) return;

        objetoTransportado.transform.SetParent(null);
        objetoTransportado.transform.position = transform.position + Vector3.right;

        Collider2D col = objetoTransportado.GetComponent<Collider2D>();
        if (col != null) col.enabled = true;

        Rigidbody2D rbItem = objetoTransportado.GetComponent<Rigidbody2D>();
        if (rbItem != null) rbItem.simulated = true;

        llevaObjeto = false;
        objetoTransportado = null;
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
            Vector3 nuevaPosicion = transform.position + new Vector3(0, 0.5f, 0);
            transform.position = nuevaPosicion;

            animator.SetTrigger("Subir");
        }

        if (collision.collider.CompareTag("Suelo") || collision.collider.CompareTag("Piso"))
        {
            enSuelo = true;
            animator.SetBool("isJumping", false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Cama"))
        {
            animator.SetTrigger("Rodar");
        }
        if (other.CompareTag("Escalon"))
        {
            animator.SetTrigger("Subir");
            estaDeslizandoEscalon = true;
        }
        if (other.CompareTag("Tapete"))
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 10f);
            animator.SetBool("isJumping", true);
            enSuelo = false;
        }
        if (other.CompareTag("Silla"))
        {
            animator.SetBool("isTouchingObject", true);
            sillaCercana = other.GetComponent<InteraccionSilla>();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Escalon"))
        {
            estaDeslizandoEscalon = false;
        }
        if (other.CompareTag("Silla"))
        {
            animator.SetBool("isTouchingObject", false);
            sillaCercana = null;
        }
    }

    public bool EstaLlevandoObjeto()
    {
        return llevaObjeto;
    }

    public void SoltarYDestruirObjeto()
    {
        if (llevaObjeto && objetoTransportado != null)
        {
            Destroy(objetoTransportado);
            objetoTransportado = null;
            llevaObjeto = false;
        }
    }
}