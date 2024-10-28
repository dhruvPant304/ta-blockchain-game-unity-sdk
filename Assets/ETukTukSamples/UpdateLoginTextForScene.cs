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

        // Set the initial text to "Login"
        textMesh.text = "Login";

        // If the scene name is "SignIn", perform replacements accordingly
        if (sceneName.Equals("SignIn", StringComparison.OrdinalIgnoreCase))
        {
            // Swap "Login" with "Sign In" and vice versa in a case-insensitive manner
            textMesh.text = Regex.Replace(textMesh.text, @"(?i)login", "Sign In", RegexOptions.IgnoreCase);
            textMesh.text = Regex.Replace(textMesh.text, @"(?i)sign in", "Login", RegexOptions.IgnoreCase);
        }
    }
}
