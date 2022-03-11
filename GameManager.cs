using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public PlayerData playerData;
    public string FilePath;
    public GlobalLeaderboard GlobalLeaderboard;
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);
    }

    private void Start()
    {
        LoadPlayerData();
        LoginToPlayFab();
    }

    public void LoadPlayerData()
    {
        if (!File.Exists(FilePath))
        {
            playerData = new PlayerData();
            SavePlayerData();
        }

        var fileContents = File.ReadAllText(FilePath);
        playerData = JsonConvert.DeserializeObject<PlayerData>(fileContents);
    }

    private void LoginToPlayFab()
    {
        var request = new LoginWithCustomIDRequest()
        {
            CreateAccount = true,
            CustomId = playerData.id,
        };
        PlayFabClientAPI.LoginWithCustomID(request, PlayFabLoginResult, PlayFabLoginError);
    }

    private void PlayFabLoginResult(LoginResult loginResult)
    {
        Debug.Log("PlayFab - Logged in: " + loginResult.ToJson());
    }

    private void PlayFabLoginError(PlayFabError playFabError)
    {
        Debug.Log("PlayFab - Login false: " + playFabError.ErrorMessage);
    }

    public void SavePlayerData()
    {
        var serializedData = JsonConvert.SerializeObject(playerData);
        File.WriteAllText(FilePath, serializedData);
    }


}
