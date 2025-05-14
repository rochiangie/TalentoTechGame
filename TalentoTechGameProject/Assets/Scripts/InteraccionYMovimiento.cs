using UnityEngine;
using TMPro;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator), typeof(Collider2D))]
public class InteraccionYMovimiento : MonoBehaviour
{
    [Header("Movimiento")]
    public float velocidadMovimiento = 5f;
    private Rigidbody2D rb;
    private Animator animator;
    private Vector2 direccionMovimiento;

    [Header("Interacción")]
    public TextMeshProUGUI mensajeUI;
    public GameObject prefabTapado;
    public GameObject prefabLleno;
    public Transform puntoSpawn;
    public KeyCode teclaInteraccion = KeyCode.E;
    [SerializeField] private LayerMask capaInteractuable; // Nueva capa para filtrado

    private GameObject objetoActivo;
    private bool estadoTapado = true;
    private bool puedeInteractuar = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        // Configuración mejorada del Rigidbody
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        // Asegurar que existe Collider2D
        if (GetComponent<Collider2D>() == null)
        {
            gameObject.AddComponent<BoxCollider2D>();
        }
    }

    void Start()
    {
        if (mensajeUI != null)
            mensajeUI.gameObject.SetActive(false);
        else
            Debug.LogWarning("mensajeUI no está asignado.");

        if (prefabTapado != null && puntoSpawn != null)
        {
            objetoActivo = Instantiate(prefabTapado, puntoSpawn.position, Quaternion.identity);
            ConfigurarObjetoInteractuable(objetoActivo);
            estadoTapado = true;
        }
    }

    void Update()
    {
        ProcesarMovimiento();
        ProcesarInteraccion();
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + direccionMovimiento.normalized * velocidadMovimiento * Time.fixedDeltaTime);
    }

    private void ProcesarMovimiento()
    {
        direccionMovimiento.x = Input.GetAxisRaw("Horizontal");
        direccionMovimiento.y = Input.GetAxisRaw("Vertical");

        animator.SetBool("isWalking", direccionMovimiento != Vector2.zero);

        if (direccionMovimiento.x != 0)
        {
            Vector3 escala = transform.localScale;
            escala.x = Mathf.Abs(escala.x) * Mathf.Sign(direccionMovimiento.x);
            transform.localScale = escala;
        }
    }

    private void ProcesarInteraccion()
    {
        if (puedeInteractuar && objetoActivo != null && Input.GetKeyDown(teclaInteraccion))
        {
            InteractuarConObjeto();
        }
    }

    private void InteractuarConObjeto()
    {
        Quaternion rot = objetoActivo.transform.rotation;
        Vector3 escala = objetoActivo.transform.localScale;

        Destroy(objetoActivo);
        estadoTapado = !estadoTapado;

        GameObject nuevo = Instantiate(
            estadoTapado ? prefabTapado : prefabLleno,
            puntoSpawn.position,
            rot
        );

        ConfigurarObjetoInteractuable(nuevo);
        objetoActivo = nuevo;

        MostrarMensaje();
    }

    private void ConfigurarObjetoInteractuable(GameObject obj)
    {
        obj.tag = "InodoroInteractuable";

        // Asegurar componentes de física
        var collider = obj.GetComponent<Collider2D>();
        if (collider == null)
        {
            collider = obj.AddComponent<BoxCollider2D>();
        }
        collider.isTrigger = true;

        if (obj.GetComponent<Rigidbody2D>() == null)
        {
            var rbObj = obj.AddComponent<Rigidbody2D>();
            rbObj.isKinematic = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("InodoroInteractuable"))
        {
            SetObjetoActivo(other.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("InodoroInteractuable"))
        {
            ClearObjetoActivo();
        }
    }

    public void SetObjetoActivo(GameObject nuevo)
    {
        if (nuevo == null || objetoActivo == nuevo) return;

        objetoActivo = nuevo;
        puedeInteractuar = true;
        MostrarMensaje();
    }

    public void ClearObjetoActivo()
    {
        puedeInteractuar = false;
        objetoActivo = null;

        if (mensajeUI != null)
            mensajeUI.gameObject.SetActive(false);
    }

    private void MostrarMensaje()
    {
        if (mensajeUI != null)
        {
            mensajeUI.text = estadoTapado ? "Presioná E para destapar" : "Presioná E para tapar";
            mensajeUI.gameObject.SetActive(true);
        }
    }

    // Método opcional para debuggear el rango de interacción
    private void OnDrawGizmosSelected()
    {
        Collider2D collider = GetComponent<Collider2D>();
        if (collider == null) return;

        Gizmos.color = new Color(0, 1, 0, 0.3f);

        // Para BoxCollider2D
        BoxCollider2D boxCollider = collider as BoxCollider2D;
        if (boxCollider != null)
        {
            Gizmos.DrawCube(transform.position + (Vector3)boxCollider.offset, boxCollider.size);
            return;
        }

        // Para CircleCollider2D
        CircleCollider2D circleCollider = collider as CircleCollider2D;
        if (circleCollider != null)
        {
            Gizmos.DrawSphere(transform.position + (Vector3)circleCollider.offset, circleCollider.radius);
            return;
        }

        // Para otros tipos de colliders
        Gizmos.DrawWireCube(transform.position, Vector3.one);
    }
}