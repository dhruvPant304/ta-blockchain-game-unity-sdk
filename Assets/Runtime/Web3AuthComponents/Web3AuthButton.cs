using TA.Services;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TA.Authentication{
[RequireComponent(typeof(Button))]
public class Web3AuthButton : MonoBehaviour{
    [SerializeField] Provider loginProvider;

    [Tooltip("Email input field only required in case of EMAIL_PASSWORD and EMAIL_PASSWORDLESS option and can be left empty for other login providers")]
    [SerializeField] TMP_InputField emailInputField;

    Web3AuthService web3AuthService;

    public void Start(){
        web3AuthService = ServiceLocator.Instance.GetService<Web3AuthService>();
        GetComponent<Button>().onClick.AddListener(Login);
    }

    void Login(){
        var loginParams = new LoginParams(){
            loginProvider = loginProvider,
            extraLoginOptions = GetExtraLoginOptions(),
        };
        web3AuthService.Login(loginParams);
    }

    public ExtraLoginOptions GetExtraLoginOptions(){
        if(emailInputField == null) return null;
        return new ExtraLoginOptions(){
            login_hint = emailInputField.text
        };
    }
}
}
