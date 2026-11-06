using UnityEngine;
using TMPro;

public static class ButtonLabel
{
    // Crea un texto flotando encima de un boton, mirando hacia el jugador
    public static void Create(Transform button, string text, float height, float fontSize)
    {
        GameObject label = new GameObject("Label " + text);
        label.transform.position = button.position + Vector3.up * height;

        if (Camera.main != null)
        {
            Vector3 direction = label.transform.position - Camera.main.transform.position;
            direction.y = 0f;
            if (direction != Vector3.zero) label.transform.rotation = Quaternion.LookRotation(direction);
        }

        TextMeshPro textMesh = label.AddComponent<TextMeshPro>();
        textMesh.text = text;
        textMesh.fontSize = fontSize;
        textMesh.alignment = TextAlignmentOptions.Center;
        textMesh.rectTransform.sizeDelta = new Vector2(2f, 0.5f);
    }
}
