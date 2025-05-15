using UnityEngine;

public class InteraccionSilla : MonoBehaviour
{
    public string triggerAnimacionJugador = "isTouchingObject";
    public float tiempoAntesDeDestruir = 1.5f;

    private bool yaInteractuado = false;

    public void EjecutarAccion(GameObject jugador)
    {
        if (yaInteractuado) return;

        Animator animJugador = jugador.GetComponent<Animator>();
        if (animJugador != null)
        {
            animJugador.SetBool(triggerAnimacionJugador, true);
        }

        yaInteractuado = true;
        Invoke(nameof(DestruirSilla), tiempoAntesDeDestruir);
    }

    void DestruirSilla()
    {
        Destroy(gameObject);
    }
}
