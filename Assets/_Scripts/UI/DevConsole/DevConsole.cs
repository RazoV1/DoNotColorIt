using Assets._Scripts.Events;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DevConsole : MonoBehaviour
{
	[SerializeField] private GameObject logPrefab;
	[SerializeField] private TMP_InputField inputField;
	[SerializeField] private Transform contentObject;
	[SerializeField] private GameObject console;
	private bool shouldAwaitInput = false;
	public void SetShouldAwaitInput(bool shouldAwaitInput) { this.shouldAwaitInput = shouldAwaitInput; }

	private void Awake()
	{
		CommandEvents.Register();
		CommandEvents.OnHelp.AddListener(HelpCommand);
		CommandEvents.OnSmolaGive.AddListener(AddSmolaCommand);
	}

	private void Update()
	{
		HandleInput();
	}

	private void HandleInput()
	{
		if (Input.GetKeyDown(KeyCode.BackQuote))
		{
			console.SetActive(!console.activeInHierarchy);
			if (SceneManager.GetActiveScene().buildIndex != 0)
			{
				Cursor.visible = console.activeInHierarchy;
				Cursor.lockState = console.activeInHierarchy ? CursorLockMode.None : CursorLockMode.Locked;
			}
		}
		if (Input.GetKeyDown(KeyCode.Return))
		{
			ReadCommand();
		}
	}

	private void ReadCommand()
	{
		string command = inputField.text;
		inputField.text = "";

		TextMeshProUGUI log = Instantiate(logPrefab, contentObject).GetComponent<TextMeshProUGUI>();
		ProcessCommand(command, log);
	}

	private void ProcessCommand(string command, TextMeshProUGUI text)
	{
		List<string> splittedCommand = command.Split().ToList();
		int commandArgument = 0;
		if (splittedCommand.Count > 1)
		{
			Debug.Log(splittedCommand[1]);
			if (int.TryParse(splittedCommand[1], out commandArgument))
			{
				Debug.Log("Parsed!");
				commandArgument = int.Parse(splittedCommand[1]);
			}
		}
		string commandBody = splittedCommand[0];
		string response = "";

		bool wasSuccsesful = CommandEvents.TryInvokeEvent(commandBody, commandArgument != 0 ? commandArgument : 0);

		if (wasSuccsesful)
		{
			response = command;
		}
		else
		{
			response = "<color=red>Ошибка в команде! /help для списка комманд";
		}
		text.text = response;
	}

	private void HelpCommand(int placeholder)
	{
		TextMeshProUGUI log = Instantiate(logPrefab, contentObject).GetComponent<TextMeshProUGUI>();
		log.text = "------------------------";

		log = Instantiate(logPrefab, contentObject).GetComponent<TextMeshProUGUI>();
		log.text = "/skipTutorial - Пропустить туториал";

		log = Instantiate(logPrefab, contentObject).GetComponent<TextMeshProUGUI>();
		log.text = "/giveLogs n - Заспавнить n брёвен. Используйте <color=green>только в багажнике</color>, пожалуйста =)";

		log = Instantiate(logPrefab, contentObject).GetComponent<TextMeshProUGUI>();
		log.text = "/giveSmola n - Добавить в хранилище n смолы. Используйте <color=green>только в багажнике</color>, пожалуйста =)";

		//log = Instantiate(logPrefab, contentObject).GetComponent<TextMeshProUGUI>();
		//log.text = "/clear - Очистить консоль";

		log = Instantiate(logPrefab, contentObject).GetComponent<TextMeshProUGUI>();
		log.text = "------------------------";
	}

	private void AddSmolaCommand(int amount)
	{
		for (int i = 0; i < amount; i++)
		{
			PocketTicker.Instance.AddSmola();
		}
	}

	private void Clear(int placeholder)
	{
		int logsCount = contentObject.childCount;
		List<Transform> childrenOfTheCity = new List<Transform>();

		for (int i = 0; i < logsCount; i++)
		{
			childrenOfTheCity.Add(contentObject.GetChild(i));
		}

	}
}
