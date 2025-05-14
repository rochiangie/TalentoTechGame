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

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player") && objetoActivo != null)
        {
            if (!mensajeUI.gameObject.activeSelf)
                MostrarMensaje();

            if (Input.GetKeyDown(teclaInteraccion))
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

                MostrarMensaje();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (mensajeUI != null)
                mensajeUI.gameObject.SetActive(false);
        }
    }

    private void MostrarMensaje()
    {
        if (mensajeUI != null)
        {
            mensajeUI.text = estadoTapado ? "Presioná E para destapar" : "Presioná E para tapar";
            mensajeUI.gameObject.SetActive(true);
        }
    }
}
