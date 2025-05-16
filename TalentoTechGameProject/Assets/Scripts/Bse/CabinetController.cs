using UnityEngine;

public class CabinetController : MonoBehaviour
{
    [Header("Estados del Cabinet")]
    public GameObject estadoVacio;
    public GameObject estadoLleno;

    [Header("Platos")]
    public GameObject prefabPlatos;

    [Header("Configuraci�n")]
    [SerializeField] private KeyCode teclaInteraccion = KeyCode.E;
    [SerializeField] public string tagObjetoRequerido = "Platos"; // �Ahora es public!
    [SerializeField] private AudioClip sonidoGuardar;

    private bool estaLleno = false;
    private bool jugadorCerca = false;

    public bool EstaLleno() { return estaLleno; }

    private void Awake()
    {
        if (estadoVacio != null) estadoVacio.SetActive(true);
        if (estadoLleno != null) estadoLleno.SetActive(false);
    }

    private void Update()
    {
        // La l�gica de Input se movi� a InteraccionJugador
    }

    public void IntentarGuardarPlatos(InteraccionJugador jugador)
    {
        if (!estaLleno && jugador != null && jugador.EstaLlevandoObjeto())
        {
            GameObject obj = jugador.ObjetoTransportado;

            if (obj != null && obj.CompareTag(tagObjetoRequerido))
            {
                jugador.SoltarYDestruirObjeto();

                estaLleno = true;
                estadoVacio.SetActive(false);
                estadoLleno.SetActive(true);

                if (sonidoGuardar != null)
                    AudioSource.PlayClipAtPoint(sonidoGuardar, transform.position);

                Debug.Log("Platos guardados en el cabinet.");
            }
        }
    }

    public void SacarPlatosDelGabinete(InteraccionJugador jugador)
    {
        if (estaLleno && jugador != null && !jugador.EstaLlevandoObjeto())
        {
            if (prefabPlatos == null) return;

            // Crear instancia de platos en el punto de carga del jugador
            Transform puntoCarga = jugador.transform.Find("PuntoCarga"); // Aseg�rate que se llame as�
            if (puntoCarga == null)
            {
                Debug.LogWarning("No se encontr� el punto de carga en el jugador.");
                return;
            }

            GameObject nuevosPlatos = Instantiate(prefabPlatos, puntoCarga.position, Quaternion.identity);
            jugador.RecogerObjeto(nuevosPlatos);

            estaLleno = false;
            estadoVacio.SetActive(true);
            estadoLleno.SetActive(false);

            Debug.Log("Platos recuperados del gabinete.");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // No necesitamos detectar al jugador aqu�, la detecci�n se hace en InteraccionJugador
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // No necesitamos detectar al jugador aqu�
    }
}