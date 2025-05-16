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

    private Rigidbody2D rb;
    private Animator animator;
    private Vector2 input;
    private ControladorEstados objetoInteractuable;

    private bool estaDeslizandoEscalon = false;
    public float velocidadEscalon = 2f;
    public Vector2 direccionEscalon = new Vector2(1f, 1f).normalized;
    private bool enSuelo = true;

    private GameObject objetoCercano;
    private GameObject objetoTransportado;
    private bool llevaObjeto = false;
    public Transform puntoDeCarga;
    [SerializeField] private string[] tagsRecogibles = { "Platos", "RopaSucia", "Tarea" };

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        if (estaDeslizandoEscalon)
        {
            rb.linearVelocity = direccionEscalon * velocidadEscalon;
            animator.SetTrigger("Subir");
            return;
        }

        if (input.x != 0)
        {
            Vector3 escala = transform.localScale;
            escala.x = Mathf.Sign(input.x) * Mathf.Abs(escala.x);
            transform.localScale = escala;
        }

        bool corriendo = Input.GetKey(teclaCorrer);
        animator.SetBool("isRunning", corriendo && input != Vector2.zero);
        animator.SetBool("isWalking", !corriendo && input != Vector2.zero);

        if (Input.GetKeyDown(teclaInteraccion))
        {
            if (llevaObjeto)
            {
                SoltarObjeto();
                return;
            }

            if (!llevaObjeto && objetoCercano != null && EsRecogible(objetoCercano.tag))
            {
                llevaObjeto = true;
                objetoTransportado = objetoCercano;
                objetoTransportado.transform.SetParent(puntoDeCarga);
                objetoTransportado.transform.localPosition = Vector3.zero;

                Collider2D col = objetoTransportado.GetComponent<Collider2D>();
                if (col != null) col.enabled = false;

                Rigidbody2D rbItem = objetoTransportado.GetComponent<Rigidbody2D>();
                if (rbItem != null) rbItem.simulated = false;
                return;
            }

            if (objetoInteractuable != null)
            {
                float distancia = Vector2.Distance(transform.position, objetoInteractuable.transform.position);
                if (distancia <= rango)
                {
                    objetoInteractuable.AlternarEstado();
                }
            }
        }
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

    void FixedUpdate()
    {
        bool corriendo = Input.GetKey(teclaCorrer);
        float velocidadFinal = corriendo ? velocidadCorrer : velocidadMovimiento;
        rb.linearVelocity = input.normalized * velocidadFinal;
    }

    private bool EsRecogible(string tag)
    {
        foreach (string recogible in tagsRecogibles)
        {
            if (tag == recogible) return true;
        }
        return false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        ProcesarContacto(other);

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
        }

        if (other.gameObject == objetoCercano)
        {
            objetoCercano = null;
        }

        if (other.GetComponentInParent<ControladorEstados>() == objetoInteractuable)
        {
            objetoInteractuable = null;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        ProcesarContacto(collision.collider);

        if (collision.collider.CompareTag("Escalon"))
        {
            transform.position += new Vector3(0, 0.5f, 0);
            animator.SetTrigger("Subir");
        }

        if (collision.collider.CompareTag("Suelo") || collision.collider.CompareTag("Piso"))
        {
            enSuelo = true;
            animator.SetBool("isJumping", false);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.gameObject == objetoCercano)
        {
            objetoCercano = null;
        }

        if (collision.collider.GetComponentInParent<ControladorEstados>() == objetoInteractuable)
        {
            objetoInteractuable = null;
        }
    }

    private void ProcesarContacto(Collider2D other)
    {
        if (EsRecogible(other.tag))
        {
            objetoCercano = other.gameObject;
        }

        var candidato = other.GetComponentInParent<ControladorEstados>();
        if (candidato != null)
        {
            objetoInteractuable = candidato;
            Debug.Log($"🟢 Detectado interactuable: {candidato.gameObject.name}");
        }
        else
        {
            Debug.Log($"⚠️ Sin ControladorEstados en: {other.name}");
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