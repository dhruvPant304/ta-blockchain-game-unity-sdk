using System;
using UnityEngine;

namespace TA.Authentication{
public class Web3AuthConfigProviderService: Service<Web3AuthConfigProviderService> { 

    Web3AuthConfig _config;
    Web3AuthOptions _options;
    private string web3AuthConfigPath = "Web3AuthConfig";

    public Web3AuthConfig Config => _config;
    public Web3AuthOptions SelectedOptions => _options; 

    protected override void OnInitialize(){
        try{
            _config = Resources.Load<Web3AuthConfig>(web3AuthConfigPath);

        }catch(Exception e){
            throw new Exception($"Failed to load Web3Auth config: {e}");
        }

        _options = CreateOptionsFromConfig(_config);
    }

    Web3AuthOptions CreateOptionsFromConfig(Web3AuthConfig config){
        var options = new Web3AuthOptions(){
            redirectUrl = new Uri(config.redirecUrl),
            clientId = config.clientID,
            network = config.network,
            buildEnv = config.env,
        };
        return options;
    }
}
}
