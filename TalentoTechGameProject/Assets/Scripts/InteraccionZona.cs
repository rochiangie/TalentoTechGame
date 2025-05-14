using UnityEngine;

public class InteraccionZona : MonoBehaviour
{
    public GameObject prefabTapado;
    public GameObject prefabLleno;
    public GameObject mensajeUI;
    public KeyCode teclaInteraccion = KeyCode.E;

    private GameObject objetoEnZona;
    private bool estaLleno = false;

    void Update()
    {
        if (objetoEnZona != null && Input.GetKeyDown(teclaInteraccion))
        {
            Vector3 posicion = objetoEnZona.transform.position;
            Quaternion rotacion = objetoEnZona.transform.rotation;
            Vector3 escala = objetoEnZona.transform.localScale;

            Destroy(objetoEnZona);

            if (!estaLleno)
            {
                objetoEnZona = Instantiate(prefabLleno, posicion, rotacion);
                estaLleno = true;
            }
            else
            {
                objetoEnZona = Instantiate(prefabTapado, posicion, rotacion);
                estaLleno = false;
            }

            objetoEnZona.transform.localScale = escala;
            mensajeUI?.SetActive(true);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("InodoroInteractuable"))
        {
            objetoEnZona = other.gameObject;
            mensajeUI?.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (objetoEnZona != null && other.gameObject == objetoEnZona)
        {
            objetoEnZona = null;
            mensajeUI?.SetActive(false);
        }
    }
}
