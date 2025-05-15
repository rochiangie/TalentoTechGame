using UnityEngine;
using TMPro;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator))]
public class InteraccionJugador : MonoBehaviour
{
    [Header("Movimiento")]
    public float velocidadMovimiento = 5f;

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

    private InteraccionSilla sillaCercana; // NUEVO

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // Movimiento
        input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        animator.SetBool("isWalking", input != Vector2.zero);

        if (input.x != 0)
        {
            Vector3 escala = transform.localScale;
            escala.x = Mathf.Sign(input.x) * Mathf.Abs(escala.x);
            transform.localScale = escala;
        }

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
        rb.linearVelocity = input.normalized * velocidadMovimiento;
    }

    void DetectarObjetoInteractuable()
    {
        Collider2D[] objetos = Physics2D.OverlapCircleAll(transform.position, rango, capaInteractuable);
        ControladorEstados encontrado = null;
        sillaCercana = null; // reset

        foreach (var col in objetos)
        {
            // Detectar ControladorEstados (tipo genérico)
            var candidato = col.GetComponentInParent<ControladorEstados>();
            if (candidato != null && encontrado == null)
            {
                encontrado = candidato;
            }

            // Detectar si también hay una silla cerca
            var silla = col.GetComponentInParent<InteraccionSilla>();
            if (silla != null && sillaCercana == null)
            {
                sillaCercana = silla;
            }
        }

        if (encontrado != objetoInteractuable)
        {
            objetoInteractuable = encontrado;
            ActualizarUI();
        }
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
}
