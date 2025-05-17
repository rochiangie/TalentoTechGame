using System.Collections;
using UnityEngine;

public class CambioJugador : MonoBehaviour
{
    [Header("Prefabs de Jugador")]
    public GameObject prefabConGravedad;
    public GameObject prefabSinGravedad;

    [Header("Puntos de Aparición")]
    public Transform apareceAqui;
    public Transform aparecerAqui;
    public Transform aparicionFinal;

    public string tagJugador = "Player";

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag(tagJugador)) return;

        GameObject jugador = GetJugadorActivo(); // Aseguramos que sea el único jugador

        if (jugador == null) return;

        string tagTrigger = gameObject.tag;
        string nombreActual = jugador.name;

        if (tagTrigger == "Shit2")
        {
            if (nombreActual.Contains("SinGravedad"))
                StartCoroutine(ReemplazarJugador(jugador, prefabConGravedad, aparecerAqui.position));
            else if (nombreActual.Contains("ConGravedad"))
                StartCoroutine(ReemplazarJugador(jugador, prefabSinGravedad, apareceAqui.position));
        }
        else if (tagTrigger == "Shift")
        {
            StartCoroutine(ReemplazarJugador(jugador, prefabSinGravedad, aparicionFinal.position));
        }
        else if (tagTrigger == "Agrandar")
        {
            if (!nombreActual.Contains("ConGravedad") && !nombreActual.Contains("SinGravedad"))
                StartCoroutine(ReemplazarJugador(jugador, prefabSinGravedad, apareceAqui.position));
        }
    }

    // ✅ Obtiene el único jugador activo con el tag "Player"
    private GameObject GetJugadorActivo()
    {
        GameObject[] jugadores = GameObject.FindGameObjectsWithTag(tagJugador);
        if (jugadores.Length == 1)
        {
            return jugadores[0];
        }
        else
        {
            Debug.LogWarning($"[ADVERTENCIA] Se encontraron {jugadores.Length} objetos con tag 'Player'.");
            // Devuelve el primero, pero podrías forzar destrucción de todos excepto uno si querés.
            return jugadores[0];
        }
    }

    // ✅ Reemplaza al jugador actual y ajusta cámara
    private IEnumerator ReemplazarJugador(GameObject jugadorActual, GameObject nuevoPrefab, Vector3 posicion)
    {
        Debug.Log($"[DEBUG] Reemplazando a: {jugadorActual.name}");

        Destroy(jugadorActual);
        yield return null;

        GameObject nuevoJugador = Instantiate(nuevoPrefab, posicion, Quaternion.identity);
        nuevoJugador.tag = tagJugador;
        Debug.Log($"[DEBUG] Nuevo jugador instanciado: {nuevoJugador.name} en {posicion}");

        Camera camara = Camera.main;
        if (camara != null)
        {
            CamaraSeguirJugador seguir = camara.GetComponent<CamaraSeguirJugador>();
            if (seguir != null)
            {
                seguir.EstablecerObjetivo(nuevoJugador.transform);

                // Zoom + offset dinámico
                if (nuevoJugador.name.Contains("SinGravedad") || nuevoJugador.name.Contains("ConGravedad"))
                {
                    camara.orthographicSize = 18f;
                    seguir.offset = new Vector3(0, 3f, -10);
                    Debug.Log("[DEBUG] Cámara ajustada para jugador con/sin gravedad");
                }
                else
                {
                    camara.orthographicSize = 10f;
                    seguir.offset = new Vector3(0, 1.5f, -10);
                    Debug.Log("[DEBUG] Cámara restaurada para jugador inicial");
                }
            }
        }
    }
}
