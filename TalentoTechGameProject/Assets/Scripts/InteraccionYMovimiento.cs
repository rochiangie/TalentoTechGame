using UnityEngine;
using TMPro;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator), typeof(Collider2D))]
public class InteraccionYMovimiento : MonoBehaviour
{
    [Header("Movimiento")]
    public float velocidadMovimiento = 5f;

    [Header("Configuración Interacción")]
    public KeyCode teclaInteraccion = KeyCode.E;
    public TextMeshProUGUI textoInteraccion;
    public float rangoDeteccion = 1.5f;

    [Header("Inodoro")]
    public GameObject inodoroTapadoPrefab;
    public GameObject inodoroLlenoPrefab;
    public Transform spawnInodoro;

    [Header("Bañera")]
    public GameObject bañeraVaciaPrefab;
    public GameObject bañeraLlenaPrefab;
    public Transform spawnBañera;

    [Header("Lavamanos")]
    public GameObject lavamanosVacioPrefab;
    public GameObject lavamanosLlenoPrefab;
    public Transform spawnLavamanos;

    private Rigidbody2D rb;
    private Animator animator;
    private Vector2 direccionMovimiento;
    private GameObject objetoActual;
    private bool puedeInteractuar = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.freezeRotation = true;
    }

    void Start()
    {
        if (textoInteraccion != null)
            textoInteraccion.gameObject.SetActive(false);
    }

    void Update()
    {
        ProcesarMovimiento();
        DetectarObjetosCercanos();

        if (puedeInteractuar && Input.GetKeyDown(teclaInteraccion))
        {
            InteractuarConObjeto();
        }
    }

    void FixedUpdate()
    {
        rb.linearVelocity = direccionMovimiento.normalized * velocidadMovimiento;
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

    private void DetectarObjetosCercanos()
    {
        Collider2D[] objetos = Physics2D.OverlapCircleAll(transform.position, rangoDeteccion);
        GameObject objetoMasCercano = null;
        float distanciaMinima = Mathf.Infinity;

        foreach (Collider2D col in objetos)
        {
            if (col.CompareTag("Inodoro") || col.CompareTag("Bañera") || col.CompareTag("Lavamanos"))
            {
                float distancia = Vector2.Distance(transform.position, col.transform.position);
                if (distancia < distanciaMinima)
                {
                    distanciaMinima = distancia;
                    objetoMasCercano = col.gameObject;
                }
            }
        }

        objetoActual = objetoMasCercano;
        puedeInteractuar = (objetoActual != null);
        ActualizarInterfaz();
    }

    private void InteractuarConObjeto()
    {
        if (objetoActual == null) return;

        // Determinar qué objeto es y su configuración
        GameObject prefabAlternativo = null;
        Transform spawnPoint = null;

        if (objetoActual.CompareTag("Inodoro"))
        {
            bool estaTapado = objetoActual.name.Contains("Tapado");
            prefabAlternativo = estaTapado ? inodoroLlenoPrefab : inodoroTapadoPrefab;
            spawnPoint = spawnInodoro;
        }
        else if (objetoActual.CompareTag("Bañera"))
        {
            bool estaLlena = objetoActual.name.Contains("Llena");
            prefabAlternativo = estaLlena ? bañeraVaciaPrefab : bañeraLlenaPrefab;
            spawnPoint = spawnBañera;
        }
        else if (objetoActual.CompareTag("Lavamanos"))
        {
            bool estaLleno = objetoActual.name.Contains("Lleno");
            prefabAlternativo = estaLleno ? lavamanosVacioPrefab : lavamanosLlenoPrefab;
            spawnPoint = spawnLavamanos;
        }

        if (prefabAlternativo == null || spawnPoint == null)
        {
            Debug.LogError("Prefab o spawn point no asignado!");
            return;
        }

        // Guardar propiedades antes de destruir
        Quaternion rotacion = objetoActual.transform.rotation;
        Vector3 escala = objetoActual.transform.localScale;

        Destroy(objetoActual);

        // Instanciar nuevo objeto
        objetoActual = Instantiate(
            prefabAlternativo,
            spawnPoint.position, // Usar posición exacta del spawn point
            rotacion
        );

        // Configurar el nuevo objeto
        objetoActual.transform.localScale = escala;
        ConfigurarObjetoInteractuable(objetoActual);
        ActualizarInterfaz();

        Debug.Log($"Instanciado {objetoActual.name} en {spawnPoint.position}");
    }

    private void ConfigurarObjetoInteractuable(GameObject obj)
    {
        if (obj == null) return;

        // Configurar tag según el tipo de objeto
        if (obj.name.Contains("Inodoro")) obj.tag = "Inodoro";
        else if (obj.name.Contains("Bañera")) obj.tag = "Bañera";
        else if (obj.name.Contains("Lavamanos")) obj.tag = "Lavamanos";

        // Asegurar componentes físicos
        Collider2D collider = obj.GetComponent<Collider2D>() ?? obj.AddComponent<BoxCollider2D>();
        collider.isTrigger = true;

        Rigidbody2D rbObj = obj.GetComponent<Rigidbody2D>() ?? obj.AddComponent<Rigidbody2D>();
        rbObj.isKinematic = true;
    }

    private void ActualizarInterfaz()
    {
        if (textoInteraccion == null) return;

        textoInteraccion.text = puedeInteractuar ? $"Presiona {teclaInteraccion} para interactuar" : "";
        textoInteraccion.gameObject.SetActive(puedeInteractuar);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, rangoDeteccion);

        DibujarSpawnPoint(spawnInodoro, Color.blue);
        DibujarSpawnPoint(spawnBañera, Color.red);
        DibujarSpawnPoint(spawnLavamanos, Color.yellow);
    }

    private void DibujarSpawnPoint(Transform spawn, Color color)
    {
        if (spawn != null)
        {
            Gizmos.color = color;
            Gizmos.DrawWireCube(spawn.position, Vector3.one * 0.3f);
            Gizmos.DrawLine(spawn.position, spawn.position + Vector3.right * 0.5f);
        }
    }
}