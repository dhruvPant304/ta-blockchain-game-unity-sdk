using TA.Components;
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
        try{
            var canvas = ServiceLocator.Instance.GetService<BlockchainGameCanvas>();
            if(canvas != null) Destroy(canvas.gameObject);
            Debug.Log("Deleting blockchain canvas before entring login scene");
        }
        catch{
            //Do nothing
        }
        SceneManager.LoadScene(1);
    }

    void LoadSignInScene(){
         try{
            var canvas = ServiceLocator.Instance.GetService<BlockchainGameCanvas>();
            if(canvas != null) Destroy(canvas.gameObject);
            Debug.Log("Deleting blockchain canvas before entring sign in scene");
         }
        catch{
            //Do nothing
        }
        SceneManager.LoadScene(2);
    }
}
