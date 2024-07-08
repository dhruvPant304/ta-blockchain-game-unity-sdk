using System;
using System.Text.RegularExpressions;
using System.Globalization;
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
    [SerializeField] TextMeshProUGUI inValidEmailText;

    Web3AuthService web3AuthService;

    public void Start(){
        web3AuthService = ServiceLocator.Instance.GetService<Web3AuthService>();
        GetComponent<Button>().onClick.AddListener(Login);

        if(emailInputField != null) emailInputField.onValueChanged.AddListener((s) => HideValidationText());
    }

    void Login(){
        if(!EmailValidation()) return;

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

    bool EmailValidation(){
        if(emailInputField == null) return true;
        var valid = IsValidEmail(emailInputField.text);
        if(!valid){
            inValidEmailText.gameObject.SetActive(true);
            inValidEmailText.text = "email address is invalid";
        }
        else{
            HideValidationText();
        }

        return valid;
    }

    void HideValidationText(){
        inValidEmailText.gameObject.SetActive(false);
    }

    public static bool IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        try
        {
            // Use IdnMapping class to convert Unicode domain names.
            email = Regex.Replace(email, @"(@)(.+)$", DomainMapper, RegexOptions.None, TimeSpan.FromMilliseconds(200));

            // Return true if email is in valid e-mail format.
            return Regex.IsMatch(email,
                @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
                RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
        }
        catch (RegexMatchTimeoutException)
        {
            return false;
        }
        catch (ArgumentException)
        {
            return false;
        }
    }

    private static string DomainMapper(Match match)
    {
        // Use IdnMapping class to convert Unicode domain names.
        var idn = new IdnMapping();

        string domainName = idn.GetAscii(match.Groups[2].Value);
        return match.Groups[1].Value + domainName;
    }
}
}
