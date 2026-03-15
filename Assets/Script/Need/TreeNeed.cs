using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/*
public class TreeNeed : MonoBehaviour
{
    [SerializeField] NeedCondition[] Needs;
    int currentIndex;

    public void CheckNeed()
    {
        if (Needs[currentIndex] != null)
        {
            if (Needs[currentIndex].WantCN)
            {
                int CN1 = 0;
                int CN2 = 0;
                int CN3 = 0;
                int CN4 = 0;
                int CN5 = 0;

                foreach (var i in Needs[currentIndex].typeOfCN)
                {
                    if (i == LegumeManager.type.un)
                    {
                        CN1++;
                    }
                    else if (i == LegumeManager.type.deux)
                    {
                        CN2++;
                    }
                    else if (i == LegumeManager.type.trois)
                    {
                        CN3++;
                    }
                    else if (i == LegumeManager.type.quatre)
                    {
                        CN4++;
                    }
                    else if (i == LegumeManager.type.cinq)
                    {
                        CN5++;
                    }
                }

                int CNT1 = 0;
                int CNT2 = 0;
                int CNT3 = 0;
                int CNT4 = 0;
                int CNT5 = 0;
                foreach (var i in GameManager.Instance.legumeManagerList)
                {
                    if (i.legumeType == LegumeManager.type.un)
                    {
                        CNT1++;
                    }
                    else if (i.legumeType == LegumeManager.type.deux)
                    {
                        CNT2++;
                    }
                    else if (i.legumeType == LegumeManager.type.trois)
                    {
                        CNT3++;
                    }
                    else if (i.legumeType == LegumeManager.type.quatre)
                    {
                        CNT4++;
                    }
                    else if (i.legumeType == LegumeManager.type.cinq)
                    {
                        CNT5++;
                    }
                }

                bool pass = false;
                if (CN1 != 0 && CNT1 >= CN1) 
                {
                    pass = true;
                }
                if(CN2 != 0 && CNT2 >= CN2)
                {
                        pass = true;
                }
            }
        }
    }
}


[Serializable]
class NeedCondition
{
    [InfoBox("ajouter autant de CrocNote et de ressources que nécessaire")]
    public bool WantCN;
    public bool WantRessources;

    public UnityEvent OnAllNeed;

    public LegumeManager.type[] typeOfCN;
    public TypeOfRessources[] typeOfRessources;
}
*/

public class Need : MonoBehaviour
{
    [SerializeField] PlayerProximity proxy;
    [SerializeField] Condition condition;

    [SerializeField] UnityEvent Event;
    
    private void Start()
    {
        StartCoroutine(WaitToCheck());
    }

    IEnumerator WaitToCheck()
    {
        yield return new WaitUntil(() => proxy.Proximity == true);

        //Debug.Log("wait pass " + this.gameObject.name);
        if (condition.CheckCondition())
        {
            Event.Invoke();
        }
        else
        {
            StartCoroutine(WaitToCheck());
        }
    }


}