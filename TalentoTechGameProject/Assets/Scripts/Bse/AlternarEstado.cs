using UnityEngine;

public class AlternarEstado : MonoBehaviour
{
    public GameObject estadoA;
    public GameObject estadoB;
    private bool enEstadoA = true;

    public void Alternar()
    {
        enEstadoA = !enEstadoA;
        if (estadoA != null) estadoA.SetActive(enEstadoA);
        if (estadoB != null) estadoB.SetActive(!enEstadoA);
    }
}
