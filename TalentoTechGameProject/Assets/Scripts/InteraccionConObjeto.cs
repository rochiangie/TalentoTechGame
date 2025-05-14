using UnityEngine;
using TMPro;

public class InteraccionConObjeto : MonoBehaviour
{
    public string tagObjetoInteractuable = "InodoroTapado";
    public GameObject prefabObjetoNuevo; // El inodoro lleno
    public KeyCode teclaInteraccion = KeyCode.E;
    public GameObject mensajeUI; // Texto en pantalla

    private GameObject objetoEnZona;

    void Update()
    {
        if (objetoEnZona != null)
        {
            if (Input.GetKeyDown(teclaInteraccion))
            {
                Vector3 posicion = objetoEnZona.transform.position;
                Quaternion rotacion = objetoEnZona.transform.rotation;

                Destroy(objetoEnZona);
                Instantiate(prefabObjetoNuevo, posicion, rotacion);
                mensajeUI.SetActive(false);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(tagObjetoInteractuable))
        {
            objetoEnZona = other.gameObject;
            mensajeUI.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject == objetoEnZona)
        {
            objetoEnZona = null;
            mensajeUI.SetActive(false);
        }
    }
}
