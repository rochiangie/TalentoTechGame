using UnityEngine;

public class CamaraSeguirJugador : MonoBehaviour
{
    public Vector3 offset = new Vector3(0, 1.5f, -10);
    public float smoothSpeed = 0.1f;
    private Transform objetivo;

    void Start()
    {
        GameObject jugador = GameObject.FindGameObjectWithTag("Player");
        if (jugador != null)
        {
            objetivo = jugador.transform;
            Debug.Log("[Cámara] Siguiendo al jugador inicial.");
        }
    }

    void LateUpdate()
    {
        if (objetivo == null) return;

        Vector3 posicionDeseada = objetivo.position + offset;
        Vector3 posicionSuavizada = Vector3.Lerp(transform.position, posicionDeseada, smoothSpeed);
        transform.position = new Vector3(posicionSuavizada.x, posicionSuavizada.y, transform.position.z);
    }

    public void EstablecerObjetivo(Transform nuevoObjetivo)
    {
        objetivo = nuevoObjetivo;
        Debug.Log("[Cámara] Nuevo objetivo establecido: " + nuevoObjetivo.name);
    }
}
