using UnityEngine;

public class ObjetoInteractuable : MonoBehaviour
{
    [Header("Detección del jugador")]
    public string tagDelJugador = "Player";

    [Header("Estados visuales")]
    public GameObject estadoInicial;  // Por ejemplo: sprite vacío, apagado
    public GameObject estadoAlterno;  // Por ejemplo: sprite lleno, encendido
    public bool iniciarEnEstadoAlterno = false;

    private bool enEstadoAlterno = false;

    private void Start()
    {
        enEstadoAlterno = iniciarEnEstadoAlterno;
        ActualizarEstadoVisual();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(tagDelJugador))
        {
            Animator animator = other.GetComponent<Animator>();
            if (animator != null)
            {
                animator.SetBool("isTouchingObject", true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(tagDelJugador))
        {
            Animator animator = other.GetComponent<Animator>();
            if (animator != null)
            {
                animator.SetBool("isTouchingObject", false);
            }
        }
    }

    public void AlternarEstado()
    {
        enEstadoAlterno = !enEstadoAlterno;
        ActualizarEstadoVisual();

        Debug.Log($"[{gameObject.name}] alternado a: {(enEstadoAlterno ? "Estado alterno" : "Estado inicial")}", this);
    }

    private void ActualizarEstadoVisual()
    {
        if (estadoInicial != null)
            estadoInicial.SetActive(!enEstadoAlterno);

        if (estadoAlterno != null)
            estadoAlterno.SetActive(enEstadoAlterno);
    }

    public string ObtenerNombreEstado()
    {
        return enEstadoAlterno ? "activo" : "inactivo";
    }
}
