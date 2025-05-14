using UnityEngine;
using TMPro;

public class ZonaInteraccion : MonoBehaviour
{
    public TextMeshProUGUI mensajeUI;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            mensajeUI.gameObject.SetActive(true); // Solo lo activa, sin modificar el texto
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            mensajeUI.gameObject.SetActive(false); // Lo oculta al salir
        }
    }
}
