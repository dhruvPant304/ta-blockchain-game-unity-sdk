using TA.Services;
using TA.APIClient;
using UnityEngine;

namespace TA.TADebug{
public class IncludeInEnvironments : MonoBehaviour
{
    [SerializeField] APIClient.Environment environment;

    public void Start(){
        var env = ServiceLocator.Instance.GetService<APIConfigProviderService>().APIConfig.environment;
        if(environment.HasFlag(env)) return;
        gameObject.SetActive(false);
    }
}
}
