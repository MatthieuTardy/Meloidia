using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public class Arete
{
    public Transform p1;
    public Transform p2;
}

[System.Serializable]
public class GroupeBordure
{
    public Transform parentDesPoints;
    public float hauteurMur = 10f;
    public float epaisseurMur = 1f;
    public string layerDesMurs = "Default";
    public PhysicMaterial physicsMaterial;
}

public class EasyBoundary : MonoBehaviour
{
    [Header("Paramètres d'Affichage")]
    public Color couleurGizmo = Color.cyan;
    public Color couleurLiaison = Color.green;

    [Header("Groupes de Points et Leurs Paramètres")]
    public List<GroupeBordure> groupesDePoints = new List<GroupeBordure>();

    [HideInInspector]
    public List<Arete> liaisons = new List<Arete>();

    public void NettoyerLiaisons()
    {
        liaisons.RemoveAll(l => l.p1 == null || l.p2 == null);
    }

    public bool LiaisonExiste(Transform a, Transform b)
    {
        foreach (Arete arete in liaisons)
        {
            if ((arete.p1 == a && arete.p2 == b) || (arete.p1 == b && arete.p2 == a)) return true;
        }
        return false;
    }

    public void AjouterLiaison(Transform a, Transform b)
    {
        if (a != b && !LiaisonExiste(a, b))
        {
            liaisons.Add(new Arete { p1 = a, p2 = b });
        }
    }

    public void SupprimerLiaison(Transform a, Transform b)
    {
        for (int i = liaisons.Count - 1; i >= 0; i--)
        {
            Arete arete = liaisons[i];
            if ((arete.p1 == a && arete.p2 == b) || (arete.p1 == b && arete.p2 == a))
            {
                liaisons.RemoveAt(i);
            }
        }
    }

    public GroupeBordure TrouverGroupeDuNoeud(Transform noeud)
    {
        foreach (GroupeBordure groupe in groupesDePoints)
        {
            if (groupe == null || groupe.parentDesPoints == null) continue;
            foreach (Transform enfant in groupe.parentDesPoints)
            {
                if (enfant == noeud) return groupe;
            }
        }
        return null;
    }

    private void OnDrawGizmos()
    {
        NettoyerLiaisons();
        Gizmos.color = couleurGizmo;

#if UNITY_EDITOR
        GUIStyle styleTexte = new GUIStyle();
        styleTexte.normal.textColor = Color.white;
        styleTexte.alignment = TextAnchor.MiddleCenter;
        styleTexte.fontSize = 12;
        styleTexte.fontStyle = FontStyle.Bold;
#endif

        foreach (GroupeBordure groupe in groupesDePoints)
        {
            if (groupe == null || groupe.parentDesPoints == null) continue;
            foreach (Transform enfant in groupe.parentDesPoints)
            {
                Gizmos.DrawSphere(enfant.position, 0.4f);

#if UNITY_EDITOR
                Handles.Label(enfant.position + Vector3.up * 1f, enfant.name, styleTexte);
#endif
            }
        }

        Gizmos.color = couleurLiaison;
        foreach (Arete arete in liaisons)
        {
            Gizmos.DrawLine(arete.p1.position, arete.p2.position);
        }
    }

