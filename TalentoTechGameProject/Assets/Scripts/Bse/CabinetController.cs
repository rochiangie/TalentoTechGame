using UnityEngine;

public class CabinetController : MonoBehaviour
{
    public GameObject estadoVacio;
    public GameObject estadoLleno;

    private bool yaLleno = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (yaLleno) return;

        if (other.CompareTag("Platos"))
        {
            LlenarGabinete(other.gameObject);
        }
    }

    private void LlenarGabinete(GameObject objeto)
    {
        yaLleno = true;
        if (estadoVacio != null) estadoVacio.SetActive(false);
        if (estadoLleno != null) estadoLleno.SetActive(true);

        Destroy(objeto); // Destruye los platos
    }
}
