using Assets._Scripts.Events;
using Assets._Scripts.Game.SaveSystem;
using Assets._Scripts.Interaction_System.Objects;
using Assets._Scripts.Utils;
using Assets._Scripts.Weather;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.Rendering.HableCurve;

public class PigmentMonster : MonoBehaviour, ISavable
{
    [Header("Settings")]
    [SerializeField] private bool shouldBeWorkable;
    [SerializeField] private Color idealColor;
    [SerializeField] private GameObject pigmentPrefab;
    [SerializeField] private float painSensitivity;
    [SerializeField] private float neededWork;
    [SerializeField] private float patCoef;
    [SerializeField] private string monsterName;
    [SerializeField] private Animator animator;
    [SerializeField] private BasicItem monsterItem;

    private float workProgress;
    private bool isInTheFence;
    private Rigidbody rb;
    private bool isOnGround = true;
    [Header("Stats")]
    [SerializeField] private float fear;
    [SerializeField] private float happiness;
    [SerializeField] private float hunger;
    [SerializeField] private float health;
    [SerializeField] private float disruptance;
    [SerializeField] private float curiosity;
    [SerializeField] WeatherType preferredWeather;
    [Header("Particles")]
    [SerializeField] private ParticleSystem happy;
    [SerializeField] private ParticleSystem sad;
    [SerializeField] private ParticleSystem hungry;
    [SerializeField] private ParticleSystem lowHP;
    [SerializeField] private ParticleSystem fearful;
    private PigmentMonster neigbour;
    [Header("Config Values")]
    private float surprise = 0.0f;
    private float hungerCoef = 0.0015f;
    private float weatherCoef = 0.01f;
    private float disruptanceCoef = 0.02f;
    private float surpriseCoef = 0.3f;
    private float fearCoef = 0.01f;
    private float curiosityCoef = 0.35f;
    private float hungerGrowthPerTick = 0.005f;
    private float tolerance;
    private float saturationPerFood = 0.3f;
    private float fadeFactor = 1.5f;
    private float harvestCoef = 0.2f;
    private float healthRecoveryBase = 0.02f;
    private float exitFenceHealthBonus = 0.15f;
    private float exitFenceHappinessBonus = 0.2f;

    private float hungerToStatCoef = 0.04f;
    private float healthBonusCoef = 0.003f;
    private float neighbourHappinessCoef = 0.005f;

    private float feedHappinessBonus = 0.25f;
    private float lastWeatherCheckTime = 0f;
    private float weatherEmojiCooldown = 15f;
    private bool wasWeatherBad = false;

    private float minPigmentVolume = 1f;
    private float maxPigmentVolume = 5f;

    public Dictionary<string, float> GetMonsterStats() => new Dictionary<string, float> { { "happiness", happiness }, { "fear", fear }, { "hunger", hunger }, { "health", health }, { "disruptance", disruptance }, { "curiosity", curiosity } };

    public bool GetShouldBeWorkable() => shouldBeWorkable;

    public bool IsCurrentWeatherPreferred() => WeatherManager.Instance.GetCurrentWeather() == preferredWeather;

    public void SetInTheFence(bool isInTheFence)
    {
        if (this.isInTheFence && !isInTheFence)
        {
            health = Mathf.Clamp(health + exitFenceHealthBonus, 0f, 1f);
            disruptance = Mathf.Clamp(disruptance - exitFenceHappinessBonus, 0f, 1f);
        }
        this.isInTheFence = isInTheFence;
    }

    public Color GetIdealColor() => idealColor;

    public void SetNeighbour(PigmentMonster neighbour)
    {
        this.neigbour = neighbour;

        if (neighbour != null && neighbour.neigbour != this)
        {
            neighbour.SetNeighbour(this);
        }
    }

    private Coroutine ticker;

    #region Stat Calculations
    private void CalculateTolerance()
    {
        if (neigbour == null)
        {
            tolerance = 0;
            return;
        }
        tolerance = neigbour.GetIdealColor() == idealColor ? 1 : 0;
    }

    private void CalculateHunger(int ticks)
    {
        float newHunger = hunger + hungerGrowthPerTick;
        hunger = Mathf.Clamp(newHunger, 0f, 1f);
    }

