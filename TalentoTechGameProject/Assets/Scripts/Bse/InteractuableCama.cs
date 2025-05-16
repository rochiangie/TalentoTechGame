using UnityEngine;

public class InteractuableCama : MonoBehaviour
{
    public Sprite camaOrdenada;
    public Sprite camaDesordenada;
    private bool estaDesordenada = false;

    private SpriteRenderer sr;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        sr.sprite = camaOrdenada;
    }

    public void Interactuar()
    {
        estaDesordenada = !estaDesordenada;
        sr.sprite = estaDesordenada ? camaDesordenada : camaOrdenada;
    }
}
