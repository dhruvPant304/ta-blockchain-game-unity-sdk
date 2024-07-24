using System.Collections;
using System.Collections.Generic;
using TA.Leaderboard;
using TA.Services;
using TA.UserProfile;
using UnityEngine;

public class LeaderBoardView : MonoBehaviour {

    [SerializeField] 

    UserProfileService _userProfileService;
    LeaderboardService _leaderBoardService;


    void Start(){
        _userProfileService = ServiceLocator.Instance.GetService<UserProfileService>();
    }

    public void Show() {
        gameObject.SetActive(true);
    }

    public void Hide(){
        gameObject.SetActive(false);
    }
}
