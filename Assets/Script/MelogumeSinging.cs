using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MelogumeSinging : MonoBehaviour
{

    public FMODUnity.StudioEventEmitter DO;
    public FMODUnity.StudioEventEmitter RE;
    public FMODUnity.StudioEventEmitter MI;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SongOfHealing());
    }

    // Update is called once per frame
    void Update()
    {

    }
    void StopChant()
    {
        DO.Stop();
        RE.Stop();
        MI.Stop();
    }

    IEnumerator SongOfHealing()
    {
        DO.Play();
        yield return new WaitForSeconds(1);
        StopChant();
        RE.Play();
        yield return new WaitForSeconds(1);
        StopChant();
        MI.Play();
        yield return new WaitForSeconds(1);
        StopChant();
        yield return new WaitForSeconds(3);
        StartCoroutine(SongOfHealing());
    }
}

