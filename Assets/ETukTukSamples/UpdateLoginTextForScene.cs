using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UpdateLoginTextForScene : MonoBehaviour{
    [SerializeField] TextMeshProUGUI textMesh;

    void OnEnable(){
        var sceneName = SceneManager.GetActiveScene().name;

        textMesh.text = "Login";
        if(sceneName == "SignIn"){
            textMesh.text = "Sign In"; 
        }
    }
}
