using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator), typeof(Collider2D))]
public class InteraccionYMovimiento : MonoBehaviour
{
    [Header("Movimiento")]
    public float velocidadMovimiento = 5f;

    [Header("Interacción")]
    public GameObject prefabTapado;
    public GameObject prefabLleno;
    public Transform puntoSpawn;
    public KeyCode teclaInteraccion = KeyCode.E;
    public GameObject textoInteraccion;

    private Rigidbody2D rb;
    private Animator animator;
    private Vector2 direccionMovimiento;
    private GameObject objetoActivo;
    private bool estadoTapado = true;
    private bool puedeInteractuar = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        // Configuración óptima del Rigidbody
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.freezeRotation = true;
    }

    void Start()
    {
        if (textoInteraccion != null)
            textoInteraccion.SetActive(false);

        CrearObjetoInteractuableInicial();
    }

    void Update()
    {
        ProcesarMovimiento();
        ProcesarInteraccion();
    }

    void FixedUpdate()
    {
        rb.linearVelocity = direccionMovimiento.normalized * velocidadMovimiento;
    }

    private void CrearObjetoInteractuableInicial()
    {
        if (prefabTapado != null && puntoSpawn != null)
        {
            objetoActivo = Instantiate(prefabTapado, puntoSpawn.position, Quaternion.identity);
            ConfigurarObjetoInteractuable(objetoActivo);
            estadoTapado = true;
        }
    }

    private void ConfigurarObjetoInteractuable(GameObject obj)
    {
        if (obj == null) return;

        obj.tag = "InodoroInteractuable";

        // Asegurar Collider2D
        var collider = obj.GetComponent<Collider2D>();
        if (collider == null)
        {
            collider = obj.AddComponent<BoxCollider2D>();
        }
        collider.isTrigger = true;

        // Asegurar Rigidbody2D
        var rbObj = obj.GetComponent<Rigidbody2D>();
        if (rbObj == null)
        {
            rbObj = obj.AddComponent<Rigidbody2D>();
        }
        rbObj.isKinematic = true;
        rbObj.simulated = true;
    }

    private void ProcesarMovimiento()
    {
        direccionMovimiento.x = Input.GetAxisRaw("Horizontal");
        direccionMovimiento.y = Input.GetAxisRaw("Vertical");

        animator.SetBool("isWalking", direccionMovimiento != Vector2.zero);

        if (direccionMovimiento.x != 0)
        {
            transform.localScale = new Vector3(
                Mathf.Sign(direccionMovimiento.x) * Mathf.Abs(transform.localScale.x),
                transform.localScale.y,
                transform.localScale.z
            );
        }
    }

    private void ProcesarInteraccion()
    {
        if (puedeInteractuar && Input.GetKeyDown(teclaInteraccion))
        {
            InteractuarConObjeto();
        }
    }

    private void InteractuarConObjeto()
    {
        if (objetoActivo == null) return;

        Quaternion rotacion = objetoActivo.transform.rotation;
        Vector3 escala = objetoActivo.transform.localScale;

        Destroy(objetoActivo);
        estadoTapado = !estadoTapado;

        objetoActivo = Instantiate(
            estadoTapado ? prefabTapado : prefabLleno,
            puntoSpawn.position,
            rotacion
        );

        ConfigurarObjetoInteractuable(objetoActivo);
        ActualizarInterfaz();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("InodoroInteractuable"))
        {
            objetoActivo = other.gameObject;
            puedeInteractuar = true;
            ActualizarInterfaz();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("InodoroInteractuable"))
        {
            puedeInteractuar = false;
            ActualizarInterfaz();
        }
    }

    private void ActualizarInterfaz()
    {
        if (textoInteraccion != null)
        {
            textoInteraccion.SetActive(puedeInteractuar);
        }
    }
}