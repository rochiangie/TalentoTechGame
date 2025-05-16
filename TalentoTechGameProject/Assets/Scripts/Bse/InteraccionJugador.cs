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

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;

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

        // Animaciones
        bool corriendo = Input.GetKey(teclaCorrer);
        animator.SetBool("isRunning", corriendo && input != Vector2.zero);
        animator.SetBool("isWalking", !corriendo && input != Vector2.zero);

        // Voltear sprite
        if (input.x != 0)
        {
            Vector3 escala = transform.localScale;
            escala.x = Mathf.Sign(input.x) * Mathf.Abs(escala.x);
            transform.localScale = escala;
        }

        DetectarObjetosCercanos();
    }

    void FixedUpdate()
    {
        float velocidad = Input.GetKey(teclaCorrer) ? velocidadCorrer : velocidadMovimiento;
        rb.linearVelocity = input * velocidad;
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
            if (col.TryGetComponent(out ControladorEstados est)) objetoInteractuableCercano = est;
            if (col.TryGetComponent(out CabinetController cab)) gabinetePlatosCercano = cab;
            if (!llevaObjeto && EsRecogible(col.tag)) objetoCercanoRecogible = col.gameObject;
            if (col.TryGetComponent(out InteraccionSilla silla)) sillaCercana = silla;
        }

        ActualizarUI();
    }

    void ActualizarUI()
    {
        if (mensajeUI == null) return;

        if (gabinetePlatosCercano != null)
        {
            mensajeUI.text = gabinetePlatosCercano.EstaLleno()
                ? $"Presiona {teclaInteraccion} para sacar {gabinetePlatosCercano.PrefabPlatos.name}"
                : $"Presiona {teclaInteraccion} para guardar {gabinetePlatosCercano.TagObjetoRequerido}";
        }
        else if (objetoInteractuableCercano != null)
        {
            mensajeUI.text = $"Presiona {teclaInteraccion} para usar {objetoInteractuableCercano.ObtenerNombreEstado()}";
        }
        else if (objetoCercanoRecogible != null && !llevaObjeto)
        {
            mensajeUI.text = $"Presiona {teclaInteraccion} para recoger {objetoCercanoRecogible.tag}";
        }
        else
        {
            mensajeUI.text = "";
        }

        mensajeUI.gameObject.SetActive(!string.IsNullOrEmpty(mensajeUI.text));
    }

    public void RecogerObjeto(GameObject objeto)
    {
        if (llevaObjeto || objeto == null) return;

        llevaObjeto = true;
        objetoTransportado = objeto;

        objeto.transform.SetParent(puntoDeCarga);
        objeto.transform.localPosition = Vector3.zero;
        objeto.transform.localRotation = Quaternion.identity;
        objeto.transform.localScale = Vector3.one;

        if (objeto.TryGetComponent(out Collider2D col)) col.enabled = false;
        if (objeto.TryGetComponent(out Rigidbody2D body)) body.simulated = false;
    }

    public void SoltarYDestruirObjeto()
    {
        if (objetoTransportado != null)
        {
            Destroy(objetoTransportado);
            objetoTransportado = null;
            llevaObjeto = false;
        }
    }

    public bool EstaLlevandoObjeto() => llevaObjeto;

    private bool EsRecogible(string tag) => System.Array.Exists(tagsRecogibles, t => t == tag);

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, rango);
    }
}
