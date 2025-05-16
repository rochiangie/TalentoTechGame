using UnityEngine;

public class InteraccionSofa : MonoBehaviour
{
    public GameObject sofaActivo;
    public GameObject sofaAlternativo;
    public KeyCode tecla = KeyCode.E;
    private bool jugadorCerca = false;

    void Update()
    {
        if (jugadorCerca && Input.GetKeyDown(tecla))
        {
            sofaActivo.SetActive(false);
            sofaAlternativo.SetActive(true);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorCerca = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorCerca = false;
        }
    }
}
