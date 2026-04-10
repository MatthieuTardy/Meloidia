using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generator : MonoBehaviour
{
    [SerializeField] CrocNoteType type;

    [SerializeField] float MaxTime;
    [SerializeField] float MinTime;

    private void Start()
    {
        StartCoroutine(LauchRoutine());
    }

    IEnumerator LauchRoutine()
    {
        
        switch (type)
        {
            case CrocNoteType.un:
                yield return new WaitUntil(() => RanchManager.instance.RanchGenerator.CNbyType[0].Count > 0);
                StartCoroutine(CreateCrocNote(RanchManager.instance.RanchGenerator.CNbyType[0].Count));
                    break;
            case CrocNoteType.deux:
                yield return new WaitUntil(() => RanchManager.instance.RanchGenerator.CNbyType[1].Count > 0);
                StartCoroutine(CreateCrocNote(RanchManager.instance.RanchGenerator.CNbyType[1].Count));
                
                break;
            case CrocNoteType.trois:
                yield return new WaitUntil(() => RanchManager.instance.RanchGenerator.CNbyType[2].Count > 0);
                StartCoroutine(CreateCrocNote(RanchManager.instance.RanchGenerator.CNbyType[2].Count));
                
                break;
            case CrocNoteType.quatre:
                yield return new WaitUntil(() => RanchManager.instance.RanchGenerator.CNbyType[3].Count > 0);
                StartCoroutine(CreateCrocNote(RanchManager.instance.RanchGenerator.CNbyType[3].Count));
                
                break;
            case CrocNoteType.cinq:
                yield return new WaitUntil(() => RanchManager.instance.RanchGenerator.CNbyType[4].Count > 0);
                StartCoroutine(CreateCrocNote(RanchManager.instance.RanchGenerator.CNbyType[4].Count));
                
                break;
        }
    }
    GameObject PrefabRessouces;
    Transform SpawnPoint;
    IEnumerator CreateCrocNote(float CNNb)
    {
        yield return null;
        // on genere la ressouces apres x temps
        // pour chaque croc note 

        yield return new WaitForSeconds(Mathf.Lerp(MinTime, MaxTime, CNNb));
        Instantiate(PrefabRessouces, SpawnPoint.position, SpawnPoint.rotation);
        StartCoroutine(LauchRoutine());
    }
}
