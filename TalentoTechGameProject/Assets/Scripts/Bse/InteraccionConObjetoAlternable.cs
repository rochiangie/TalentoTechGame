using UnityEngine;
using TMPro;

public class InteraccionConObjetoAlternable : MonoBehaviour
{
    public TextMeshProUGUI mensajeUI;
    public GameObject prefabTapado;
    public GameObject prefabLleno;
    public Transform puntoSpawn;
    public KeyCode teclaInteraccion = KeyCode.E;

    private GameObject objetoActivo;
    private bool estadoTapado = true;
    private bool puedeInteractuar = false;

    void Start()
    {
        if (mensajeUI != null)
            mensajeUI.gameObject.SetActive(false);
        else
            Debug.LogError("mensajeUI no está asignado.");

        if (prefabTapado != null && puntoSpawn != null)
        {
            objetoActivo = Instantiate(prefabTapado, puntoSpawn.position, Quaternion.identity);
            objetoActivo.tag = "InodoroInteractuable";
            estadoTapado = true;
        }
    }

    void Update()
    {
        if (puedeInteractuar && objetoActivo != null && Input.GetKeyDown(teclaInteraccion))
        {
            Quaternion rot = objetoActivo.transform.rotation;
            Vector3 escala = objetoActivo.transform.localScale;

            Destroy(objetoActivo);
            estadoTapado = !estadoTapado;

            GameObject nuevo = Instantiate(
                estadoTapado ? prefabTapado : prefabLleno,
                puntoSpawn.position,
                rot
            );
            nuevo.transform.localScale = escala;
            nuevo.tag = "InodoroInteractuable";
            objetoActivo = nuevo;

            if (mensajeUI != null)
                mensajeUI.gameObject.SetActive(true); // Solo activamos el objeto, no cambiamos el texto
        }
    }

    public void SetObjetoActivo(GameObject nuevo)
    {
        if (nuevo == null || objetoActivo == nuevo) return;

        objetoActivo = nuevo;
        puedeInteractuar = true;
    }

    public void ClearObjetoActivo()
    {
        puedeInteractuar = false;
        if (mensajeUI != null)
            mensajeUI.gameObject.SetActive(false);
    }

    private void MostrarMensaje()
    {
        if (mensajeUI != null)
        {
            //mensajeUI.text = estadoTapado ? "Presioná E para destapar" : "Presioná E para tapar";
            mensajeUI.gameObject.SetActive(true);
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("InodoroInteractuable"))
        {
            objetoActivo = other.gameObject;
            puedeInteractuar = true;

            if (mensajeUI != null)
                mensajeUI.gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("InodoroInteractuable"))
        {
            objetoActivo = null;
            puedeInteractuar = false;

            if (mensajeUI != null)
                mensajeUI.gameObject.SetActive(false);
        }
    }


}