    [ContextMenu("Générer les Colliders (Confirmer)")]
    public void GenererColliders()
    {
        NettoyerLiaisons();

        if (liaisons.Count == 0)
        {
            Debug.LogWarning("No links available to generate walls.");
            return;
        }

        GameObject dossierPrincipal = new GameObject("Colliders_Generes");
        dossierPrincipal.transform.SetParent(this.transform);

        Dictionary<GroupeBordure, Transform> sousDossiers = new Dictionary<GroupeBordure, Transform>();

        for (int i = 0; i < liaisons.Count; i++)
        {
            Transform p1 = liaisons[i].p1;
            Transform p2 = liaisons[i].p2;

            GroupeBordure groupe = TrouverGroupeDuNoeud(p1);
            if (groupe == null) continue;

            Transform parentActuel = dossierPrincipal.transform;
            if (!sousDossiers.ContainsKey(groupe))
            {
                GameObject sd = new GameObject($"Murs_{groupe.parentDesPoints.name}");
                sd.transform.SetParent(dossierPrincipal.transform);
                sousDossiers[groupe] = sd.transform;
            }
            parentActuel = sousDossiers[groupe];

            int layerID = LayerMask.NameToLayer(groupe.layerDesMurs);
            if (layerID == -1)
            {
                Debug.LogWarning($"Layer '{groupe.layerDesMurs}' not found for group '{groupe.parentDesPoints.name}'. Using Default layer.");
                layerID = 0;
            }

            Vector3 direction = p2.position - p1.position;
            float distance = direction.magnitude;
            Vector3 centre = p1.position + (direction / 2f);

            GameObject mur = new GameObject($"Mur_{p1.name}_a_{p2.name}");
            mur.transform.SetParent(parentActuel);
            mur.transform.position = centre;
            mur.layer = layerID;

            if (direction != Vector3.zero)
            {
                mur.transform.rotation = Quaternion.LookRotation(direction);
            }

            BoxCollider box = mur.AddComponent<BoxCollider>();
            box.size = new Vector3(groupe.epaisseurMur, groupe.hauteurMur, distance);

            if (groupe.physicsMaterial != null)
            {
                box.material = groupe.physicsMaterial;
            }
        }

        Debug.Log($"{liaisons.Count} invisible walls generated successfully!");
    }

    [ContextMenu("Auto-Link Sequential (Hierarchy Order)")]
    public void LierAutomatiquement()
    {
        foreach (GroupeBordure groupe in groupesDePoints)
        {
            if (groupe == null || groupe.parentDesPoints == null) continue;
            List<Transform> points = new List<Transform>();
            foreach (Transform enfant in groupe.parentDesPoints) points.Add(enfant);

            for (int i = 0; i < points.Count - 1; i++)
            {
                AjouterLiaison(points[i], points[i + 1]);
            }
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(EasyBoundary))]
public class EasyBoundaryEditor : Editor
{
    private Transform noeudSelectionne = null;

    private void OnSceneGUI()
    {
        EasyBoundary script = (EasyBoundary)target;
        script.NettoyerLiaisons();

        foreach (GroupeBordure groupe in script.groupesDePoints)
        {
            if (groupe == null || groupe.parentDesPoints == null) continue;

            foreach (Transform point in groupe.parentDesPoints)
            {
                float tailleNoeud = HandleUtility.GetHandleSize(point.position) * 0.2f;
                Handles.color = (point == noeudSelectionne) ? Color.yellow : Color.cyan;

                if (Handles.Button(point.position, Quaternion.identity, tailleNoeud, tailleNoeud, Handles.SphereHandleCap))
                {
                    if (noeudSelectionne == null)
                    {
                        noeudSelectionne = point;
                    }
                    else if (noeudSelectionne == point)
                    {
                        noeudSelectionne = null;
                    }
                    else
                    {
                        Undo.RecordObject(script, "Add Link");
                        script.AjouterLiaison(noeudSelectionne, point);
                        EditorUtility.SetDirty(script);
                        noeudSelectionne = null;
                    }
                }
            }
        }

        for (int i = script.liaisons.Count - 1; i >= 0; i--)
        {
            Arete arete = script.liaisons[i];
            if (arete.p1 == null || arete.p2 == null) continue;

            Vector3 centre = (arete.p1.position + arete.p2.position) / 2f;
            float tailleBouton = HandleUtility.GetHandleSize(centre) * 0.15f;

            Handles.color = Color.red;
            if (Handles.Button(centre, Quaternion.identity, tailleBouton, tailleBouton, Handles.CubeHandleCap))
            {
                Undo.RecordObject(script, "Remove Link");
                script.SupprimerLiaison(arete.p1, arete.p2);
                EditorUtility.SetDirty(script);
            }
        }
    }
}
#endif