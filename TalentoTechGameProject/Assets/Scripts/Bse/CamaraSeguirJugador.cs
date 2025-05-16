using UnityEngine;

public class CamaraSeguirJugador : MonoBehaviour
{
    public Transform objetivo;
    public float suavizado = 0.125f;
    public Vector3 offset;

    void LateUpdate()
    {
        if (objetivo == null) return;

        Vector3 posicionDeseada = objetivo.position + offset;
        Vector3 posicionSuavizada = Vector3.Lerp(transform.position, posicionDeseada, suavizado);
        transform.position = new Vector3(posicionSuavizada.x, posicionSuavizada.y, transform.position.z);
    }
}
