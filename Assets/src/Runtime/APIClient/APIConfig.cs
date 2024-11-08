using System;
using UnityEngine;

namespace TA.APIClient{
[CreateAssetMenu(menuName="TA/APIService", fileName="APIConfig")]
public class APIConfig : ScriptableObject {

    [Header("Env")]
    public string serverUrl;
    public Environment environment = Environment.Development;

    [Header("Keys")]
    public string encryptionPublicKey;
    public string gameId;

    [Header("User Balance Config")]
    public bool fetchUserBalanceOnLogin = true;

    [Header("Login Config")]
    public string signatureString;
    public string firstLoginMessage = "Welcome to TukTuk Crazy Taxi, We have 20 free credits for you";
    public string firstLoginMessageHeader = "Login Successful!";

    [Header("Credit Shop config")]
    public bool inAppCreditPurchaseAvailable;
    public bool showShopInBrowser;
    public string shopUrl;

    [Header("Game Config")]
    public bool requireCreditsToPlay = true;

    [Header("Leaderboard")]
    public int entriesPerPage = 20;
    public bool hasHighScoreLeaderboard = true;
    public bool hasTotalScoreLeaderboard = true;
    public string rewardUnit;
    public string totalRewardTextPrefix;
    public string totalRewardSuffix;

    [Header("Delete Account Config")]
    public bool showDeleteSuccessPopUp;
    public bool showDeleteConfirmationPopUp = true;
    public string deleteConfirmationMessageHeader = "Leaving Us?";
    public string deleteConfirmationMessage = "Are you sure you want to delete your account? your details will be removed.";
    public string deleteSuccessMessageHeader;
    public string deleteSuccessMessage;

    [Header("DebugSettings")]
    public bool logResponses;
    public bool logRequest;
}

[Flags]
public enum Environment{
    None = 0,
    Development = 1 << 0,
    Testing= 1 << 1,
    Staging = 1<< 2,
    Production = 1 << 3
}
}
