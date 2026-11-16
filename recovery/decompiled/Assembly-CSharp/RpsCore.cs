using System;
using System.Collections;
using Photon;
using UnityEngine;
using UnityEngine.UI;

public class RpsCore : PunBehaviour, IPunTurnManagerCallbacks
{
	public enum Hand
	{
		None,
		Rock,
		Paper,
		Scissors
	}

	public enum ResultType
	{
		None,
		Draw,
		LocalWin,
		LocalLoss
	}

	[SerializeField]
	private RectTransform ConnectUiView;

	[SerializeField]
	private RectTransform GameUiView;

	[SerializeField]
	private CanvasGroup ButtonCanvasGroup;

	[SerializeField]
	private RectTransform TimerFillImage;

	[SerializeField]
	private Text TurnText;

	[SerializeField]
	private Text TimeText;

	[SerializeField]
	private Text RemotePlayerText;

	[SerializeField]
	private Text LocalPlayerText;

	[SerializeField]
	private Image WinOrLossImage;

	[SerializeField]
	private Image localSelectionImage;

	public Hand localSelection;

	[SerializeField]
	private Image remoteSelectionImage;

	public Hand remoteSelection;

	[SerializeField]
	private Sprite SelectedRock;

	[SerializeField]
	private Sprite SelectedPaper;

	[SerializeField]
	private Sprite SelectedScissors;

	[SerializeField]
	private Sprite SpriteWin;

	[SerializeField]
	private Sprite SpriteLose;

	[SerializeField]
	private Sprite SpriteDraw;

	[SerializeField]
	private RectTransform DisconnectedPanel;

	private ResultType result;

	private PunTurnManager turnManager;

	public Hand randomHand;

	private bool IsShowingResults;

	public void Start()
	{
		turnManager = gameObject.AddComponent<PunTurnManager>();
		turnManager.TurnManagerListener = this;
		turnManager.TurnDuration = 5f;
		localSelectionImage.gameObject.SetActive(value: false);
		remoteSelectionImage.gameObject.SetActive(value: false);
		StartCoroutine("CycleRemoteHandCoroutine");
		RefreshUIViews();
	}

	public void Update()
	{
		if (DisconnectedPanel == null)
		{
			UnityEngine.Object.Destroy(gameObject);
		}
		if (Input.GetKeyUp(KeyCode.L))
		{
			PhotonNetwork.LeaveRoom();
		}
		if (Input.GetKeyUp(KeyCode.C))
		{
			PhotonNetwork.ConnectUsingSettings(null);
			PhotonHandler.StopFallbackSendAckThread();
		}
		if (!PhotonNetwork.inRoom)
		{
			return;
		}
		if (PhotonNetwork.connected && DisconnectedPanel.gameObject.GetActive())
		{
			DisconnectedPanel.gameObject.SetActive(value: false);
		}
		if (!PhotonNetwork.connected && !PhotonNetwork.connecting && !DisconnectedPanel.gameObject.GetActive())
		{
			DisconnectedPanel.gameObject.SetActive(value: true);
		}
		if (PhotonNetwork.room.playerCount > 1)
		{
			if (turnManager.IsOver)
			{
				return;
			}
			if (TurnText != null)
			{
				TurnText.text = turnManager.Turn.ToString();
			}
			if (turnManager.Turn > 0 && TimeText != null && !IsShowingResults)
			{
				TimeText.text = turnManager.RemainingSecondsInTurn.ToString("F1") + " SECONDS";
				TimerFillImage.anchorMax = new Vector2(1f - turnManager.RemainingSecondsInTurn / turnManager.TurnDuration, 1f);
			}
		}
		UpdatePlayerTexts();
		Sprite sprite = SelectionToSprite(localSelection);
		if (sprite != null)
		{
			localSelectionImage.gameObject.SetActive(value: true);
			localSelectionImage.sprite = sprite;
		}
		if (turnManager.IsCompletedByAll)
		{
			sprite = SelectionToSprite(remoteSelection);
			if (sprite != null)
			{
				remoteSelectionImage.color = new Color(1f, 1f, 1f, 1f);
				remoteSelectionImage.sprite = sprite;
			}
			return;
		}
		ButtonCanvasGroup.interactable = PhotonNetwork.room.playerCount > 1;
		if (PhotonNetwork.room.playerCount < 2)
		{
			remoteSelectionImage.color = new Color(1f, 1f, 1f, 0f);
		}
		else if (turnManager.Turn > 0 && !turnManager.IsCompletedByAll)
		{
			PhotonPlayer next = PhotonNetwork.player.GetNext();
			float a = 0.5f;
			if (turnManager.GetPlayerFinishedTurn(next))
			{
				a = 1f;
			}
			if (next != null && next.isInactive)
			{
				a = 0.1f;
			}
			remoteSelectionImage.color = new Color(1f, 1f, 1f, a);
			remoteSelectionImage.sprite = SelectionToSprite(randomHand);
		}
	}

