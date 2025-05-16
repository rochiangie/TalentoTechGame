using UnityEngine;

public class InteraccionSilla : MonoBehaviour
{
    public string triggerAnimacionJugador = "isTouchingObject";

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        Animator animJugador = collision.GetComponent<Animator>();
        if (animJugador != null)
        {
            animJugador.SetBool(triggerAnimacionJugador, true);
            Debug.Log("Jugador tocó la silla. Trigger animación activado.");
        }
    }
    public void EjecutarAccion(GameObject jugador)
    {
        // Lógica para sentarse/levantarse
        var jugadorInteract = jugador.GetComponent<InteraccionJugador>();
        if (jugadorInteract != null && jugadorInteract.EstaLlevandoObjeto())
        {
            Debug.Log("No puedes sentarte mientras llevas un objeto");
            return;
        }

        // Resto de la lógica de la silla...
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        Animator animJugador = collision.GetComponent<Animator>();
        if (animJugador != null)
        {
            animJugador.SetBool(triggerAnimacionJugador, false);
            Debug.Log("Jugador se alejó de la silla. Trigger desactivado.");
        }
    }
}
