using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CallEventOnConditionMet : MonoBehaviour
{
    [SerializeField] Condition condition;
    [SerializeField] UnityEvent events;

    private void Start()
    {
        StartCoroutine(WaitForCondition());
    }

    IEnumerator WaitForCondition()
    {
        yield return new WaitUntil(() => condition.CheckCondition() == true);
        events.Invoke();
    }

}
