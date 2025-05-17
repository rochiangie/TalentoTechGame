using UnityEngine;

public class CamaraSeguirJugador : MonoBehaviour
{
    public Vector3 offset;
    public float smoothSpeed = 0.1f;
    private Transform objetivo;

    void Start()
    {
        BuscarJugador();
    }

    void LateUpdate()
    {
        if (objetivo == null)
        {
            BuscarJugador();
            return;
        }

        Vector3 posicionDeseada = objetivo.position + offset;
        Vector3 posicionSuavizada = Vector3.Lerp(transform.position, posicionDeseada, smoothSpeed);
        transform.position = new Vector3(posicionSuavizada.x, posicionSuavizada.y, transform.position.z);
    }

    public void EstablecerObjetivo(Transform nuevoObjetivo)
    {
        objetivo = nuevoObjetivo;
    }

    void BuscarJugador()
    {
        GameObject jugador = GameObject.FindGameObjectWithTag("Player");
        if (jugador != null)
        {
            objetivo = jugador.transform;
            Debug.Log($"[Cámara] Ahora siguiendo a: {jugador.name}");
        }
    }
}
