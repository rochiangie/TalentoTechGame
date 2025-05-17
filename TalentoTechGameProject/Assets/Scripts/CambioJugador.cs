using System.Collections;
using UnityEngine;

public class CambioJugador : MonoBehaviour
{
    [Header("Prefabs de Jugador")]
    public GameObject prefabConGravedad;
    public GameObject prefabSinGravedad;
    public GameObject prefabOriginal;

    [Header("Puntos de Aparición")]
    public Transform apareceAqui;
    public Transform aparecerAqui;
    public Transform aparicionFinal;
    public Transform puntoDeAparicion;

    [Header("Configuración")]
    public string tagJugador = "Player";

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag(tagJugador)) return;

        GameObject jugador = GetJugadorActivo();
        if (jugador == null) return;

        string tagTrigger = gameObject.tag;
        string nombreActual = jugador.name;

        // Transición especial: PlayerSinGravedad → Player original
        if (nombreActual.Contains("SinGravedad") && tagTrigger == "Agrandar")
        {
            Vector3 spawnPos = puntoDeAparicion != null ? puntoDeAparicion.position : transform.position;
            StartCoroutine(ReemplazarJugador(jugador, prefabOriginal, spawnPos));
            return;
        }

        // Lógica para triggers específicos
        if (tagTrigger == "Shit2")
        {
            if (nombreActual.Contains("SinGravedad"))
                StartCoroutine(ReemplazarJugador(jugador, prefabConGravedad, aparecerAqui.position));
            else if (nombreActual.Contains("ConGravedad"))
                StartCoroutine(ReemplazarJugador(jugador, prefabSinGravedad, apareceAqui.position));
        }
        else if (tagTrigger == "Shift")
        {
            StartCoroutine(ReemplazarJugador(jugador, prefabOriginal, aparicionFinal.position));
        }
        else if (tagTrigger == "Agrandar")
        {
            if (!nombreActual.Contains("ConGravedad") && !nombreActual.Contains("SinGravedad"))
                StartCoroutine(ReemplazarJugador(jugador, prefabSinGravedad, apareceAqui.position));
        }
    }

    private GameObject GetJugadorActivo()
    {
        GameObject[] jugadores = GameObject.FindGameObjectsWithTag(tagJugador);
        if (jugadores.Length == 1) return jugadores[0];

        Debug.LogWarning($"[ADVERTENCIA] Se encontraron {jugadores.Length} objetos con tag 'Player'.");
        return jugadores[0];
    }

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
                    Debug.Log("[DEBUG] Cámara restaurada para jugador original");
                }
            }
        }
    }
}
