using TA.Services;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LandingPage : MonoBehaviour
{
    [SerializeField] Button signInButton;
    [SerializeField] Button loginInButton;
    // Start is called before the first frame update
    void Start(){
       signInButton.onClick.AddListener(LoadSignInScene);
       loginInButton.onClick.AddListener(LoadLoginScene);
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
