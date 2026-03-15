using Assets._Scripts.Events;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CursorHint : MonoBehaviour
{
	[SerializeField] private TextMeshProUGUI hintText;
	[SerializeField] private Image hintSprite;
	[SerializeField] private MouseHints currentHint;
	[SerializeField] private Transform director;
	[Header("Sprites")]
	[SerializeField] private Sprite interactCircular;
	[SerializeField] private Sprite interactSprite;
	[SerializeField] private Sprite interactHorizontal;
	[SerializeField] private Sprite interactVertical;

	private string lmb;
	private string talk;
	private string talkNext;
	private string getOut;
	private string getIn;
	private string enterPocket;
	private string toggleMode;
	private string toggleInfuser;

	public MouseHints GetCurrentHint() => currentHint;

	public void Awake()
	{
		GameSetupEvents.OnTranslationLoaded.AddListener(GetTranslatableString);
	}

	private void GetTranslatableString()
	{
		lmb = LanguageManager.Instance.GetTranslatable("ui.hint.lmb");
		talk = LanguageManager.Instance.GetTranslatable("ui.hint.talk");
		talkNext = LanguageManager.Instance.GetTranslatable("ui.hint.talk_next");
		getIn = LanguageManager.Instance.GetTranslatable("ui.hint.get_in");
		enterPocket = LanguageManager.Instance.GetTranslatable("ui.hint.enter_pocket");
		toggleMode = LanguageManager.Instance.GetTranslatable("ui.hint.toggle_mode");
		toggleInfuser = LanguageManager.Instance.GetTranslatable("ui.hint.toggle_infuser");
		Debug.Log(toggleMode);
	}

	public void ShowHint(MouseHints hintType)
	{
		hintText.text = "";
		hintSprite.enabled = false;
		currentHint = hintType;
		switch (hintType)
		{
			case (MouseHints.Talk):
				hintText.text = talk;
				break;
			case (MouseHints.EnterPocket):
				hintText.text = enterPocket;
				break;
			case MouseHints.GetIn:
				hintText.text = getIn;
				break;
			case MouseHints.vertical:
				hintSprite.enabled = true;
				hintSprite.sprite = interactVertical; 
				break;
			case MouseHints.horizontal:
				hintSprite.enabled = true;
				hintSprite.sprite = interactHorizontal;
				break;
			case MouseHints.Circular:
				hintSprite.enabled = true;
				hintSprite.sprite = interactCircular; 
				break;
			case MouseHints.Default:
				hintSprite.enabled = true;
				hintSprite.sprite = interactSprite; 
				break;
			case MouseHints.None:
				hintSprite.sprite = null;
				hintSprite.enabled = false;
				break;
			case MouseHints.TalkMouse:
				hintSprite.sprite = null;
				hintText.text = talkNext;
				break;
			case MouseHints.ToggleMode:
				hintSprite.sprite = null;
				hintText.text = toggleMode;
				break;
			case MouseHints.ToggleInfuser:
				hintSprite.sprite = null;
				hintText.text = toggleInfuser;
				break;
		}
	}

	public void ClearHint()
	{
		hintSprite.enabled = false;
		currentHint = MouseHints.None;
		hintText.text = "";
	}
}

public enum MouseHints
{
	None,
	GetIn,
	GetOut,
	Talk,
	EnterPocket,
	Circular,
	Default,
	horizontal,
	vertical,
	TalkMouse,
	ToggleMode,
	ToggleInfuser
}