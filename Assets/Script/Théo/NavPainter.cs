using UnityEngine;

public class NavPainter : MonoBehaviour
{
    [Header("Brush Settings")]
    public float brushSize = 2f;
    public Color brushColor = new Color(1f, 0f, 0f, 0.3f);

    [HideInInspector]
    public Transform volumeContainer;
}