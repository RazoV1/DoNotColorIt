using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Assets._Scripts.Events;
using TMPro;
using Assets._Scripts.Game.SaveSystem;
using Assets._Scripts.Audio;
using System.Linq;

public class GameManager : MonoBehaviour, ISavable
{
	public static GameManager Instance;

	[SerializeField] private DialogueEvent dialogue;
	[SerializeField] private float acceptableFluct;
	[SerializeField] private AudioManager audioManager;
	[SerializeField] private TutorialManager tutorialManager;
	private string currentNpcTaskName;

	private Coroutine dimnsionChangeRoutine;
	private int currentDimension = 0;

	[SerializeField] private int currentTaskIndex;
	[Header("UI elements")]
	[SerializeField] private GameObject pauseScreen;
	[SerializeField] private GameObject loadingScreen;
	[SerializeField] private TextMeshProUGUI loadingText;
	[SerializeField] private Book book;
	[SerializeField] private MenuManager menuManager;
	[SerializeField] private CursorHint cursorHint;

	public TutorialManager GetTutorial() => tutorialManager;

	public void SetCurrentTaskName(string currentNpcTaskName) { this.currentNpcTaskName = currentNpcTaskName; }

	public CursorHint GetCursorHint() => cursorHint;

	public int GetCurrentTaskIndex() => currentTaskIndex;

	public Book GetBook() => book;

	public void DisruptDialogue()
	{
		dialogue.Cancel();
	}

	public void HandlePauseInput()
	{
		if (Input.GetKeyDown(KeyCode.Escape) && SceneManager.GetActiveScene().buildIndex != 0 && !menuManager.gameObject.activeSelf)
		{
			pauseScreen.SetActive(!pauseScreen.activeSelf);
			Time.timeScale = pauseScreen.activeSelf ? 0.0001f : 1f;
			Cursor.lockState = pauseScreen.activeSelf ? CursorLockMode.None : CursorLockMode.Locked;
			Cursor.visible = pauseScreen.activeSelf;
			GameplayEvents.OnPauseToggled.Invoke(pauseScreen.activeSelf);
		}
		else if (!pauseScreen.activeSelf && Input.GetKeyDown(KeyCode.Tab) && -SceneManager.GetActiveScene().buildIndex != 0)
		{
			book.ToggleBook();
		}
	}

	public void ClosePauseByButton()
	{
		pauseScreen.SetActive(false);
		Time.timeScale = pauseScreen.activeSelf ? 0.0001f : 1f;
		Cursor.lockState = pauseScreen.activeSelf ? CursorLockMode.None : CursorLockMode.Locked;
		Cursor.visible = pauseScreen.activeSelf;
		GameplayEvents.OnPauseToggled.Invoke(false);
	}

	public void ToMenu()
	{
		pauseScreen.SetActive(false);
		Time.timeScale = 1f;
		GameplayEvents.OnPauseToggled.Invoke(false);
		Cursor.visible = true;
		Cursor.lockState = CursorLockMode.None;
		menuManager.gameObject.SetActive(true);
		ChangeDimensions(0, false);
	}

	public void Update()
	{
		HandlePauseInput();
	}

