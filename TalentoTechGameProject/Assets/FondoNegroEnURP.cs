using UnityEngine;

public class FondoNegroEnURP : MonoBehaviour
{
    void Start()
    {
        Camera camara = Camera.main;

        if (camara != null)
        {
            camara.clearFlags = CameraClearFlags.SolidColor;
            camara.backgroundColor = Color.black; // o cualquier otro color
        }
    }
}
