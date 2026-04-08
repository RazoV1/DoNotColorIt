using Assets._Scripts.Events;
using Assets._Scripts.Game.SaveSystem;
using Assets._Scripts.Game.Statistycs;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

public class MenuManager : MonoBehaviour, ISavable
{
	[Header("Скрываемые UI элементы")]
	[SerializeField] private GameObject continueButton;
	[Header("Якоря")]
	[SerializeField] private Transform languageSelectGrid;
	[Header("Сохраняемые UI эелементы:")]
	[SerializeField] private Slider masterSlider;
	[SerializeField] private Slider musicSlider;
	[SerializeField] private Slider soundEffectsSlider;
	[Header("Префабы UI элементов")]
	[SerializeField] private GameObject languageButtonPrefab;
    [SerializeField] private GameObject videoPlayerPrefab;
    [SerializeField] private GameObject buttons;
    [SerializeField] private GameObject image;
	[SerializeField] private GameObject loadingImage;
    [SerializeField] private RawImage rawImage;

	private StatisticsManager statisticsManager;
    private GameObject currentVideoPlayer;
    private void Awake()
	{
		Debug.Log("Добавили слушатель");
		SubscribeToSaveEvent();
		SaveEvents.OnSettingsLoadEvent.AddListener(InitializeUI);
	}

	private void Start()
	{

		statisticsManager = GameManager.Instance.GetStatsManager();
	}

	public void NewGame()
	{
		StartCoroutine(StartDelay());
		statisticsManager.StartSession();
	}

    IEnumerator StartDelay()
    {
        buttons.SetActive(false);
        image.SetActive(false);
        loadingImage.SetActive(true);
        rawImage.gameObject.SetActive(true);

        if (currentVideoPlayer != null)
            Destroy(currentVideoPlayer);

        currentVideoPlayer = Instantiate(videoPlayerPrefab);
        VideoPlayer vp = currentVideoPlayer.GetComponent<VideoPlayer>();
        RenderTexture rt = new RenderTexture(Screen.width, Screen.height, 0);
        vp.renderMode = VideoRenderMode.RenderTexture;
        vp.targetTexture = rt;
        rawImage.texture = rt;
        vp.Play();

        yield return new WaitForSeconds(8);

        SaveManager saveManager = SaveManager.Instance;
        saveManager.ClearAllCathy();
        saveManager.LoadSave();
        GameManager.Instance.ChangeDimensions(1);
        yield return new WaitUntil(() => SceneManager.GetActiveScene().buildIndex == 1);

        rawImage.gameObject.SetActive(false);
        rt.Release();
        Destroy(rt);
        loadingImage.SetActive(false);
        buttons.SetActive(true);
        image.SetActive(true);
        gameObject.SetActive(false);
    }
    IEnumerator ContinueDelay()
    {
        SaveManager saveManager = SaveManager.Instance;
        saveManager.LoadSave();
        int dimensionind = (int)SaveManager.Instance.GetFloat("dimension");
        buttons.SetActive(false);
        image.SetActive(false);
        loadingImage.SetActive(true);
        rawImage.gameObject.SetActive(true);

        if (currentVideoPlayer != null)
            Destroy(currentVideoPlayer);

        currentVideoPlayer = Instantiate(videoPlayerPrefab);
        VideoPlayer vp = currentVideoPlayer.GetComponent<VideoPlayer>();
        RenderTexture rt = new RenderTexture(Screen.width, Screen.height, 0);
        vp.renderMode = VideoRenderMode.RenderTexture;
        vp.targetTexture = rt;
        rawImage.texture = rt;
        vp.Play();

        yield return new WaitForSeconds(8);

        GameManager.Instance.ChangeDimensions(dimensionind);

        rawImage.gameObject.SetActive(false);
        rt.Release();
        Destroy(rt);
        loadingImage.SetActive(false);
        buttons.SetActive(true);
        image.SetActive(true);

        if (dimensionind != 0)
        {
            gameObject.SetActive(false);
        }
        else
        {
            gameObject.SetActive(true);
            saveManager.ActivateValidator();
        }
    }

    public void Continue()
	{
		StartCoroutine(ContinueDelay());

		statisticsManager.StartSession();
	}

	public void InitializeUI()
	{
		SyncData(new SavablePrefab());
		GameSetupEvents.OnUiGenerated.Invoke(); if (languageSelectGrid.childCount == 0)
		{
			GenerateLanguageButtons();
		}
		Debug.Log(SaveManager.Instance.IsSavePresent());
		continueButton.SetActive(SaveManager.Instance.IsSavePresent());
	}

	public void Quit()
	{
		Application.Quit();
	}

	private void GenerateLanguageButtons()
	{
		LanguageManager languageManager = LanguageManager.Instance;
		//if (languageSelectGrid.childCount > 1)
		//{
		//	while (languageSelectGrid.childCount > 1)
		//	{
		//		Destroy(languageSelectGrid.GetChild(0).gameObject);
		//	}
		//}
		try
		{
			List<string> langs = languageManager.GetLanguages();
			foreach (string language in langs)
			{
				GameObject newButton = Instantiate(languageButtonPrefab, languageSelectGrid);
				newButton.GetComponent<Button>().onClick.AddListener(() => ChangeLanguage(language));
				newButton.GetComponentInChildren<TextMeshProUGUI>().text = language.Split('.')[0];
			}
		}
		catch
		{
			Debug.Log("<color=red>Языков не существует/битый файл!");
		}
	}

	public void ChangeLanguage(string lang)
	{
		LanguageManager.Instance.ChangeLanguage(lang);
	}

	public void SubscribeToSaveEvent()
	{
		SaveEvents.OnSettingsSaveEvent.AddListener(SaveData);
	}

	public void SaveData()
	{
		SaveManager saveManager = SaveManager.Instance;

		saveManager.SaveFloatOption("masterVolume", masterSlider.value);
		saveManager.SaveFloatOption("musicVolume", musicSlider.value);
		saveManager.SaveFloatOption("sfxVolume", soundEffectsSlider.value);
	}

	public void SyncData(SavablePrefab data)
	{
		SaveManager saveManager = SaveManager.Instance;

		masterSlider.value = saveManager.GetFloatOption("masterVolume");
		musicSlider.value = saveManager.GetFloatOption("musicVolume");
		soundEffectsSlider.value = saveManager.GetFloatOption("sfxVolume");
	}
}
