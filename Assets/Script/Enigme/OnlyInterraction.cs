using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OnlyInterraction : Interractable
{
    [SerializeField] UnityEvent OnInterract;
    [SerializeField] Condition condition;
    [SerializeField] bool RepeteEvent;
    bool activate = false;

    // === AJOUT POUR ANIMATION ===
    private Animator playerAnimator;
    private PlayerController playerController;

    private void Start()
    {
        playerController = FindObjectOfType<PlayerController>();
        if (playerController != null)
        {
            playerAnimator = playerController.animator;
        }
    }

    public override void Interract()
    {
        // === AJOUT : Active INTERACT sur l'animator du player ===
        if (playerAnimator != null)
        {
            playerAnimator.SetBool("interact", true);
            StartCoroutine(ResetInteractBool());
        }

        if (condition == null)
        {
            if (RepeteEvent)
            {
                OnInterract.Invoke();
            }
            else if (!activate)
            {
                OnInterract.Invoke();
                activate = true;
            }
        }
        else
        {
            if (condition.CheckCondition())
            {
                if (RepeteEvent)
                {
                    OnInterract.Invoke();
                }
                else if (!activate)
                {
                    OnInterract.Invoke();
                    activate = true;
                }
            }
        }
    }

    // === AJOUT : Coroutine pour remettre interact à false ===
    private IEnumerator ResetInteractBool()
    {
        yield return new WaitForSeconds(0.2f);
        if (playerAnimator != null)
        {
            playerAnimator.SetBool("interact", false);
        }
    }

    public void DebugFunction()
    {
        Debug.Log(" Interraction ");
    }

    public void DestroyTarget(GameObject target)
    {
        if (target != null)
        {
            Destroy(target);
        }
    }
}