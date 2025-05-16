using UnityEngine;
using TMPro;

public class MostrarHoraAlTocarReloj : MonoBehaviour
{
    public TextMeshProUGUI textoHora;
    public float tiempoVisible = 3f;

    private float tiempoOcultar = 0f;

    void Update()
    {
        // Ocultar el texto después del tiempo
        if (textoHora != null && Time.time >= tiempoOcultar && textoHora.gameObject.activeSelf)
        {
            textoHora.gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Reloj") && textoHora != null)
        {
            string horaActual = System.DateTime.Now.ToString("HH:mm:ss");
            textoHora.text = $"Hora: {horaActual}";
            textoHora.gameObject.SetActive(true);
            tiempoOcultar = Time.time + tiempoVisible;

            Debug.Log("Tocaste el reloj a las " + horaActual);
        }
    }
}
