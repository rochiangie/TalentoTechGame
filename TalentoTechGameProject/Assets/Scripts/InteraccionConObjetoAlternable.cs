using UnityEngine;

public class InteraccionConObjetoAlternable : MonoBehaviour
{
    public GameObject prefabTapado; // Prefab 1
    public GameObject prefabLleno;  // Prefab 2
    public KeyCode teclaInteraccion = KeyCode.E;
    public GameObject mensajeUI;

    private GameObject objetoEnZona;
    private bool estaLleno = false;
    private bool enZonaDeInteraccion = false;

    void Start()
    {
        if (mensajeUI != null)
            mensajeUI.SetActive(false);
    }

    void Update()
    {
        if (enZonaDeInteraccion && objetoEnZona != null && Input.GetKeyDown(teclaInteraccion))
        {
            Vector3 posicion = objetoEnZona.transform.position;
            Quaternion rotacion = objetoEnZona.transform.rotation;

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

            // 👇 Reasignar manualmente si sigue en zona
            if (enZonaDeInteraccion)
            {
                mensajeUI?.SetActive(true);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("InodoroInteractuable"))
        {
            objetoEnZona = other.gameObject;
            enZonaDeInteraccion = true;
            mensajeUI?.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject == objetoEnZona)
        {
            objetoEnZona = null;
            enZonaDeInteraccion = false;
            mensajeUI?.SetActive(false);
        }
    }
}