    private void CalculateDisruptance(int ticks)
    {
        float weatherInfluence = IsCurrentWeatherPreferred() ? -weatherCoef : weatherCoef;

        float fearInducedDisruptance = fear * disruptanceCoef;

        float baseGrowth = (weatherInfluence + fearInducedDisruptance) * ticks;

        float surpriseEffect = surprise * surpriseCoef;

        float newDisruptance = disruptance + baseGrowth - surpriseEffect;

        disruptance = Mathf.Clamp(newDisruptance, 0f, 1f);
    }

    private void CalculateFear(int ticks)
    {
        float toleranceEffect = 0f;
        float antiFearCoef = 0;
        if (neigbour != null)
        {
            antiFearCoef = 0f;
            toleranceEffect = ((1f - tolerance) * fearCoef * ticks) - (tolerance * fearCoef * ticks);
        }
        else 
        {
            antiFearCoef = 0.05f;
        }

            float surpriseEffect = surprise * surpriseCoef;

        float newFear = (fear + toleranceEffect) - surpriseEffect - antiFearCoef;
        fear = Mathf.Clamp(newFear, 0f, 1f);
    }

    private void CalculateHappiness(int ticks)
    {
        float penalties = (disruptance * disruptanceCoef * ticks);

        float hungerPenalty = hunger * hungerToStatCoef * ticks;
        float bonuses = (surprise * surpriseCoef) + (curiosity * curiosityCoef * ticks) + 0.001f;

        float newHappiness = happiness - penalties - hungerPenalty + bonuses;
        happiness = Mathf.Clamp(newHappiness, 0f, 1f);

        surprise = 0.0f;
    }

    private void GroundCheck()
    {
        RaycastHit hit;
        isOnGround = Physics.Raycast(transform.position, Vector3.down, out hit) && hit.distance < 0.2f;
        animator.SetBool("Falling", !isOnGround && !monsterItem.GetIsGrabbed());
    }

    private void CalculateCuriosity(int ticks)
    {
        if (neigbour == null)
        {
            float curiosityDecay = curiosity * 0.005f * ticks;
            curiosity = Mathf.Clamp(curiosity - curiosityDecay, 0f, 1f);
        }
        else
        {
            float newCuriosity = curiosity + (tolerance * curiosityCoef * ticks);
            curiosity = Mathf.Clamp(newCuriosity, 0f, 1f);
        }
    }

    private void CalculateHealthRecovery(int ticks)
    {
        float hungerPenalty = hunger * hungerToStatCoef * ticks;
        float fearPenalty = (fear * fearCoef * ticks);
        float newHealth = health + healthRecoveryBase - fearPenalty - hungerPenalty; 
        health = Mathf.Clamp(newHealth, 0f, 1f);
    }

    private void CheckWeatherAndShowEmoji()
    {
        bool isWeatherBad = !IsCurrentWeatherPreferred();

        if (isWeatherBad)
        {
            if (sad != null)
            {
                sad.Play();
            }
            lastWeatherCheckTime = Time.time;
        }

        if (Time.time - lastWeatherCheckTime < weatherEmojiCooldown)
        {
            wasWeatherBad = isWeatherBad;
            return;
        }

        if (isWeatherBad && sad != null)
        {
            sad.Play();
            lastWeatherCheckTime = Time.time;
        }

        wasWeatherBad = isWeatherBad;
    }
    #endregion

    public void Start()
    {
        SubscribeToSaveEvent();
        rb = GetComponent<Rigidbody>();
        ticker = StartCoroutine(MonsterTicker());
    }

    private void TickStats(int ticks)
    {
        CalculateTolerance();
        CalculateHunger(ticks);

        CalculateFear(ticks);

        CalculateDisruptance(ticks);

        CalculateCuriosity(ticks);

        CalculateHappiness(ticks);
        CalculateHealthRecovery(ticks);
    }

