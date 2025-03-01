using System;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UpdateLoginTextForScene : MonoBehaviour{
    [SerializeField] TextMeshProUGUI textMesh;

    void OnEnable()
    {
        var sceneName = SceneManager.GetActiveScene().name;

        // If the scene name is "SignIn", perform replacements accordingly
        if (sceneName.Equals("SignIn", StringComparison.OrdinalIgnoreCase))
        {
            // Swap "Login" with "Sign In" and vice versa in a case-insensitive manner
            var replacement = Regex.Replace(textMesh.text, @"(?i)login", "Sign In", RegexOptions.IgnoreCase);
            if(replacement != textMesh.text){
                textMesh.text = replacement;
                return;
            }
            textMesh.text = Regex.Replace(textMesh.text, @"(?i)sign in", "Login", RegexOptions.IgnoreCase);
        }
    }
}