	/// <summary>
	///"Nonetheless… We must finish this. If these wrongdoings can finally cut this vicious cycle once and for all… I will gladly take this burden upon myself."
	/// </summary>
	/// <param name="npc"></param>
	/// <param name="color"></param>
	/// <param name="bucket"></param>
	public void StartDialogueForCurrentIndex(NPCWaiter npc, Color color, Bucket bucket = null)
	{
		//int clusterIndex = Mathf.CeilToInt(npc.GetColorTasks()[0].id / 2);
		int clusterIndex = Mathf.CeilToInt(currentTaskIndex / 2);
		if (bucket == null)
		{
			if (!npc.GetWasIntroduced())
			{
				dialogue.InvokeDialogue(npc.GetName(), "Hello");
				npc.SetWasIntroduced();
				return;
			}
			if (!npc.GetGaveTask() && npc.GetColorTasks().Where(x => x.id == currentTaskIndex).ToList().Count > 0)
			{
				if (npc.GetName() == "DedMiron")
				{
					tutorialManager.ProgressTutorial(1);
				}
				if (book.GetShowTask()) { return; }
				npc.SetGaveTask(true);
				SetCurrentTaskName(npc.GetName());
				dialogue.InvokeDialogue(npc.GetName(), "task");
				book.TakeTask(npc.GetName());
			}
			else if (npc.GetIsCompleted() || npc.GetColorTasks().Where(x => x.id == currentTaskIndex).ToList().Count == 0)
			{
				dialogue.InvokeDialogue(npc.GetName(), $"{clusterIndex}");
			}
		}
		else if (IsCorrectColor(color, bucket.GetColor()))
		{
			Destroy(bucket.gameObject);
			if (npc.GetName() == "DedMiron")
			{
				GameManager.Instance.GetTutorial().ProgressTutorial(11);
			}
			dialogue.InvokeDialogue(npc.GetName(), $"{clusterIndex}");
			npc.SetCompleted(true);
			currentTaskIndex++;
			book.RemoveTask();
			npc.PaintHouse();
		}
		else
		{
			Destroy(bucket.gameObject);
			if (npc.GetName() == "DedMiron")
			{

				GameManager.Instance.GetTutorial().ProgressTutorial(11);
			}
			dialogue.InvokeDialogue(npc.GetName(), "incorrect");
		}
	}

	private bool IsCorrectColor(Color color, Color original)
	{
		float deltaR = Mathf.Abs(original.r - color.r);
		float deltaG = Mathf.Abs(original.g - color.g);
		float deltaB = Mathf.Abs(original.b - color.b);

		return deltaR <= acceptableFluct && deltaG <= acceptableFluct && deltaB <= acceptableFluct;
	}

	public void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
			DontDestroyOnLoad(gameObject);
			SubscribeToSaveEvent();
		}
		else
		{
			Destroy(gameObject);
		}
	}

	public void Start()
	{
		GameplayEvents.ChangeDimensionsEvent.Invoke(SceneManager.GetActiveScene().buildIndex);
	}

	private IEnumerator DimensionChangeEnum(int index)
	{
		string loadingString = LanguageManager.Instance.GetTranslatable("ui.loading.text");
		AsyncOperation operation = SceneManager.LoadSceneAsync(index);
		while (!operation.isDone)
		{
			float progress = Mathf.Clamp01(operation.progress / 0.9f);
			loadingText.text = $"{loadingString}:{progress * 100}%";
			yield return null;
		}
		loadingText.text = "";
		GameplayEvents.ChangeDimensionsEvent.Invoke(index);
		SaveManager.Instance.LoadSave();
		book.GetComponent<Canvas>().worldCamera = Camera.main.GetComponentsInChildren<Camera>().First(cam => cam.gameObject.tag == "UI_cam");
		ClearRoutine();
	}

	public void ClearRoutine()
	{
		StopCoroutine(dimnsionChangeRoutine);
		dimnsionChangeRoutine = null;
	}

	public void ChangeDimensions(int index, bool saveDimensionIndex = true)
	{
		if (dimnsionChangeRoutine != null)
		{
			Debug.Log("LLL");
			return;
		}
		if (saveDimensionIndex)
		{
			currentDimension = index;
		}
		SaveManager.Instance.SaveToSlot();
		dimnsionChangeRoutine = StartCoroutine(DimensionChangeEnum(index));
	}

	public void SubscribeToSaveEvent()
	{
		SaveEvents.OnSaveEvent.AddListener(SaveData);
		SaveEvents.OnLoadEvent.AddListener(SyncDataEmpty);
	}

	private void SyncDataEmpty()
	{
		SyncData(new SavablePrefab());
	}

	public void SaveData()
	{
		SaveManager saveManager = SaveManager.Instance;
		saveManager.SaveFloat("dimension", currentDimension);
		saveManager.SaveFloat("currentTaskIndex", currentTaskIndex);
		saveManager.SaveString("currentNpcTaskName",currentNpcTaskName);
	}

	public void SyncData(SavablePrefab data)
	{
		SaveManager saveManager = SaveManager.Instance;

		currentDimension = (int)saveManager.GetFloat("dimension");
		currentTaskIndex = (int)saveManager.GetFloat("currentTaskIndex");
		currentNpcTaskName = saveManager.GetString("currentNpcTaskName");

		book.UpdateTask(currentNpcTaskName);
	}
}