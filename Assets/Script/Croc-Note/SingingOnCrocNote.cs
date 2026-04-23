using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingingOnCrocNote : MonoBehaviour
{
    public List<musicalNotes> chantDuFollow = new List<musicalNotes> { musicalNotes.Do, musicalNotes.Mi, musicalNotes.Sol };
    public List<musicalNotes> chantDuUnfollow = new List<musicalNotes> { musicalNotes.Sol, musicalNotes.Mi, musicalNotes.Do };
    LegumeManager Lmanager;
    bool CanFollow;
    private void Start()
    {
        Lmanager = GetComponent<LegumeManager>();
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == 0)
        {
            if(other.gameObject.tag == "Chant")
            {
                CheckNote();
            }
        }
    }

    void CheckNote()
    {

            //Debug.Log("Check note");
            if (GameManager.Instance.playerManager.noteSystem.PlayerSingCorrectPattern(chantDuFollow) && !CanFollow)
            {
                Lmanager.StartFollowingLocation(GameManager.Instance.playerManager.transform);
                CanFollow = true;
            }
            else if (GameManager.Instance.playerManager.noteSystem.PlayerSingCorrectPattern(chantDuUnfollow) && CanFollow)
            {
                CanFollow = false;
                Lmanager.StopFollowingLocation();
            }
        
    }
}
