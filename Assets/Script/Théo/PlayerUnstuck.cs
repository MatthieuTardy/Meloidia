using UnityEngine;

public class PlayerUnstuck : MonoBehaviour
{
    public Transform pointDeSecoursParDefaut;
    private CharacterController characterController;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    public void DeBloquerJoueur()
    {
        if (pointDeSecoursParDefaut == null) return;

        if (characterController != null)
        {
            characterController.enabled = false;
        }

        transform.position = pointDeSecoursParDefaut.position;
        transform.rotation = pointDeSecoursParDefaut.rotation;

        if (characterController != null)
        {
            characterController.enabled = true;
        }
    }
}