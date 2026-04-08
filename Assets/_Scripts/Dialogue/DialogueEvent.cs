using Assets._Scripts.Game;
using Assets._Scripts.NPC;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum DialogueType
{
    Task,
    WrongColor,
    RightColor
}

public class DialogueEvent : MonoBehaviour
{
    [SerializeField] private GameObject dialogueMenu;
    [SerializeField] private Image characterLeftImage;
    [SerializeField] private Image characterRightImage;
    [SerializeField] private float leftCharacterX;
    [SerializeField] private float rightCharacterX;
    [SerializeField] private TextMeshProUGUI leftCharacterName;
    [SerializeField] private TextMeshProUGUI rightCharacterName;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private Vector2 leftTextPos;
    [SerializeField] private Vector2 rightTextPos;
    [SerializeField] private Transform imagesPivot;
    [SerializeField] private Vector2 leftPivotPos;
    [SerializeField] private Vector2 rightPivotPos;
    private Coroutine dialogueRoutine;
    [SerializeField] private TextAnimations textAnimations;
    [SerializeField] private VoicePlayer voicePlayer;

    [SerializeField] private List<GameObject> uiToHide = new List<GameObject>();

    public bool IsTalking() => dialogueRoutine != null;

    public void Cancel()
    {
        try
        {
            StopCoroutine(dialogueRoutine);
            dialogueMenu.SetActive(false);
            dialogueText.gameObject.SetActive(false);
            leftCharacterName.gameObject.SetActive(false);
            rightCharacterName.gameObject.SetActive(false);
            NPCTicker npcTicker = PocketTicker.Instance.gameObject.GetComponent<NPCTicker>();
            npcTicker.SetShouldTick(true);

            PlayerController playerController = GameObject.FindFirstObjectByType<PlayerController>();
            playerController.SetCanWalk(true);

            PlayerGrabber grabber = GameObject.Find("Player").GetComponent<PlayerGrabber>();
            grabber.SetIsTalking(false);
        }
        catch { }
    }

    public void InvokeDialogue(string currentEventFolder, string dialogueName, bool shouldGiveTask = false)
    {
        DialogueModel dialogue = JsonAccessor.GetDialogueInfo(currentEventFolder, dialogueName);
        if (dialogue == null)
        {
            Debug.Log($"<color=red>Нет диалога по адресу {currentEventFolder}/{dialogueName}");
            return;
        }
        dialogueMenu.SetActive(true);
        dialogueText.gameObject.SetActive(true);
        leftCharacterName.gameObject.SetActive(true);
        dialogueRoutine = StartCoroutine(DialogueCycle(dialogue, dialogueName, currentEventFolder,shouldGiveTask));
        //dialogueMenu.transform.eulerAngles = Camera.main.transform.eulerAngles;
    }

    private IEnumerator DialogueCycle(DialogueModel model, string dialogueName, string currentEventFolder, bool shouldGiveTask = false)
    {
		PlayerController playerController = GameObject.FindFirstObjectByType<PlayerController>();
		CameraController cameraController = GameObject.FindFirstObjectByType<CameraController>();

        NPCTicker npcTicker = PocketTicker.Instance.gameObject.GetComponent<NPCTicker>();

		uiToHide.ForEach(x => x.SetActive(false));
		playerController.SetCanWalk(false);
		//cameraController.SetShouldRotate(false);
        npcTicker.SetShouldTick(false);

		var replics = model.replics;
        leftCharacterName.text = model.characterA;
        //rightCharacterName.text = model.characterB;
        //rightCharacterName.gameObject.SetActive(true);
        characterLeftImage.sprite = Resources.Load<Sprite>($"DialogueSprites/{model.characterA}");
        characterRightImage.sprite = Resources.Load<Sprite>($"DialogueSprites/{model.characterB}");
        PlayerGrabber grabber = GameObject.Find("Player").GetComponent<PlayerGrabber>();
        grabber.SetIsTalking(true);
        textAnimations.StartMoveText(leftTextPos, leftCharacterName.transform, 10f);
        textAnimations.StartMoveImage(rightPivotPos, imagesPivot, 10f);
		yield return null;
		for (int i = 0; i < replics.Count; i++)
        {
            string[] fullString = replics[i].Split(":");
            dialogueText.text = fullString[1];
            string speaker = fullString[0];
            voicePlayer.PlayNext(speaker);
            if (fullString[0] == model.characterA)
            {
                leftCharacterName.text = model.characterA;
                leftCharacterName.alignment = TextAlignmentOptions.Left;
                textAnimations.StartMoveText(leftTextPos, leftCharacterName.transform, 10f);
                textAnimations.StartMoveImage(rightPivotPos, imagesPivot, 10f);
            }
            else if (fullString[0] == model.characterB)
            {
                leftCharacterName.text = model.characterB;
                leftCharacterName.alignment = TextAlignmentOptions.Right;
                textAnimations.StartMoveText(rightTextPos, leftCharacterName.transform, 10f);
                textAnimations.StartMoveImage(leftPivotPos, imagesPivot, 10f);
            }
            while (!Input.GetMouseButtonDown(0))
            {
                yield return null;
            }
            yield return null;
        }
        if (dialogueName == "task" && !GameManager.Instance.GetBook().GetShowTask())
		{
			GameManager.Instance.GetBook().TakeTask(currentEventFolder);
			GameManager.Instance.GetBook().ToggleBook(bypassLock:true);
			GameManager.Instance.GetTutorial().ProgressTutorial("talk");
			while (!Input.GetMouseButtonDown(0))
			{
				yield return null;
			}
		}
        dialogueMenu.SetActive(false);
        dialogueText.gameObject.SetActive(false);
        leftCharacterName.gameObject.SetActive(false);
        rightCharacterName.gameObject.SetActive(false);
		npcTicker.SetShouldTick(true);
		grabber.SetIsTalking(false);
        playerController.SetCanWalk(true);

		uiToHide.ForEach(x => x.SetActive(true));
		if (shouldGiveTask)
        {
            Debug.Log("<color=green>Пропускаем до таска!");
            InvokeDialogue(currentEventFolder,"task");

        }
        else
        {
            dialogueRoutine = null;
            yield break;
        }
		//cameraController.SetShouldRotate(true);
	}
}