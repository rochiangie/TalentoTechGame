using UnityEngine;

public class SeguirJugadorUI : MonoBehaviour
{
    public Transform objetivo; // El jugador
    public Vector3 offset = new Vector3(0, 1.5f, 0); // Altura sobre el jugador

    void LateUpdate()
    {
        if (objetivo != null)
            transform.position = objetivo.position + offset;
    }
}
