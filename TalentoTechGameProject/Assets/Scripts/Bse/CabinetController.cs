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
        input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        if (input.x != 0)
        {
            Vector3 escala = transform.localScale;
            escala.x = Mathf.Sign(input.x) * Mathf.Abs(escala.x);
            transform.localScale = escala;
        }

        bool corriendo = Input.GetKey(teclaCorrer);
        animator.SetBool("isRunning", corriendo && input != Vector2.zero);
        animator.SetBool("isWalking", !corriendo && input != Vector2.zero);

        DetectarObjetosCercanos();

        if (Input.GetKeyDown(teclaInteraccion))
        {
            if (gabinetePlatosCercano != null)
            {
                Debug.Log("\u2714 Intentando guardar platos en gabinete");
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

            if (objetoInteractuableCercano != null)
            {
                objetoInteractuableCercano.AlternarEstado();
                ActualizarUI();
                return;
            }

            if (!llevaObjeto && objetoCercanoRecogible != null && EsRecogible(objetoCercanoRecogible.tag))
            {
                RecogerObjeto(objetoCercanoRecogible);
                return;
            }

            if (!llevaObjeto && sillaCercana != null)
            {
                sillaCercana.EjecutarAccion(gameObject);
            }
        }

        ActualizarUI();
    }

    void FixedUpdate()
    {
        float velocidadFinal = Input.GetKey(teclaCorrer) ? velocidadCorrer : velocidadMovimiento;
        rb.velocity = input.normalized * velocidadFinal;
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
            if (interactuable != null) objetoInteractuableCercano = interactuable;

            var gabinete = col.GetComponent<CabinetController>();
            if (gabinete != null) gabinetePlatosCercano = gabinete;

            if (!llevaObjeto && EsRecogible(col.tag)) objetoCercanoRecogible = col.gameObject;

            var silla = col.GetComponent<InteraccionSilla>();
            if (silla != null) sillaCercana = silla;
        }
    }

    void ActualizarUI(string mensaje = "")
    {
        if (mensajeUI == null) return;

        if (!string.IsNullOrEmpty(mensaje))
        {
            mensajeUI.text = mensaje;
        }
        else if (gabinetePlatosCercano != null)
        {
            mensajeUI.text = llevaObjeto && !gabinetePlatosCercano.EstaLleno()
                ? $"Presiona {teclaInteraccion} para guardar {gabinetePlatosCercano.TagObjetoRequerido}"
                : $"Presiona {teclaInteraccion} para sacar {gabinetePlatosCercano.PrefabPlatos.name}";
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

        SpriteRenderer srJugador = GetComponent<SpriteRenderer>();
        SpriteRenderer srObjeto = objeto.GetComponent<SpriteRenderer>();
        if (srJugador != null && srObjeto != null)
        {
            srObjeto.sortingLayerName = srJugador.sortingLayerName;
            srObjeto.sortingOrder = srJugador.sortingOrder + 1;
        }

        Collider2D col = objeto.GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        Rigidbody2D rb = objeto.GetComponent<Rigidbody2D>();
        if (rb != null) rb.simulated = false;

        objetoCercanoRecogible = null;
        ActualizarUI();
    }

    public void SoltarYDestruirObjeto()
    {
        if (!llevaObjeto || objetoTransportado == null) return;

        Destroy(objetoTransportado);
        objetoTransportado = null;
        llevaObjeto = false;
        ActualizarUI();
    }

    private bool EsRecogible(string tag)
    {
        foreach (string recogible in tagsRecogibles)
        {
            if (tag == recogible) return true;
        }
        return false;
    }

    public bool EstaLlevandoObjeto() => llevaObjeto;
}
