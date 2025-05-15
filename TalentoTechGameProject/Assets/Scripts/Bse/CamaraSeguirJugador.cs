using UnityEngine;

public class CamaraSeguirJugador : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0, 0, -10f);
    public float suavizado = 0.15f;

    private Vector3 velocidad = Vector3.zero;

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 posicionDeseada = new Vector3(target.position.x + offset.x, transform.position.y, transform.position.z);
        transform.position = Vector3.SmoothDamp(transform.position, posicionDeseada, ref velocidad, suavizado);
    }
}
