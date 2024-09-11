using TA.Services;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TA.UserProfile;
using TA.Components;

public class LandingPage : MonoBehaviour
{
    [SerializeField] Button signInButton;
    [SerializeField] Button loginInButton;
    [SerializeField] CanvasGroupFader fader;

    // Start is called before the first frame update
    void Start(){
        signInButton.onClick.AddListener(LoadSignInScene);
        loginInButton.onClick.AddListener(LoadLoginScene);

        if(!UserProfileService.HasSavedLoginSession()){
            fader.FadeIn();
        }

        if(SceneManager.GetActiveScene().name != "LandingPage") {
            return;
        }
        //Skip rest of the code, if not landing page

        if(UserProfileService.HasSavedLoginSession()){
            SceneManager.LoadScene(1); //Loads Login scene if has a saved login session
            return;
        }

        try{
            ServiceLocator.Instance.CloseServices();
        }
        catch{

        }
    }

    // Update is called once per frame
    void Update(){

    }

    void LoadLoginScene(){
        if(ServiceLocator.Instance != null) ServiceLocator.Instance.CloseServices();
        SceneManager.LoadScene(1);
    }

    void LoadSignInScene(){
        if(ServiceLocator.Instance != null) ServiceLocator.Instance.CloseServices(); 
        SceneManager.LoadScene(2);
    }
}