	public void OnTurnBegins(int turn)
	{
		Debug.Log("OnTurnBegins() turn: " + turn);
		localSelection = Hand.None;
		remoteSelection = Hand.None;
		WinOrLossImage.gameObject.SetActive(value: false);
		localSelectionImage.gameObject.SetActive(value: false);
		remoteSelectionImage.gameObject.SetActive(value: true);
		IsShowingResults = false;
		ButtonCanvasGroup.interactable = true;
	}

	public void OnTurnCompleted(int obj)
	{
		Debug.Log("OnTurnCompleted: " + obj);
		CalculateWinAndLoss();
		UpdateScores();
		OnEndTurn();
	}

	public void OnPlayerMove(PhotonPlayer photonPlayer, int turn, object move)
	{
		Debug.Log(string.Concat("OnPlayerMove: ", photonPlayer, " turn: ", turn, " action: ", move));
		throw new NotImplementedException();
	}

	public void OnPlayerFinished(PhotonPlayer photonPlayer, int turn, object move)
	{
		Debug.Log(string.Concat("OnTurnFinished: ", photonPlayer, " turn: ", turn, " action: ", move));
		if (photonPlayer.isLocal)
		{
			localSelection = (Hand)(byte)move;
		}
		else
		{
			remoteSelection = (Hand)(byte)move;
		}
	}

	public void OnTurnTimeEnds(int obj)
	{
		if (!IsShowingResults)
		{
			Debug.Log("OnTurnTimeEnds: Calling OnTurnCompleted");
			OnTurnCompleted(-1);
		}
	}

	private void UpdateScores()
	{
		if (result == ResultType.LocalWin)
		{
			PhotonNetwork.player.AddScore(1);
		}
	}

	public void StartTurn()
	{
		if (PhotonNetwork.isMasterClient)
		{
			turnManager.BeginTurn();
		}
	}

	public void MakeTurn(Hand selection)
	{
		turnManager.SendMove((byte)selection, finished: true);
	}

	public void OnEndTurn()
	{
		StartCoroutine("ShowResultsBeginNextTurnCoroutine");
	}

	public IEnumerator ShowResultsBeginNextTurnCoroutine()
	{
		ButtonCanvasGroup.interactable = false;
		IsShowingResults = true;
		if (result == ResultType.Draw)
		{
			WinOrLossImage.sprite = SpriteDraw;
		}
		else
		{
			WinOrLossImage.sprite = ((result != ResultType.LocalWin) ? SpriteLose : SpriteWin);
		}
		WinOrLossImage.gameObject.SetActive(value: true);
		yield return new WaitForSeconds(2f);
		StartTurn();
	}

	public void EndGame()
	{
		Debug.Log("EndGame");
	}

