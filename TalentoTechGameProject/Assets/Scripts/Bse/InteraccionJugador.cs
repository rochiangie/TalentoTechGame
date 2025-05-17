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

    private GameObject objetoCercanoRecogible;
    private GameObject objetoTransportado;
    private bool llevaObjeto = false;
    public GameObject ObjetoTransportado => objetoTransportado;
    private GameObject objetoCercano;
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        // Movimiento y animación
        bool corriendo = Input.GetKey(teclaCorrer);
        animator.SetBool("isRunning", corriendo && input != Vector2.zero);
        animator.SetBool("isWalking", !corriendo && input != Vector2.zero);

        // Flip de sprite
        if (input.x != 0)
        {
            Vector3 escala = transform.localScale;
            escala.x = Mathf.Sign(input.x) * Mathf.Abs(escala.x);
            transform.localScale = escala;
        }

        // Detectar objetos
        DetectarObjetosCercanos();

        // Interacciones
        if (Input.GetKeyDown(teclaInteraccion))
        {
            if (gabinetePlatosCercano != null)
            {
                if (!gabinetePlatosCercano.EstaLleno() && llevaObjeto && objetoTransportado.CompareTag(gabinetePlatosCercano.TagObjetoRequerido))
                {
                    gabinetePlatosCercano.IntentarGuardarPlatos(this);
                    return;
                }
                else if (gabinetePlatosCercano.EstaLleno() && !llevaObjeto)
                {
                    gabinetePlatosCercano.SacarPlatosDelGabinete(this);
                    return;
                }
                return;
            }
            else if (objetoInteractuableCercano != null)
            {
                objetoInteractuableCercano.AlternarEstado();
                return;
            }
            else if (!llevaObjeto && objetoCercanoRecogible != null && EsRecogible(objetoCercanoRecogible.tag))
            {
                RecogerObjeto(objetoCercanoRecogible);
                return;
            }
            else if (!llevaObjeto && sillaCercana != null)
            {
                sillaCercana.EjecutarAccion(gameObject);
            }
        }

        // Soltar objeto (si lleva algo y presiona E)
        if (Input.GetKeyDown(KeyCode.E) && llevaObjeto)
        {
            SoltarObjeto();
        }

        ActualizarUI();
    }

    void FixedUpdate()
    {
        float velocidad = Input.GetKey(teclaCorrer) ? velocidadCorrer : velocidadMovimiento;
        rb.linearVelocity = input.normalized * velocidad;
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
            if (col.TryGetComponent(out ControladorEstados interactuable))
                objetoInteractuableCercano = interactuable;

            if (col.TryGetComponent(out CabinetController gabinete))
                gabinetePlatosCercano = gabinete;

            if (!llevaObjeto && EsRecogible(col.tag))
                objetoCercanoRecogible = col.gameObject;

            if (col.TryGetComponent(out InteraccionSilla silla))
                sillaCercana = silla;
        }
    }

    void ActualizarUI()
    {
        if (mensajeUI == null) return;

        if (gabinetePlatosCercano != null)
        {
            mensajeUI.text = gabinetePlatosCercano.EstaLleno()
                ? $"Presiona {teclaInteraccion} para sacar {gabinetePlatosCercano.PrefabPlatos.name}"
                : $"Presiona {teclaInteraccion} para guardar {gabinetePlatosCercano.TagObjetoRequerido}";
            mensajeUI.gameObject.SetActive(true);
        }
        else if (objetoInteractuableCercano != null)
        {
            mensajeUI.text = $"Presiona {teclaInteraccion} para usar {objetoInteractuableCercano.ObtenerNombreEstado()}";
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

    bool EsRecogible(string tag)
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

        objetoTransportado.transform.SetParent(puntoDeCarga);
        objetoTransportado.transform.localPosition = Vector3.zero;
        objetoTransportado.transform.localRotation = Quaternion.identity;
        objetoTransportado.transform.localScale = new Vector3(1f / transform.localScale.x, 1f / transform.localScale.y, 1f);

        SpriteRenderer srJugador = GetComponent<SpriteRenderer>();
        SpriteRenderer srObjeto = objetoTransportado.GetComponent<SpriteRenderer>();

        if (srJugador != null && srObjeto != null)
        {
            srObjeto.sortingLayerName = srJugador.sortingLayerName; // misma capa
            srObjeto.sortingOrder = srJugador.sortingOrder + 1;     // encima del jugador
        }


        Collider2D col = objetoTransportado.GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        Rigidbody2D rb = objetoTransportado.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.simulated = false;
            rb.isKinematic = true;
        }

        objetoCercanoRecogible = null;
        ActualizarUI();
    }



    void SoltarObjeto()
    {
        if (objetoTransportado == null) return;

        objetoTransportado.transform.SetParent(null);
        objetoTransportado.transform.position = transform.position + Vector3.right;

        if (objetoTransportado.TryGetComponent(out Collider2D col)) col.enabled = true;
        if (objetoTransportado.TryGetComponent(out Rigidbody2D rb))
        {
            rb.simulated = true;
            rb.linearVelocity = Vector2.zero;
        }

        llevaObjeto = false;
        objetoTransportado = null;
    }

    public void SoltarYDestruirObjeto()
    {
        if (!llevaObjeto || objetoTransportado == null) return;

        Destroy(objetoTransportado);
        objetoTransportado = null;
        llevaObjeto = false;
    }

    public bool EstaLlevandoObjeto() => llevaObjeto;

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, rango);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject raiz = collision.transform.root.gameObject;

        if (collision.collider.CompareTag("Suelo") || collision.collider.CompareTag("Piso"))
        {
            enSuelo = true;
            animator.SetBool("isJumping", false);
        }
        if (collision.collider.CompareTag("Inodoro") || collision.collider.CompareTag("Lavamanos") || collision.collider.CompareTag("Bañera"))
        {
            objetoCercano = collision.gameObject;
            Debug.Log("Colisionó con: " + objetoCercano.name);
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        GameObject raiz = collision.transform.root.gameObject;

        if (collision.gameObject == objetoCercano)
        {
            objetoCercano = null;
            Debug.Log("Salió de la colisión");
        }
    }

    private bool enSuelo = true;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Tapete"))
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 10f);
            animator.SetBool("isJumping", true);
            enSuelo = false;
        }

        if (other.CompareTag("Inodoro") || other.CompareTag("Lavamanos") || other.CompareTag("Bañera"))
        {
            objetoCercano = other.gameObject;
            Debug.Log("Objeto cercano: " + objetoCercano.name);
        }

        if (other.CompareTag("Silla"))
        {
            animator.SetBool("isTouchingObject", true);
            sillaCercana = other.GetComponent<InteraccionSilla>();
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Silla"))
        {
            animator.SetBool("isTouchingObject", false);
            sillaCercana = null;
        }

        if (other.gameObject == objetoCercano)
        {
            objetoCercano = null;
            Debug.Log("Objeto salió de alcance");
        }
    }
}
