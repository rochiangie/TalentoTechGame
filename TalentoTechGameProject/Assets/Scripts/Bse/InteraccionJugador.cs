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
    private ControladorEstados objetoInteractuableCercano;
    private CabinetController gabinetePlatosCercano;
    private InteraccionSilla sillaCercana;

    private bool estaDeslizandoEscalon = false;
    public float velocidadEscalon = 2f;
    public Vector2 direccionEscalon = new Vector2(1f, 1f).normalized;
    private bool enSuelo = true;

    private GameObject objetoCercanoRecogible;
    private GameObject objetoTransportado;
    private bool llevaObjeto = false;
    public GameObject ObjetoTransportado { get { return objetoTransportado; } }
    [SerializeField] private Transform canvasFlotante;

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

        if (canvasFlotante != null)
        {
            Vector3 escala = canvasFlotante.localScale;
            escala.x = Mathf.Abs(escala.x); // aseguro que sea positiva
            canvasFlotante.localScale = escala;
        }


        // Animaciones de caminar/correr
        bool corriendo = Input.GetKey(teclaCorrer);
        animator.SetBool("isRunning", corriendo && input != Vector2.zero);
        animator.SetBool("isWalking", !corriendo && input != Vector2.zero);

        // Buscar objetos cercanos
        DetectarObjetosCercanos();

        // Interactuar
        if (Input.GetKeyDown(teclaInteraccion))
        {
            // Interactuar con el gabinete de platos
            if (gabinetePlatosCercano != null)
            {
                // Intentar guardar platos
                if (!gabinetePlatosCercano.EstaLleno() && llevaObjeto && objetoTransportado.CompareTag(gabinetePlatosCercano.tagObjetoRequerido))
                {
                    gabinetePlatosCercano.IntentarGuardarPlatos(this);
                    return;
                }
                // Intentar sacar platos
                else if (gabinetePlatosCercano.EstaLleno() && !llevaObjeto)
                {
                    gabinetePlatosCercano.SacarPlatosDelGabinete(this);
                    return;
                }
                // Mostrar mensaje específico si no se cumplen las condiciones (opcional)
                else if (llevaObjeto && !objetoTransportado.CompareTag(gabinetePlatosCercano.tagObjetoRequerido))
                {
                    //ActualizarMensajeUI($"Necesitas {gabinetePlatosCercano.tagObjetoRequerido} para este gabinete.");
                    return;
                }
            }
            // Interactuar con objetos con ControladorEstados
            else if (objetoInteractuableCercano != null)
            {
                objetoInteractuableCercano.AlternarEstado();
                ActualizarUI();
                return;
            }
            // Recoger objetos
            else if (!llevaObjeto && objetoCercanoRecogible != null && EsRecogible(objetoCercanoRecogible.tag))
            {
                RecogerObjeto(objetoCercanoRecogible);
                return;
            }
            // Interactuar con sillas
            else if (!llevaObjeto && sillaCercana != null)
            {
                sillaCercana.EjecutarAccion(gameObject);
            }
            else
            {
                ActualizarUI(); // Asegura que la UI se actualice si no hay interacción
            }
        }
        else
        {
            ActualizarUI(); // Actualiza la UI cada frame para mostrar el mensaje correcto
        }
    }

    void FixedUpdate()
    {
        bool corriendo = Input.GetKey(teclaCorrer);
        float velocidadFinal = corriendo ? velocidadCorrer : velocidadMovimiento;
        rb.linearVelocity = input.normalized * velocidadFinal;
    }

    void DetectarObjetosCercanos()
    {
        objetoInteractuableCercano = null;
        gabinetePlatosCercano = null;
        objetoCercanoRecogible = null;
        sillaCercana = null;

        Collider2D[] objetos = Physics2D.OverlapCircleAll(transform.position, rango, capaInteractuable);

        foreach (var col in objetos)
        {
            var interactuable = col.GetComponentInParent<ControladorEstados>();
            if (interactuable != null)
            {
                objetoInteractuableCercano = interactuable;
            }

            var gabinetePlatos = col.GetComponent<CabinetController>();
            if (gabinetePlatos != null)
            {
                gabinetePlatosCercano = gabinetePlatos;
            }

            if (!llevaObjeto && EsRecogible(col.tag))
            {
                objetoCercanoRecogible = col.gameObject;
            }

            var silla = col.GetComponent<InteraccionSilla>();
            if (silla != null)
            {
                sillaCercana = silla;
            }
        }

        ActualizarUI();
    }

    void ActualizarUI(string mensaje = "")
    {
        if (mensajeUI == null) return;

        if (!string.IsNullOrEmpty(mensaje))
        {
            mensajeUI.text = mensaje;
            mensajeUI.gameObject.SetActive(true);
        }
        else if (gabinetePlatosCercano != null)
        {
            if (!gabinetePlatosCercano.EstaLleno())
            {
                mensajeUI.text = $"Presiona {teclaInteraccion} para guardar {gabinetePlatosCercano.tagObjetoRequerido}";
            }
            else
            {
                mensajeUI.text = $"Presiona {teclaInteraccion} para sacar {gabinetePlatosCercano.prefabPlatos.name}(s)";
            }
            mensajeUI.gameObject.SetActive(true);
        }
        else if (objetoInteractuableCercano != null)
        {
            string nombre = objetoInteractuableCercano.ObtenerNombreEstado();
            mensajeUI.text = $"Presiona {teclaInteraccion} para usar {nombre}";
            mensajeUI.gameObject.SetActive(true);
        }
        else if (objetoCercanoRecogible != null && !llevaObjeto)
        {
            mensajeUI.text = $"Presiona {teclaInteraccion} para recoger {objetoCercanoRecogible.tag}";
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

    public void RecogerObjeto(GameObject objeto)
    {
        if (llevaObjeto || objeto == null) return;

        llevaObjeto = true;
        objetoTransportado = objeto;

        // Transformación
        objetoTransportado.transform.SetParent(puntoDeCarga);
        objetoTransportado.transform.localPosition = Vector3.zero;
        objetoTransportado.transform.localRotation = Quaternion.identity;
        objetoTransportado.transform.localScale = objeto.transform.localScale;

        // Renderizado
        SpriteRenderer srJugador = GetComponent<SpriteRenderer>();
        SpriteRenderer srObjeto = objetoTransportado.GetComponent<SpriteRenderer>();

        if (srJugador != null && srObjeto != null)
        {
            srObjeto.sortingLayerName = srJugador.sortingLayerName;
            srObjeto.sortingOrder = srJugador.sortingOrder + 1;
        }

        // Física
        Collider2D col = objetoTransportado.GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        Rigidbody2D rb = objetoTransportado.GetComponent<Rigidbody2D>();
        if (rb != null) rb.simulated = false;

        objetoCercanoRecogible = null;
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

    public void SoltarYDestruirObjeto()
    {
        if (llevaObjeto && objetoTransportado != null)
        {
            Destroy(objetoTransportado);
            objetoTransportado = null;
            llevaObjeto = false;
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
}