using UnityEngine;
using TMPro;
using TA.Services;
using TA.APIClient;

namespace TA.TADebug{
    public class InfoPanel : MonoBehaviour {
        [SerializeField] TextMeshProUGUI infoText;

        void Start(){
            var info = $"Env: {Environment()},";
            info += $" BuildVersion: {Application.version}";
            infoText.text = info;
        }

        string Environment(){
            var env = ServiceLocator.Instance.GetService<APIConfigProviderService>().APIConfig.environment;
            switch(env){
                case APIClient.Environment.Development:
                    return "Development";
                case APIClient.Environment.Testing:
                    return "Testing";
                case APIClient.Environment.Staging:
                    return "Staging";
                case APIClient.Environment.Production:
                    return "Production";
            }
            return "";
        }
    }
}

