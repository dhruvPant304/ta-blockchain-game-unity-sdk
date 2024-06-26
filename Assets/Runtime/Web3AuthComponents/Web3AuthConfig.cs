using UnityEngine;

namespace TA.Authentication{
[CreateAssetMenu(menuName="TA/Web3AuthConfig", fileName="Web3AuthConfig")]
public class Web3AuthConfig : ScriptableObject{
    public string clientID;
    public string redirecUrl;
    public Web3Auth.Network network;
    public Web3Auth.BuildEnv env;
}
}
