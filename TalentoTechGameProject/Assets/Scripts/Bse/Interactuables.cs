using UnityEngine;
using TMPro;

public class Interactuables : MonoBehaviour
{
    [Header("Configuración")]
    public float rangoInteraccion = 1.5f;
    public KeyCode teclaInteraccion = KeyCode.E;
    public LayerMask capaInteractuable;

    [Header("UI")]
    public TextMeshProUGUI textoInteraccion;

    private ControladorEstados objetoInteractuable;

    void Update()
    {
        DetectarObjetos();

        if (Input.GetKeyDown(teclaInteraccion) && objetoInteractuable != null)
        {
            Interactuar();
        }
    }

    void DetectarObjetos()
    {
        Collider2D[] objetos = Physics2D.OverlapCircleAll(transform.position, rangoInteraccion, capaInteractuable);

        ControladorEstados nuevo = null;

        foreach (var col in objetos)
        {
            nuevo = col.GetComponentInParent<ControladorEstados>();
            if (nuevo != null)
                break;
        }

        if (nuevo != objetoInteractuable)
        {
            objetoInteractuable = nuevo;
            ActualizarUI();
        }

        if (objetoInteractuable == null)
        {
            ActualizarUI("");
        }
    }

    void Interactuar()
    {
        if (objetoInteractuable != null)
        {
            objetoInteractuable.AlternarEstado();
        }
        else
        {
            Debug.LogWarning("No se encontró ControladorEstados al interactuar.");
        }
    }

    void ActualizarUI()
    {
        if (textoInteraccion == null) return;

        if (objetoInteractuable != null)
        {
            string nombre = objetoInteractuable.ObtenerNombreEstado();
            //textoInteraccion.text = $"Presiona {teclaInteraccion} para usar {nombre}";
            textoInteraccion.gameObject.SetActive(true);
        }
        else
        {
            textoInteraccion.gameObject.SetActive(false);
        }
    }

    void ActualizarUI(string mensaje)
    {
        if (textoInteraccion != null)
        {
            textoInteraccion.text = mensaje;
            textoInteraccion.gameObject.SetActive(!string.IsNullOrEmpty(mensaje));
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, rangoInteraccion);
    }
}
