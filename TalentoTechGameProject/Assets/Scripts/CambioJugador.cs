using UnityEngine;

public class CambioDeJugador : MonoBehaviour
{
    [Header("Prefabs de Jugador")]
    public GameObject prefabConGravedad;
    public GameObject prefabSinGravedad;

    [Header("Puntos de Aparición")]
    public Transform apareceAqui;       // Para el cambio a SinGravedad desde Shit2 o Agrandar
    public Transform aparecerAqui;      // Para el cambio a ConGravedad desde Shit2
    public Transform aparicionFinal;    // Para el cambio final desde Shift

    public string tagJugador = "Player";

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log($"[DEBUG] Algo entró al trigger: {collision.name}");

        if (!collision.CompareTag(tagJugador))
        {
            Debug.Log($"[DEBUG] {collision.name} no tiene el tag {tagJugador}, no se hace nada.");
            return;
        }

        string tagTrigger = gameObject.tag;
        string nombreActual = collision.gameObject.name;

        Debug.Log($"[DEBUG] Trigger con tag: {tagTrigger}, objeto: {nombreActual}");

        if (tagTrigger == "Shit2")
        {
            if (nombreActual.Contains("SinGravedad"))
            {
                Debug.Log("[DEBUG] Cambiando de SinGravedad a ConGravedad (posición AparecerAqui)");
                CambiarJugador(collision.gameObject, prefabConGravedad, aparecerAqui.position);
            }
            else if (nombreActual.Contains("ConGravedad"))
            {
                Debug.Log("[DEBUG] Cambiando de ConGravedad a SinGravedad (posición ApareceaAqui)");
                CambiarJugador(collision.gameObject, prefabSinGravedad, apareceAqui.position);
            }
        }
        else if (tagTrigger == "Shift")
        {
            Debug.Log("[DEBUG] Cambio final: siempre a SinGravedad (posición AparicionFinal)");
            CambiarJugador(collision.gameObject, prefabSinGravedad, aparicionFinal.position);
        }
        else if (tagTrigger == "Agrandar")
        {
            if (!nombreActual.Contains("ConGravedad") && !nombreActual.Contains("SinGravedad"))
            {
                Debug.Log("[DEBUG] Agrandar activado: de Player normal a SinGravedad (posición ApareceaAqui)");
                CambiarJugador(collision.gameObject, prefabSinGravedad, apareceAqui.position);
            }
            else
            {
                Debug.Log("[DEBUG] Agrandar ignorado: ya es un prefab con o sin gravedad.");
            }
        }
        else
        {
            Debug.Log($"[DEBUG] Trigger con tag no manejado: {tagTrigger}");
        }
    }

    private void CambiarJugador(GameObject jugadorActual, GameObject nuevoPrefab, Vector3 posicion)
    {
        Debug.Log($"[DEBUG] Destruyendo: {jugadorActual.name}");
        Destroy(jugadorActual);

        GameObject nuevoJugador = Instantiate(nuevoPrefab, posicion, Quaternion.identity);
        nuevoJugador.tag = tagJugador;
        Debug.Log($"[DEBUG] Instanciado nuevo jugador: {nuevoJugador.name} en {posicion}");
    }
}
