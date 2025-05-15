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
            Debug.Log("Jugador toc� la silla. Trigger animaci�n activado.");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        Animator animJugador = collision.GetComponent<Animator>();
        if (animJugador != null)
        {
            animJugador.SetBool(triggerAnimacionJugador, false);
            Debug.Log("Jugador se alej� de la silla. Trigger desactivado.");
        }
    }
}
