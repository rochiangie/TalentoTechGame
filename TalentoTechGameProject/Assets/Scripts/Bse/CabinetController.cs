using UnityEngine;

public class CabinetController : MonoBehaviour
{
    [Header("Estados del Cabinet")]
    [SerializeField] private GameObject estadoVacio;
    [SerializeField] private GameObject estadoLleno;

    [Header("Prefab y Configuración")]
    [SerializeField] private GameObject prefabPlatos;
    [SerializeField] private Transform puntoSpawn;
    [SerializeField] private string tagObjetoRequerido = "Platos";
    [SerializeField] private AudioClip sonidoGuardar;

    public string TagObjetoRequerido => tagObjetoRequerido;
    public GameObject PrefabPlatos => prefabPlatos;

    private bool yaLleno = false;
    private bool jugadorCerca = false;

    private void Awake()
    {
        if (estadoVacio != null) estadoVacio.SetActive(true);
        if (estadoLleno != null) estadoLleno.SetActive(false);
    }

    private void Update()
    {
        if (jugadorCerca && Input.GetKeyDown(KeyCode.E))
        {
            InteraccionJugador jugador = FindObjectOfType<InteraccionJugador>();
            if (jugador == null) return;

            if (!yaLleno && jugador.EstaLlevandoObjeto() && jugador.ObjetoTransportado.CompareTag(tagObjetoRequerido))
            {
                IntentarGuardarPlatos(jugador);
            }
            else if (yaLleno && !jugador.EstaLlevandoObjeto())
            {
                SacarPlatosDelGabinete(jugador);
            }
        }
    }

    public void IntentarGuardarPlatos(InteraccionJugador jugador)
    {
        GameObject objeto = jugador.ObjetoTransportado;

        if (yaLleno)
        {
            Debug.Log("🧺 El gabinete ya está lleno.");
            return;
        }

        if (objeto == null)
        {
            Debug.LogWarning("❌ No se está llevando ningún objeto.");
            return;
        }

        if (!objeto.CompareTag(tagObjetoRequerido))
        {
            Debug.LogWarning($"❌ El objeto no tiene el tag requerido: {tagObjetoRequerido}");
            return;
        }

        Debug.Log("✔ Intentando guardar platos en gabinete");

        // CAMBIO DE ESTADO
        estadoVacio.SetActive(false);
        estadoLleno.SetActive(true);
        yaLleno = true;

        // SONIDO
        if (sonidoGuardar != null)
            AudioSource.PlayClipAtPoint(sonidoGuardar, transform.position);

        // DESTRUIR OBJETO
        jugador.SoltarYDestruirObjeto();
    }


    public void SacarPlatosDelGabinete(InteraccionJugador jugador)
    {
        if (yaLleno && !jugador.EstaLlevandoObjeto())
        {
            GameObject nuevoObjeto = Instantiate(prefabPlatos, puntoSpawn.position, Quaternion.identity);
            jugador.RecogerObjeto(nuevoObjeto);

            if (estadoVacio != null) estadoVacio.SetActive(true);
            if (estadoLleno != null) estadoLleno.SetActive(false);

            yaLleno = false;
        }
    }

    public bool EstaLleno()
    {
        return yaLleno;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorCerca = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorCerca = false;
        }
    }
}
