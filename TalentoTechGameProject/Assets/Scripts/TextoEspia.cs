using UnityEngine;
using TMPro;

public class TextoEspia : MonoBehaviour
{
    void Update()
    {
        foreach (var texto in FindObjectsOfType<TextMeshProUGUI>(true))
        {
            if (texto.text.ToLower().Contains("presioná e para"))
            {
                Debug.LogWarning("🕵️ Texto encontrado en: " + texto.name + " (" + texto.gameObject.name + ")");
            }
        }
    }
}
