using UnityEngine;
using TMPro;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator), typeof(Collider2D))]
public class InteraccionYMovimiento : MonoBehaviour
{
    [Header("Configuración Básica")]
    public float velocidadMovimiento = 5f;
    public KeyCode teclaInteraccion = KeyCode.E;
    public TextMeshProUGUI textoInteraccion;
    public float rangoInteraccion = 1.5f;
    public LayerMask capaInteractuable;

    [Header("Prefabs Inodoro")]
    public GameObject inodoroTapadoPrefab;
    public GameObject inodoroLlenoPrefab;
    public Transform spawnInodoro;

    [Header("Prefabs Bañera")]
    public GameObject bañeraVaciaPrefab;
    public GameObject bañeraLlenaPrefab;
    public Transform spawnBañera;

    [Header("Prefabs Lavamanos")]
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

    void Update()
    {
        ProcesarMovimiento();
        VerificarInteraccion();
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

    private void VerificarInteraccion()
    {
        Collider2D[] objetosCercanos = Physics2D.OverlapCircleAll(transform.position, rangoInteraccion, capaInteractuable);
        bool interaccionDisponible = false;

        foreach (Collider2D col in objetosCercanos)
        {
            if (col.CompareTag("Inodoro") || col.CompareTag("Bañera") || col.CompareTag("Lavamanos"))
            {
                objetoActual = col.gameObject;
                interaccionDisponible = true;
                break;
            }
        }

        puedeInteractuar = interaccionDisponible;
        ActualizarUI();

        if (puedeInteractuar && Input.GetKeyDown(teclaInteraccion))
        {
            ManejarInteraccion();
        }
    }

    private void ManejarInteraccion()
    {
        if (objetoActual == null) return;

        string tag = objetoActual.tag;
        bool estadoActual = objetoActual.name.Contains("Lleno") || objetoActual.name.Contains("Llena");

        Vector3 posicionSpawn = Vector3.zero;
        GameObject prefabAlternativo = null;

        switch (tag)
        {
            case "Inodoro":
                posicionSpawn = spawnInodoro.position;
                prefabAlternativo = estadoActual ? inodoroTapadoPrefab : inodoroLlenoPrefab;
                break;
            case "Bañera":
                posicionSpawn = spawnBañera.position;
                prefabAlternativo = estadoActual ? bañeraVaciaPrefab : bañeraLlenaPrefab;
                break;
            case "Lavamanos":
                posicionSpawn = spawnLavamanos.position;
                prefabAlternativo = estadoActual ? lavamanosVacioPrefab : lavamanosLlenoPrefab;
                break;
        }

        if (prefabAlternativo == null)
        {
            Debug.LogError($"Prefab no asignado para {tag}");
            return;
        }

        // Guardar rotación y escala
        Quaternion rotacion = objetoActual.transform.rotation;
        Vector3 escala = objetoActual.transform.localScale;

        Destroy(objetoActual);

        // Instanciar nuevo objeto
        objetoActual = Instantiate(prefabAlternativo, posicionSpawn, rotacion);
        objetoActual.transform.localScale = escala;

        // Configuración automática
        ConfigurarObjetoInteractuable(objetoActual, tag);
    }

    private void ConfigurarObjetoInteractuable(GameObject obj, string tag)
    {
        obj.tag = tag;

        // Asegurar Collider2D
        Collider2D collider = obj.GetComponent<Collider2D>();
        if (collider == null) collider = obj.AddComponent<BoxCollider2D>();
        collider.isTrigger = true;

        // Asegurar Rigidbody2D
        Rigidbody2D rbObj = obj.GetComponent<Rigidbody2D>();
        if (rbObj == null) rbObj = obj.AddComponent<Rigidbody2D>();
        rbObj.isKinematic = true;
    }

    private void ActualizarUI()
    {
        if (textoInteraccion == null) return;

        textoInteraccion.gameObject.SetActive(puedeInteractuar);
        if (puedeInteractuar)
        {
            textoInteraccion.text = $"Presiona {teclaInteraccion} para interactuar";
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, rangoInteraccion);
    }
}