using UnityEngine;
using TMPro;

public class ZonaInteraccionEstado : MonoBehaviour
{
    public TextMeshProUGUI mensajeUI;
    public GameObject prefabTapado;
    public GameObject prefabLleno;
    public Transform puntoSpawn;
    public KeyCode teclaInteraccion = KeyCode.E;

    private GameObject objetoActivo;
    private bool estadoTapado = true;
    private bool enZona = false;

    void Start()
    {
        if (mensajeUI != null)
            mensajeUI.gameObject.SetActive(false);

        if (prefabTapado != null && puntoSpawn != null)
        {
            objetoActivo = Instantiate(prefabTapado, puntoSpawn.position, Quaternion.identity);
            objetoActivo.tag = "InodoroInteractuable";
            estadoTapado = true;
        }
    }

    void Update()
    {
        if (enZona && objetoActivo != null && Input.GetKeyDown(teclaInteraccion))
        {
            Quaternion rot = objetoActivo.transform.rotation;
            Vector3 escala = objetoActivo.transform.localScale;

            Destroy(objetoActivo);
            objetoActivo = null;

            estadoTapado = !estadoTapado;

            GameObject nuevo = Instantiate(
                estadoTapado ? prefabTapado : prefabLleno,
                puntoSpawn.position,
                rot
            );
            nuevo.transform.localScale = escala;
            nuevo.tag = "InodoroInteractuable";
            objetoActivo = nuevo;

            // Solo muestra el texto, no lo cambia
            MostrarMensaje();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("InodoroInteractuable"))
        {
            enZona = true;
            MostrarMensaje();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("InodoroInteractuable"))
        {
            enZona = false;
            objetoActivo = null;
            if (mensajeUI != null)
                mensajeUI.gameObject.SetActive(false);
        }
    }

    private void MostrarMensaje()
    {
        if (mensajeUI != null)
            mensajeUI.gameObject.SetActive(true); // No modifica el texto
    }
}
