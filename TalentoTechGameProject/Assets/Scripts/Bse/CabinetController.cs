using UnityEngine;

public class CabinetController : MonoBehaviour
{
    [Header("Estados del Cabinet")]
    public GameObject estadoVacio;
    public GameObject estadoLleno;

    [Header("Platos")]
    public GameObject prefabPlatos;

    [Header("Configuración")]
    [SerializeField] private KeyCode teclaInteraccion = KeyCode.E;
    [SerializeField] public string tagObjetoRequerido = "Platos";
    [SerializeField] private AudioClip sonidoGuardar;

    private bool estaLleno = false;

    public bool EstaLleno() => estaLleno;

    private void Awake()
    {
        if (estadoVacio != null) estadoVacio.SetActive(true);
        if (estadoLleno != null) estadoLleno.SetActive(false);
    }

    public void IntentarGuardarPlatos(InteraccionJugador jugador)
    {
        Debug.Log("🔍 Intentando guardar en gabinete...");

        if (estaLleno)
        {
            Debug.Log("⚠ Ya está lleno.");
            return;
        }

        if (jugador == null)
        {
            Debug.LogWarning("❌ Jugador es null.");
            return;
        }

        GameObject obj = jugador.ObjetoTransportado;

        if (obj == null)
        {
            Debug.LogWarning("❌ No se está llevando ningún objeto.");
            return;
        }

        Debug.Log($"🎯 Objeto tiene tag: {obj.tag}, requerido: {tagObjetoRequerido}");

        if (!obj.CompareTag(tagObjetoRequerido))
        {
            Debug.LogWarning("❌ Tag del objeto no coincide.");
            return;
        }

        jugador.SoltarYDestruirObjeto();

        estaLleno = true;

        if (estadoVacio != null) estadoVacio.SetActive(false);
        if (estadoLleno != null) estadoLleno.SetActive(true);

        if (sonidoGuardar != null)
            AudioSource.PlayClipAtPoint(sonidoGuardar, transform.position);

        Debug.Log("✅ Objeto guardado, estado actualizado.");
    }


    public void SacarPlatosDelGabinete(InteraccionJugador jugador)
    {
        if (estaLleno && jugador != null && !jugador.EstaLlevandoObjeto())
        {
            if (prefabPlatos == null)
            {
                Debug.LogWarning("❌ No hay prefab de platos asignado.");
                return;
            }

            Transform puntoCarga = jugador.transform.Find("PuntoCarga");
            if (puntoCarga == null)
            {
                Debug.LogWarning("❌ No se encontró el punto de carga en el jugador.");
                return;
            }

            GameObject nuevosPlatos = Instantiate(prefabPlatos, puntoCarga.position, Quaternion.identity);
            jugador.RecogerObjeto(nuevosPlatos);

            estaLleno = false;
            if (estadoVacio != null) estadoVacio.SetActive(true);
            if (estadoLleno != null) estadoLleno.SetActive(false);

            Debug.Log("✔ Platos recuperados del gabinete.");
        }
    }

    public string TagObjetoRequerido => tagObjetoRequerido;
    public GameObject PrefabPlatos => prefabPlatos;
}
