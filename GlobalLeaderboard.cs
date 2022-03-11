using System;
using PlayFab;
using PlayFab.ClientModels;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalLeaderboard : MonoBehaviour
{
    public int MaxResult = 5;
    public LeaderboardPopUp LeaderboardPopup;
  public void SubmitScore(int playerScore)
    {
        PlayFabClientAPI.UpdatePlayerStatistics(new UpdatePlayerStatisticsRequest
        {
            Statistics = new List<StatisticUpdate>
            {
                new StatisticUpdate
                {
                    StatisticName = "Most kills",
                    Value = playerScore
                }
            }
        }, OnStatisticsUpdated, OnStatisticsUpdateFailed);
       
    }

    private void OnStatisticsUpdateFailed(PlayFabError obj)
    {
        throw new NotImplementedException();
    }

    private void OnStatisticsUpdated(UpdatePlayerStatisticsResult obj)
    {
        throw new NotImplementedException();
    }

    public void GetLeaderboard()
    {
        /* var request = new GetLeaderboardRequest()
         {
             MaxResultsCount = MaxResult,
             StatisticName = "Most Kills"
         };
         PlayFabClientAPI.GetLeaderboard(request,
             (result) => { 
                 Debug.Log("PlayFab - Get leaderboard completed!");
                 LeaderboardPopup.UpdateUI(result.Leaderboard);
             },
             (error) => { Debug.Log("PlayFab - Error occured while retrieving the score: " + error.ErrorMessage); });
        */
        /*PlayFabClientAPI.GetLeaderboard(new GetLeaderboardRequest
        {
            StatisticName = "Most kills",
            StartPosition = 0,
            MaxResultsCount = MaxResult
        }, OnGetLeaderboard, OnRequestLeaderboardFailed);
        */
    }
}
