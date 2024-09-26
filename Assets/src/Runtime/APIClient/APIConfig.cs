using UnityEngine;

namespace TA.APIClient{
[CreateAssetMenu(menuName="TA/APIService", fileName="APIConfig")]
public class APIConfig : ScriptableObject {

    [Header("Env")]
    public string serverUrl;

    [Header("Keys")]
    public string encryptionPublicKey;
    public string gameId;

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

    [Header("DebugSettings")]
    public bool logResponses;
    public bool logRequest;
}
}
