using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class MultiplayerLevelManager : MonoBehaviourPunCallbacks
{
    public int MaxKills = 3;
    public GameObject GameOverPopUp;
    public Text WinnerText;
    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.Instantiate("Multiplayer", new Vector3(0, 0, 0), Quaternion.identity);
    }

    public override void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        //if player reaches maximum kills, activate game over pop up and display winner
        if(targetPlayer.GetScore() == MaxKills)
        {
            WinnerText.text = targetPlayer.NickName;
            GameOverPopUp.SetActive(true);
            StorePersonalBest();
        }
    }

    private void StorePersonalBest()
    {
        var newScore = PhotonNetwork.LocalPlayer.GetScore();
        var playerData = GameManager.Instance.playerData;

        if(newScore > playerData.bestScore)
        {
            playerData.username = PhotonNetwork.LocalPlayer.NickName;
            playerData.bestScore = newScore;
            playerData.date = DateTime.UtcNow;
            playerData.totalPlayersInTheGame = PhotonNetwork.CurrentRoom.PlayerCount;
            playerData.roomName = PhotonNetwork.CurrentRoom.Name;

            GameManager.Instance.GlobalLeaderboard.SubmitScore(newScore);
            GameManager.Instance.SavePlayerData();
        }
    }

    public void LeaveGame()
    {
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        PhotonNetwork.Disconnect();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        SceneManager.LoadScene("MultiplayerLobby");
    }


}