	private void CalculateWinAndLoss()
	{
		result = ResultType.Draw;
		if (localSelection == remoteSelection)
		{
			return;
		}
		if (localSelection == Hand.None)
		{
			result = ResultType.LocalLoss;
			return;
		}
		if (remoteSelection == Hand.None)
		{
			result = ResultType.LocalWin;
		}
		if (localSelection == Hand.Rock)
		{
			result = ((remoteSelection != Hand.Scissors) ? ResultType.LocalLoss : ResultType.LocalWin);
		}
		if (localSelection == Hand.Paper)
		{
			result = ((remoteSelection != Hand.Rock) ? ResultType.LocalLoss : ResultType.LocalWin);
		}
		if (localSelection == Hand.Scissors)
		{
			result = ((remoteSelection != Hand.Paper) ? ResultType.LocalLoss : ResultType.LocalWin);
		}
	}

	private Sprite SelectionToSprite(Hand hand)
	{
		return hand switch
		{
			Hand.Rock => SelectedRock, 
			Hand.Paper => SelectedPaper, 
			Hand.Scissors => SelectedScissors, 
			_ => null, 
		};
	}

	private void UpdatePlayerTexts()
	{
		PhotonPlayer next = PhotonNetwork.player.GetNext();
		PhotonPlayer player = PhotonNetwork.player;
		if (next != null)
		{
			RemotePlayerText.text = next.name + "        " + next.GetScore().ToString("D2");
		}
		else
		{
			TimerFillImage.anchorMax = new Vector2(0f, 1f);
			TimeText.text = string.Empty;
			RemotePlayerText.text = "waiting for another player        00";
		}
		if (player != null)
		{
			LocalPlayerText.text = "YOU   " + player.GetScore().ToString("D2");
		}
	}

	public IEnumerator CycleRemoteHandCoroutine()
	{
		while (true)
		{
			randomHand = (Hand)UnityEngine.Random.Range(1, 4);
			yield return new WaitForSeconds(0.5f);
		}
	}

	public void OnClickRock()
	{
		MakeTurn(Hand.Rock);
	}

	public void OnClickPaper()
	{
		MakeTurn(Hand.Paper);
	}

	public void OnClickScissors()
	{
		MakeTurn(Hand.Scissors);
	}

	public void OnClickConnect()
	{
		PhotonNetwork.ConnectUsingSettings(null);
		PhotonHandler.StopFallbackSendAckThread();
	}

	public void OnClickReConnectAndRejoin()
	{
		PhotonNetwork.ReconnectAndRejoin();
		PhotonHandler.StopFallbackSendAckThread();
	}

	private void RefreshUIViews()
	{
		TimerFillImage.anchorMax = new Vector2(0f, 1f);
		ConnectUiView.gameObject.SetActive(!PhotonNetwork.inRoom);
		GameUiView.gameObject.SetActive(PhotonNetwork.inRoom);
		ButtonCanvasGroup.interactable = PhotonNetwork.room != null && PhotonNetwork.room.playerCount > 1;
	}

	public override void OnLeftRoom()
	{
		Debug.Log("OnLeftRoom()");
		RefreshUIViews();
	}

	public override void OnJoinedRoom()
	{
		RefreshUIViews();
		if (PhotonNetwork.room.playerCount == 2)
		{
			if (turnManager.Turn == 0)
			{
				StartTurn();
			}
		}
		else
		{
			Debug.Log("Waiting for another player");
		}
	}

	public override void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
	{
		Debug.Log("Other player arrived");
		if (PhotonNetwork.room.playerCount == 2 && turnManager.Turn == 0)
		{
			StartTurn();
		}
	}

	public override void OnPhotonPlayerDisconnected(PhotonPlayer otherPlayer)
	{
		Debug.Log("Other player disconnected! isInactive: " + otherPlayer.isInactive);
	}

	public override void OnConnectionFail(DisconnectCause cause)
	{
		DisconnectedPanel.gameObject.SetActive(value: true);
	}
}
