using TA.APIClient.ResponseData;
using TA.Components;
using TA.Services;
using TA.UserProfile;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;

public class LoadHomePageOnLogin : MonoBehaviour{

    UserProfileService _userProfileService;
    CanvasGroupFader _canvasGroupFader;

    public void Start(){
        _userProfileService = ServiceLocator.Instance.GetService<UserProfileService>();
        _canvasGroupFader = ServiceLocator.Instance.GetService<CanvasGroupFader>();
        _userProfileService.OnAuthSuccess += OnAuthSuccess;
    }

    void OnDisable(){
        _userProfileService.OnAuthSuccess -= OnAuthSuccess;
    }

    void OnAuthSuccess(LoginSessionData userData){
        // Debug.Log("Login successful loading home page...");
        // SceneManager.LoadScene("HomePage");
        // _canvasGroupFader.FadeIn().Forget();
    }
}