    private void SpawnPigment()
    {
        float r = Mathf.Clamp(Mathf.Clamp(idealColor.r + ((10 * (1 - happiness)) / 255f), 0, 1f) - (fadeFactor * hunger), 0.1f, 1f);
        float g = Mathf.Clamp(Mathf.Clamp(idealColor.g + ((10 * (1 - happiness)) / 255f), 0, 1f) - (fadeFactor * hunger), 0.1f, 1f);
        float b = Mathf.Clamp(Mathf.Clamp(idealColor.b + ((10 * (1 - happiness)) / 255f), 0, 1f) - (fadeFactor * hunger), 0.1f, 1f);

        float volume = minPigmentVolume + ((maxPigmentVolume - minPigmentVolume) * health * 0.9f);

        health = Mathf.Clamp(health - harvestCoef, 0f, 1f);
        Debug.Log("<color=blue>Лутай пигмент");

        bool emojiShown = false;

        if (health < 0.3f && lowHP != null)
        {
            lowHP.Play();
            emojiShown = true;
        }
        else if (hunger > 0.7f && hungry != null)
        {
            hungry.Play();
            emojiShown = true;
        }
        else if (fear > 0.6f && fearful != null)
        {
            fearful.Play();
            emojiShown = true;
        }
        else if (happiness < 0.3f && sad != null)
        {
            sad.Play();
            emojiShown = true;
        }
        else if (!IsCurrentWeatherPreferred() && (happiness < 0.5f || health < 0.5f || hunger > 0.5f) && sad != null)
        {
            sad.Play();
            emojiShown = true;
        }

        if (!emojiShown && happy != null)
        {
            happy.Play();
        }
        try
        {

            GameManager.Instance.GetTutorial().ProgressTutorial(5);
            animator.SetTrigger("Worked");
        }
        catch
        {

        }
        GameObject pigment = Instantiate(pigmentPrefab, transform.position, Quaternion.identity);

        pigment.GetComponent<ColorPigment>().InitializePigment(new Color(r, g, b), volume);
    }

    protected virtual void MaintainBalance()
    {

    }

    private void CalculateWork(float force)
    {
        Debug.Log($"<color=green>Pat with force {force}");
        if (force >= painSensitivity)
        {
            health -= patCoef;
            health = Mathf.Clamp(health, 0, 1f);
            Debug.Log("<color=yellow>Больна");
        }
        else
        {
            Debug.Log("<color=green>Кайфарик");
            workProgress += force;
            surprise = 0.3f;
        }
        if (workProgress >= neededWork)
        {
            workProgress = 0;
            SpawnPigment();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!isInTheFence) return;
        if (collision.collider.tag == "Sponge")
        {
            CalculateWork(collision.impulse.magnitude);
        }
        if (collision.collider.tag == "Carrot")
        {
            Destroy(collision.gameObject);
            Eat();
        }
    }

    private void Update()
    {
        GroundCheck();
    }


    private void Eat()
    {
        Debug.Log("<color=green>Я это хаваю!");
        hunger = Mathf.Clamp(hunger - saturationPerFood, 0.0f, 1.0f);
        surprise = 1.0f;
        happiness = Mathf.Clamp(happiness + feedHappinessBonus, 0.0f, 1.0f);
        if (happy != null)
        {
            happy.Play();
        }
    }

    private IEnumerator MonsterTicker()
    {
        while (true)
        {
            yield return new WaitForSeconds(2);
            int tick = PocketTicker.Instance.GetTicksForCalculations();
            TickStats(tick);
        }
    }

    public void SubscribeToSaveEvent()
    {
        SaveEvents.OnSaveEvent.AddListener(SaveData);
    }

    public void OnDestroy()
    {
        SaveEvents.OnSaveEvent.RemoveListener(SaveData);
    }

    public void SaveData()
    {
        SavablePrefab monsterSave = new SavablePrefab
        {
            prefabName = monsterName,
            dimension = 2,
            worldPosition = Mapper.VectorToFloatData(transform.position), //new List<float> { transform.position.x, transform.position.y, transform.position.z },
            quaternionRotation = Mapper.QuaternionToFloatData(transform.rotation), //new List<float> { transform.rotation.x, transform.rotation.y, transform.rotation.z, transform.rotation.w },
            floatData = new Dictionary<string, float>
      {
        {"fear",fear },
        {"happiness",happiness },
        {"hunger",hunger },
        {"health",health },
        {"disruptance",disruptance },
        {"curiosity",curiosity }
      }
        };

        SaveManager.Instance.SavePrefab(monsterSave);
    }

    public void SyncData(SavablePrefab data)
    {
        fear = data.floatData["fear"];
        happiness = data.floatData["happiness"];
        hunger = data.floatData["hunger"];
        health = data.floatData["health"];
        disruptance = data.floatData["disruptance"];
        curiosity = data.floatData["curiosity"];
    }
}