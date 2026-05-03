using UnityEngine;

[ExecuteAlways]
public class GrassInteractor : MonoBehaviour
{
    void Update()
    {
        // Envoie la position exacte de cet objet globalement à tous les shaders
        // qui utilisent la variable "_InteractorPosition"
        Shader.SetGlobalVector("_InteractorPosition", transform.position);
    }
}