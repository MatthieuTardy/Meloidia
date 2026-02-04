using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ReloadGame : MonoBehaviour
{
    [SerializeField] KeyCode ReloadKey = KeyCode.F10;
    
    void Update()
    {
        if(Application.isPlaying && Input.GetKeyDown(ReloadKey))
        {
            string currentSceneName = SceneManager.GetActiveScene().name;
           // FMODUnity.RuntimeManager.MuteAllEvents(true);
            SceneManager.LoadScene(currentSceneName);
        }
    }
}
