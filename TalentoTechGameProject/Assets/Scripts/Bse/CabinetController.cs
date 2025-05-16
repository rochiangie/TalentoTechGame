using UnityEngine;

public class CabinetController : MonoBehaviour
{
    [Header("Estados del Cabinet")]
    public GameObject estadoVacio;
    public GameObject estadoLleno;
    [SerializeField] private KeyCode teclaInteraccion = KeyCode.E;

    [Header("Configuración")]
    [SerializeField] private string tagObjetoRequerido = "Platos";
    [SerializeField] private AudioClip sonidoGuardar;

    private bool yaLleno = false;
    private InteraccionJugador jugador;
    private bool jugadorCerca = false;

    private void Awake()
    {
        // Inicializar estados visuales
        if (estadoVacio != null) estadoVacio.SetActive(true);
        if (estadoLleno != null) estadoLleno.SetActive(false);

        jugador = FindObjectOfType<InteraccionJugador>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(teclaInteraccion) && jugadorCerca && !yaLleno)
        {
            ProcesarInteraccion();
        }
    }

    private void ProcesarInteraccion()
    {
        if (jugador != null && jugador.EstaLlevandoObjeto() &&
            jugador.ObjetoTransportado != null &&
            jugador.ObjetoTransportado.CompareTag(tagObjetoRequerido))
        {
            LlenarGabinete(jugador.ObjetoTransportado);
            jugador.SoltarYDestruirObjeto();

            if (sonidoGuardar != null)
            {
                AudioSource.PlayClipAtPoint(sonidoGuardar, transform.position);
            }
        }
    }

    private void LlenarGabinete(GameObject objeto)
    {
        yaLleno = true;

        if (estadoVacio != null) estadoVacio.SetActive(false);
        if (estadoLleno != null) estadoLleno.SetActive(true);

        Destroy(objeto);
        Debug.Log($"{tagObjetoRequerido} guardados en el cabinet");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorCerca = true;
        }
        else if (!yaLleno && other.CompareTag(tagObjetoRequerido) && other.transform.parent == null)
        {
            LlenarGabinete(other.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorCerca = false;
        }
    }

    // Método para reiniciar el cabinet (opcional)
    public void ReiniciarCabinet()
    {
        yaLleno = false;
        if (estadoVacio != null) estadoVacio.SetActive(true);
        if (estadoLleno != null) estadoLleno.SetActive(false);
    }
}