using UnityEngine;

public class IncludeInPlatforms : MonoBehaviour{
    [SerializeField] bool android;
    [SerializeField] bool iOS;

    public void Awake(){
        #if UNITY_ANDROID
            if(!android) gameObject.SetActive(false);
        #endif

        #if UNITY_IOS
            if(!iOS) gameObject.SetActive(false);
        #endif
    }
}
