using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class MainMenu : MonoBehaviourPunCallbacks {
    [Header("Menu Panels")]
    [SerializeField] private GameObject mainScreenPanel;
    [SerializeField] private GameObject lobbyPanel;

    [Header("Main Menu")]
    [SerializeField] private Button newGameButton;
    [SerializeField] private TextMeshProUGUI newGameButtonText;
    [SerializeField] private TMP_InputField playerName;


    [Header("Lobby")]
    [SerializeField] private TextMeshProUGUI playerListText;
    [SerializeField] private TextMeshProUGUI waitingText;
    [SerializeField] private Button leaveLobbyButton;

    void Start() {
        newGameButton.interactable = false;
    }

    public override void OnConnectedToMaster() {
        playerName.interactable = true;
        newGameButton.interactable = true;
        newGameButtonText.color = Color.white;
        playerName.text = PlayerPrefs.GetString("LastNickName");
    }

    public void SetScreen(GameObject screen) {
        mainScreenPanel.SetActive(false);
        lobbyPanel.SetActive(false);

        screen.SetActive(true);
    }
    
    public void OnNewGameButton() {
        NetworkManager.instance.JoinRoom();
    }

    public void OnPlayerNameUpdate(TMP_InputField playerNameInput) {
        PhotonNetwork.NickName = playerNameInput.text;
        PlayerPrefs.SetString("LastNickName", playerNameInput.text);
    }

    public override void OnJoinedRoom() {
        SetScreen(lobbyPanel);
        photonView.RPC("UpdateLobbyUI", RpcTarget.All);
    }

    public void OnCancelLobbyButton() {
        PhotonNetwork.LeaveRoom();
        SetScreen(mainScreenPanel);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer) {
        UpdateLobbyUI();
    }

    [PunRPC]
    public void UpdateLobbyUI() {
        playerListText.text = "";

        foreach (Player player in PhotonNetwork.PlayerList) {
            playerListText.text += player.NickName + "\n";
        }

        if (PhotonNetwork.PlayerList.Length == PhotonNetwork.CurrentRoom.MaxPlayers) {
            StartCoroutine(StartGame());
        }
    }

    private IEnumerator StartGame()
    {
        int timer = 0;

        while (timer > 0)
        {
            waitingText.text = "Game starting in " + timer;
            yield return new WaitForSeconds(1f);
            timer--;
        }

        NetworkManager.instance.photonView.RPC("ChangeScene", RpcTarget.All, "Prototype");
    }
}