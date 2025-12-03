using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildUI : MonoBehaviour
{
    public Transform wheelCenter; // Objet au centre de la roue
    public int segmentCount = 8;
    void Update()
    {
       // SelectConstructMouse();
        SelectConstructXbox();

        // ICI : appeler ta logique de construction en fonction de l’index
        // Build(index);

    }

    private void SelectConstructMouse()
    {
        Debug.Log("BuildUI");
        Vector2 mousePos = Input.mousePosition;
        Vector2 centerScreenPos = Camera.main.WorldToScreenPoint(wheelCenter.position);

        // Direction souris -> centre
        Vector2 dir = mousePos - centerScreenPos;

        // Calcul de l'angle en degrés (0° au dessus + rotation horaire)
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        angle = (angle + 360f + 90f) % 360f; // +90 pour mettre 0° vers le haut

        // Trouver le segment
        float segmentAngle = 360f / segmentCount;
        int index = Mathf.FloorToInt(angle / segmentAngle);

        // Affichage pour debug
        Debug.Log("Segment sélectionné : " + index);
    }
    private void SelectConstructXbox()
    {
        
        float inputX = Input.GetAxis("SongX_Xbox");
        float inputY = Input.GetAxis("SongY_Xbox");

        if (Mathf.Abs(inputX) > 0.5f || Mathf.Abs(inputY) > 0.5f)
        {
            int buildIndex = 0;

            if (inputY > 0.5f && Mathf.Abs(inputX) < 0.5f && Input.GetButtonDown("Fire1")) { buildIndex = 4; }
            else if (inputX > 0.3f && inputY > 0.3f) { buildIndex = 3; }
            else if (inputX > 0.5f && Mathf.Abs(inputY) < 0.5f) { buildIndex = 2; }
            else if (inputX > 0.3f && inputY < -0.3f) { buildIndex = 1; }
            else if (inputY < -0.5f && Mathf.Abs(inputX) < 0.5f) { buildIndex = 0; }
            else if (inputX < -0.3f && inputY < -0.3f) { buildIndex = 7; }
            else if (inputX < -0.5f && Mathf.Abs(inputY) < 0.5f) { buildIndex = 6; }
            else if (inputX < -0.3f && inputY > 0.3f) { buildIndex = 5; }

            SelectConstruct(buildIndex);
        }
    }


    public void SelectConstruct(int index)
    {
        GameManager.Instance.buildManager.ChangeSelectedBuild(index);
        Destroy(this.gameObject);
    }
} 
