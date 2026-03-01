using FMODUnity;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.UI.Image;

public class Plantation_interaction : MonoBehaviour
{
    /*
    [Header("Init")]
    private bool detected = false;
    public Seed graine;
    public GameObject legume;
    public GameObject plante;
    public GameObject maison;

    [Header("Particle Systems")]
    public ParticleSystem plantingParticles;
    public ParticleSystem preparingParticles;
    public ParticleSystem cleaningParticles;
    public ParticleSystem wateringParticles;
    public float particleSystemExtraLifetime = 1.0f;
    public float particleYOffset = 1.0f;
    public Vector3 particleRotation = new Vector3(90, 0, 0); // NOUVEAU : Rotation des particules

    [Header("State")]
    private bool watered = false;
    private bool prepared = false;
    private bool clean = false;
    private bool planted = false;
    private bool finished = false;
    private bool isHarvesting = false;
    private bool isPlanting = false;
    public int happyness = 0;

    private float timer;

    [Header("Effet de Gigotement")]
    public float jiggleAmount = 5f;
    public float jiggleSpeed = 10f;

    [Header("Effet de Récolte")]
    public float harvestSpinSpeed = 360f;
    public int essenceGainOnHarvest = 25;

    private Quaternion originalRotation;

    private void Start()
    {
        // Initialisation si nécessaire
    }

    void Update()
    {
        if (isHarvesting || isPlanting) return;

        if (planted)
        {
            timer += Time.deltaTime;
            if (timer / graine.GrowthTimeTotal >= 1)
            {
                if (!finished)
                {
                    plante.transform.localPosition = new Vector3(0, 0.30f, 0);
                    plante.transform.localScale = new Vector3(0.18f, 0.18f, 0.18f);
                    finished = true;
                    originalRotation = plante.transform.localRotation;
                }
            }
            else if (timer / graine.GrowthTimeTotal >= 0.5)
            {
                plante.transform.localPosition = new Vector3(0, 0.25f, 0);
                plante.transform.localScale = new Vector3(0.15f, 0.15f, 0.15f);
            }
        }

        if (finished)
        {
            float angle = jiggleAmount * Mathf.Sin(Time.time * jiggleSpeed);
            plante.transform.localRotation = originalRotation * Quaternion.Euler(0, 0, angle);
        }

        if (detected && Input.GetButtonDown("Fire1") && GameManager.Instance.playerManager.outils == 0 && prepared == true && clean == true)
        {
            PlayerManager playerManager = GameManager.Instance.playerManager;
            if (playerManager.indexTuto == 2 && (playerManager.indexTuto + 1) < playerManager.tuto.Count)
            {
                playerManager.indexTuto++;
                playerManager.tutoSelect = playerManager.tuto[playerManager.indexTuto];
            }

            HandleAction();
        }
        else if (detected && Input.GetButtonDown("Fire1") && GameManager.Instance.playerManager.outils == 1 && watered == false && planted == true && GameManager.Instance.playerManager.eau > 0)
        {
            GameManager.Instance.playerManager.eau -= 1;
            watered = true;
            timer += 10;
            happyness += 10;
            PlayParticles(wateringParticles, particleSystemExtraLifetime);
            Debug.Log("Arrosé");
        }
        else if (detected && Input.GetButtonDown("Fire1") && GameManager.Instance.playerManager.outils == 2 && prepared == false && GameManager.Instance.playerManager.terre > 0)
        {
            GameManager.Instance.playerManager.terre -= 1;
            prepared = true;
            PlayParticles(preparingParticles, particleSystemExtraLifetime);
            Debug.Log("Terre est mise");
        }
        else if (detected && Input.GetButtonDown("Fire1") && GameManager.Instance.playerManager.outils == 3 && clean == false && prepared == true)
        {
            clean = true;
            PlayParticles(cleaningParticles, particleSystemExtraLifetime);
            Debug.Log("Terrain propre");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Detecteur")) { detected = true; }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Detecteur")) { detected = false; }
    }

    private void HandleAction()
    {
        if (isHarvesting || isPlanting) return;

        if (finished)
        {
            StartCoroutine(HarvestAndShrink());
        }
        else if (!planted)
        {
            StartCoroutine(PlantAndGrow());
        }
    }

    private IEnumerator PlantAndGrow()
    {
        isPlanting = true;

        PlayParticles(plantingParticles, particleSystemExtraLifetime);

        float duration = 0.3f;
        Vector3 initialScale = Vector3.zero;
        Vector3 targetScale = new Vector3(0.1f, 0.1f, 0.1f);
        float elapsedTime = 0f;

        plante.transform.localPosition = new Vector3(0, 0.20f, 0);
        plante.transform.localScale = initialScale;
        plante.SetActive(true);
        originalRotation = plante.transform.localRotation;

        while (elapsedTime < duration)
        {
            plante.transform.localScale = Vector3.Lerp(initialScale, targetScale, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        plante.transform.localScale = targetScale;
        planted = true;
        finished = false;
        timer = 0;
        isPlanting = false;
    }

    private IEnumerator HarvestAndShrink()
    {
        isHarvesting = true;
        GameManager.Instance.playerManager.essenceMagique += essenceGainOnHarvest;

        float duration = 0.5f;
        Vector3 initialScale = plante.transform.localScale;
        float elapsedTime = 0f;

        plante.transform.localRotation = originalRotation;

        while (elapsedTime < duration)
        {
            float progress = elapsedTime / duration;
            plante.transform.localScale = Vector3.Lerp(initialScale, Vector3.zero, progress);
            plante.transform.Rotate(Vector3.up, harvestSpinSpeed * Time.deltaTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        Instantiate(legume, new Vector3(transform.position.x, transform.position.y + 2, transform.position.z - 3), new Quaternion(0, 180, 0, 0));
        maison.SetActive(true);
        if (GameManager.Instance.playerManager.indexTuto == 4)
        {
            GameManager.Instance.playerManager.indexTuto++;
            GameManager.Instance.playerManager.tutoSelect = GameManager.Instance.playerManager.tuto[GameManager.Instance.playerManager.indexTuto];
        }
        Debug.Log("Récolté ! Essence gagnée : " + essenceGainOnHarvest);
        clean = false;
        prepared = false;
        watered = false;
        plante.SetActive(false);
        plante.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        plante.transform.localRotation = originalRotation;

        finished = false;
        planted = false;
        timer = 0;
        isHarvesting = false;
    }

    /// <summary>
    /// Instancie, joue et détruit un système de particules après sa durée.
    /// </summary>
    /// <param name="particleSystemPrefab">Le préfabriqué du système de particules à jouer.</param>
    /// <param name="extraLifetime">Temps supplémentaire avant la destruction de l'objet.</param>
    private void PlayParticles(ParticleSystem particleSystemPrefab, float extraLifetime)
    {
        if (particleSystemPrefab == null) return;

        Vector3 spawnPosition = transform.position + Vector3.up * particleYOffset;
        Quaternion spawnRotation = Quaternion.Euler(particleRotation); // MODIFIÉ : Utilise la variable de rotation

        ParticleSystem psInstance = Instantiate(particleSystemPrefab, spawnPosition, spawnRotation);

        float totalDuration = psInstance.main.duration + psInstance.main.startLifetime.constantMax + extraLifetime;

        Destroy(psInstance.gameObject, totalDuration);
    }
    */

