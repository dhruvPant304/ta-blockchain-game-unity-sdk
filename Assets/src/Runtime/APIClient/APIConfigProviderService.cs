using TA.Services;
using UnityEngine;

namespace TA.APIClient{
public class APIConfigProviderService : Service<APIConfigProviderService> {
    APIConfig apiConfig;
    public APIConfig APIConfig => apiConfig;

    string configPath = "APIConfig";

    protected override void OnInitialize(){
        apiConfig = Resources.Load<APIConfig>(configPath);
    }
}
}
