using UnityEngine;

namespace TA.APIClient{
[CreateAssetMenu(menuName="TA/APIService", fileName="APIConfig")]
public class APIConfig : ScriptableObject {
    public string serverUrl;
    public string encryptionPublicKey;

    [Header("DebugSettings")]
    public bool logResponses;
    public bool logRequest;
}
}