    bool isWatered;
    bool isDirty;
    bool isSeeded;
    bool isSunged = true;

    bool canSpawn = false;
    bool isGrowing = false;

    [InfoBox("0 null, 1 dirt, 2 water, 3 sing,4 seed")]
    [SerializeField] Sprite[] sprites;
    [SerializeField] SpriteRenderer spriteRenderer;
    
    [SerializeField] TypeOfRessources SeedType;
    [SerializeField] GameObject CrocNotePrefab;
    [SerializeField] Transform SpawnPoint;
    [SerializeField] Transform SeedSpawnPoint;

    Coroutine GrowRoutine;
    [SerializeField] float GrowTime;
    float CurrentTime;

    [SerializeField] int NumberOfPhases;
    int currentPhases = 0;
    [SerializeField][Range(0, 100)] int ChanceOfNeedPerPhases;

    [SerializeField] Material WaterMaterial;
    [SerializeField] float WaterPercent;

    [SerializeField] List<musicalNotes> SingPatern;
    [SerializeField] List<musicalNotes> WaterPatern;
    [SerializeField] Parcelle parcelle;

    [SerializeField] GameObject seedPrefab;
    GameObject currentSeed;


    

    [Header("Particle Systems")]
    public ParticleSystem plantingParticles;
    public ParticleSystem preparingParticles;
    public ParticleSystem cleaningParticles;
    public ParticleSystem wateringParticles;
    public float particleSystemExtraLifetime = 1.0f;
    public float particleYOffset = 1.0f;
    public Vector3 particleRotation = new Vector3(90, 0, 0); // NOUVEAU : Rotation des particules
    public void Interract(int outil)
    {
        Debug.Log("Outils in hand : " + outil);

        /* switch (outil)
        {
            case 0: // main
                break;
            case 1: // arrosoir
                AddWater();
                break;
            case 2: // pelle
                break;
            default: // 
                Debug.LogError("Unknow tool");
                break;

        }
       */
        Debug.Log("IsDirty " + isDirty);
        Debug.Log("IsSeeded " + isSeeded);
        if (!isDirty)
        {
            AddDirt();
        }
        if (!isSeeded)
        {
            AddSeed();
            StartGrow();
            StartCoroutine(NeedWater());
        }
        UpdateSprite();
        

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Chant")
        {

            waitRoutine = StartCoroutine(WaterChant());
            Debug.LogWarning("start chant");



        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Chant")
        {
            if (waitRoutine != null)
            {
                StopCoroutine(waitRoutine);
                Debug.LogWarning("stop chant");
            }


        }
    }

    IEnumerator NeedWater()
    {
        if(WaterPercent < 25)
        {
            isGrowing = false;
            isWatered = false;
            UpdateSprite();
        }
        else
        {
            isGrowing = true;
            isWatered = true;
            UpdateSprite();
        }
        yield return new WaitForSeconds(.6f);
        if(WaterPercent > 0)
        {
            WaterPercent--;
            
        }
        Debug.Log("Amount " + WaterPercent / 100);
        WaterMaterial.SetFloat("Amount", (WaterPercent/100));
        StartCoroutine(NeedWater());   
    }

    IEnumerator WaterChant()
    {
        yield return new WaitUntil(() => GameManager.Instance.playerManager.noteSystem.PlayerSingCorrectPattern(WaterPatern));
        yield return new WaitUntil(() => GameManager.Instance.playerManager.noteSystem.PlayerHoldLastNote(WaterPatern.Last()));

        while (GameManager.Instance.playerManager.noteSystem.PlayerHoldLastNote(WaterPatern.Last()))
        {
            yield return new WaitForSeconds(0.1f);
            if(WaterPercent > 100)
            {
                WaterPercent = 100;
                yield break;
            }
            else
            {
                Debug.Log("WaterPercent = " + WaterPercent);
                WaterPercent += 2;
            }
        }
        waitRoutine = StartCoroutine(WaterChant());
        
    }

    Coroutine waitRoutine;

    #region adding ressources => 1st Dirt - 2nd Seed - 3rd Water
    void AddDirt()
    {
        if (!isDirty)
        {
            GameManager.Instance.playerManager.UseDirt();
            Debug.Log("Add Dirt " + isDirty);
            isDirty = true;
            PlayParticles(preparingParticles, particleSystemExtraLifetime);

        }
    }
    void AddSeed()
    {
        if (isDirty)
        {
            if (!isSeeded)
            {
                // parcourir l'inventaire voir si il y a une graine
                bool haveSeed = false;
                Debug.Log("GameManager.Instance = " + GameManager.Instance);
                Debug.Log("inventory manager = " + GameManager.Instance.inventoryManager);
                Debug.Log("Items = " + GameManager.Instance.inventoryManager.Items);
                foreach (var item in GameManager.Instance.inventoryManager.Items)
                {
                    if (item != null)
                    {
                        if (item.CurrentItem.type == SeedType)
                        {
                            haveSeed = true;
                            break;
                        }
                    }
                }

                if (haveSeed)
                {
                    isSeeded = true;
                    SpawnSeed();
                    PlayParticles(plantingParticles, particleSystemExtraLifetime);
                    GameManager.Instance.inventoryManager.UseItem(SeedType, 1);
                }
            }
        }
    }
    void AddWater()
    {
        if (isDirty && isSeeded)
        {
            if (!isWatered && GameManager.Instance.playerManager.GetWater() > 0)
            {
                GameManager.Instance.playerManager.UseWater();
                Debug.Log("Add Water " + isWatered);
                isWatered = true;
                if (!isGrowing)
                {
                    StartGrow();
                    PlayParticles(wateringParticles, particleSystemExtraLifetime);

                }
            }
        }
    }
    IEnumerator singRoutine()
    {
        yield return new WaitUntil(() => GameManager.Instance.playerManager.noteSystem.PlayerSingCorrectPattern(SingPatern));
        isSunged = true;
        PlayParticles(plantingParticles, particleSystemExtraLifetime);
        if (canSpawn)
        {
            SpawnCrocNote();
        }
    }
    [Button("Sing")]
    void SingDebug()
    {
        isSunged = true;
        PlayParticles(plantingParticles, particleSystemExtraLifetime);
        if (canSpawn)
        {
            SpawnCrocNote();
        }
    }
    #endregion
    #region Grow
    void StartGrow()
    {
        CurrentTime = 0;
        GrowRoutine=StartCoroutine(GrowPlant());
    }

    IEnumerator StopGrowing()
    {
        yield return new WaitUntil(() => !isGrowing);
        StopCoroutine(GrowRoutine);
        yield return new WaitUntil(() => isGrowing);
        GrowRoutine = StartCoroutine(GrowPlant());
    }

    IEnumerator GrowPlant()
    {
        yield return new WaitUntil(() => isGrowing);
        StartCoroutine(StopGrowing());
        float time = GrowTime/NumberOfPhases;
        yield return new WaitForSecondsRealtime(time);
        EndOfPhases(time);
    }

    IEnumerator WaitUntilNeedIsComplete()
    {
        yield return new WaitUntil(() => isDirty);
        yield return new WaitUntil(() => isWatered);
        yield return new WaitUntil(() => isSunged);
        UpdateSprite();
        GrowSeedVisual();
        GrowRoutine = StartCoroutine(GrowPlant());
    }
    void EndOfPhases(float time)
    {
        CurrentTime += time;
        if (CurrentTime < GrowTime)
        {
            Debug.Log("Choose new need");
            ChooseNewNeed();
        }
        else
        {
            Debug.Log("finish growing");
            canSpawn = true;
            NeedToSing();
            UpdateSprite();
        }
    }
    void SpawnCrocNote()
    {
        GameObject CN = Instantiate(CrocNotePrefab, SpawnPoint.position, Quaternion.identity);
        UpdateSprite();
        PlayParticles(plantingParticles, particleSystemExtraLifetime);
        parcelle.AddCrocNoteToParcelle(CN);
        ResetParcelleData();
    }
    #endregion
    #region Seed Need
    void ChooseNewNeed()
    {
        int prob = Random.Range(0, 101);

        if (prob <= ChanceOfNeedPerPhases)
        {
            int rand = Random.Range(0, 3);
            switch (rand)
            {
                case 0:
                    NeedToDirt();
                    break;
                case 1:
                    //NeedToWater();
                    break;
                case 2:
                    NeedToSing();
                    break;
                default:
                    break;
            }
            UpdateSprite();
        }
        
        StartCoroutine(WaitUntilNeedIsComplete());
    }

    void NeedToSing()
    {
        isSunged = false;
        StartCoroutine(singRoutine());
    }

    void NeedToDirt()
    {
        isDirty = false;
    }

    void NeedToWater()
    {
        isWatered = false;
    }

    #endregion
    #region FeedBack
    void UpdateSprite()
    {
        if (!isDirty)
        {
            spriteRenderer.sprite = sprites[1];
        }
        else if (!isSeeded)
        {
            spriteRenderer.sprite = sprites[4];
        }
        else if (!isWatered)
        {
            spriteRenderer.sprite = sprites[2];
        }
        else if (!isSunged)
        {
            spriteRenderer.sprite = sprites[3];
        }
        else
        {
            spriteRenderer.sprite = sprites[0];

        }
    }

    private void PlayParticles(ParticleSystem particleSystemPrefab, float extraLifetime)
    {
        if (particleSystemPrefab == null) return;

        Vector3 spawnPosition = transform.position + Vector3.up * particleYOffset;
        Quaternion spawnRotation = Quaternion.Euler(particleRotation);

        ParticleSystem psInstance = Instantiate(particleSystemPrefab, spawnPosition, spawnRotation);

        float totalDuration = psInstance.main.duration + psInstance.main.startLifetime.constantMax + extraLifetime;

        Destroy(psInstance.gameObject, totalDuration);
    }
    
    #region seed visual
    void SpawnSeed()
    {
        currentSeed = Instantiate(seedPrefab, SeedSpawnPoint);
    }



    void GrowSeedVisual()
    {
        currentSeed.transform.localScale += new Vector3(0.1f,0.1f,0.1f);
    }



    #endregion

    void ResetParcelleData()
    {
        Destroy(currentSeed);
        isDirty = false;
        isSeeded = false;
        isWatered = false;
        WaterPercent = 0;
        isSunged = true;
        currentPhases = 0;
        CurrentTime = 0;
        spriteRenderer.sprite = sprites[0];
        isGrowing = false;
        canSpawn = false;
    }

    #endregion
}