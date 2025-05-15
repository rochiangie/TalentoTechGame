using UnityEngine;
using TMPro;
using System.Collections;

public class InteraccionPuerta : MonoBehaviour
{
    public KeyCode teclaAbrir = KeyCode.R;
    public TextMeshProUGUI mensajeUI;
    public GameObject puertaPrefab;
    public float tiempoReaparicion = 2f;  // Tiempo de espera antes de reaparecer

    private bool jugadorCerca = false;
    private bool puertaAbierta = false;
    private Transform jugador;
    private Vector3 posicionOriginal;

    void Start()
    {
        posicionOriginal = transform.position;

        if (mensajeUI != null)
            mensajeUI.gameObject.SetActive(false);
    }

    void Update()
    {
        if (jugadorCerca && !puertaAbierta && Input.GetKeyDown(teclaAbrir))
        {
            AbrirPuerta();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorCerca = true;
            jugador = other.transform;

            if (mensajeUI != null)
            {
                //mensajeUI.text = $"Presiona {teclaAbrir}";
                mensajeUI.gameObject.SetActive(true);
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorCerca = false;

            if (mensajeUI != null)
                mensajeUI.gameObject.SetActive(false);

            if (puertaAbierta)
                StartCoroutine(ReinstanciarPuertaConRetraso());
        }
    }

    void AbrirPuerta()
    {
        puertaAbierta = true;

        if (mensajeUI != null)
            mensajeUI.gameObject.SetActive(false);

        Debug.Log($"[PUERTA] Destruyendo: {gameObject.name}");
        Destroy(gameObject);
    }

    IEnumerator ReinstanciarPuertaConRetraso()
    {
        yield return new WaitForSeconds(tiempoReaparicion);

        if (puertaPrefab != null)
        {
            Instantiate(puertaPrefab, posicionOriginal, Quaternion.identity);
            Debug.Log("[PUERTA] Se volvió a crear la puerta.");
        }
    }
}
