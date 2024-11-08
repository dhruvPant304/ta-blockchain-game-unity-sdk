using UnityEngine;
using TMPro;
using TA.Services;
using TA.APIClient;
using TA.Authentication;
using System.Collections.Generic;
using UnityEngine.UI;

namespace TA.TADebug{
    public class InfoPanel : MonoBehaviour {
        [SerializeField] TextMeshProUGUI infoText;
        [SerializeField] Button copyButton;

        void Start(){
            infoText.overflowMode = TextOverflowModes.ScrollRect;

            var apiConfig = ServiceLocator.Instance.GetService<APIConfigProviderService>().APIConfig;
            var web3AuthConfig = ServiceLocator.Instance.GetService<Web3AuthConfigProviderService>().Config;

            var infoDict = new Dictionary<string,string>(){
                {"Env", apiConfig.environment.ToString()},
                {"BuildVersion", Application.version},
                {"API Url", apiConfig.serverUrl},
                {"Web3Auth ClientId", web3AuthConfig.clientID},
                {"Web3Auth Network", web3AuthConfig.network.ToString()}
            };

            var info = "";
            foreach(var pair in infoDict){
                info += $"{pair.Key} : {pair.Value}, ";
            }

            infoText.text = info;

            copyButton.onClick.AddListener(() => GUIUtility.systemCopyBuffer = info);
        }

    }
}

