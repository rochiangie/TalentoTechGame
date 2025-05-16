using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ControladorEstados : MonoBehaviour
{
    [Header("Referencias de Estados")]
    [Tooltip("Arrastra aquí el GameObject que representa el estado vacío")]
    [SerializeField] private GameObject estadoVacio;
    [Tooltip("Arrastra aquí el GameObject que representa el estado lleno")]
    [SerializeField] private GameObject estadoLleno;

    [Header("Configuración")]
    [Tooltip("Nombre que se mostrará en la UI")]
    public string nombreMostrado = "Objeto";

    [Header("Debug")]
    [SerializeField] private bool estaLleno = false;
    [SerializeField] private bool debugLogs = true;


    void Start()
    {
        ValidarReferencias();
        InicializarEstados();
    }

    private void ValidarReferencias()
    {
        if (estadoVacio == null || estadoLleno == null)
        {
            Debug.LogError($"[ControladorEstados] Error en {gameObject.name}: Ambos estados (Vacio/Lleno) deben ser asignados en el inspector");
            enabled = false;
            return;
        }

        if (estadoVacio == estadoLleno)
        {
            Debug.LogError($"[ControladorEstados] Error en {gameObject.name}: Los estados Vacio y Lleno no pueden ser el mismo objeto");
            enabled = false;
        }
    }

    private void InicializarEstados()
    {
        estadoVacio.SetActive(!estaLleno);
        estadoLleno.SetActive(estaLleno);

        if (debugLogs)
        {
            Debug.Log($"[ControladorEstados] {gameObject.name} inicializado. Estado inicial: {(estaLleno ? "Lleno" : "Vacío")}");
        }
    }

    public void AlternarEstado()
    {
        if (!enabled) return;

        var jugador = FindObjectOfType<InteraccionJugador>();

        if (jugador != null && jugador.EstaLlevandoObjeto())
        {
            GameObject objeto = jugador.ObjetoTransportado;

            if (objeto != null && objeto.CompareTag("Platos"))
            {
                // Guardar los platos
                jugador.SoltarYDestruirObjeto();
                estaLleno = true;
                ActualizarEstados();

                if (debugLogs)
                    Debug.Log("Platos guardados en el cabinet");

                return;
            }
        }

        // Cambio normal de estado (vacío <-> lleno)
        estaLleno = !estaLleno;
        ActualizarEstados();

        if (debugLogs)
        {
            Debug.Log($"[ControladorEstados] {gameObject.name} alternado a {(estaLleno ? "Lleno" : "Vacío")}", gameObject);
        }
    }


    private void ActualizarEstados()
    {
        estadoVacio.SetActive(!estaLleno);
        estadoLleno.SetActive(estaLleno);
    }

    public string ObtenerNombreEstado()
    {
        return $"{nombreMostrado} ({(estaLleno ? "Lleno" : "Vacío")})";
    }

    public void SetEstado(bool lleno)
    {
        estaLleno = lleno;
        ActualizarEstados();
    }

    void OnValidate()
    {
        if (estadoVacio != null && estadoLleno != null && estadoVacio != estadoLleno)
        {
            ActualizarEstados();
        }
    }
}
