using UnityEngine;

public class ObjetoInteractuable : MonoBehaviour
{
    public string tagDelJugador = "Player";

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(tagDelJugador))
        {
            Animator animator = other.GetComponent<Animator>();
            if (animator != null)
            {
                animator.SetBool("isTouchingObject", true);
                //Debug.Log($"[{gameObject.name}] Jugador tocando objeto. Activado isTouchingObject = true");
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(tagDelJugador))
        {
            Animator animator = other.GetComponent<Animator>();
            if (animator != null)
            {
                animator.SetBool("isTouchingObject", false);
                //Debug.Log($"[{gameObject.name}] Jugador dejó de tocar. isTouchingObject = false");
            }
        }
    }
}
