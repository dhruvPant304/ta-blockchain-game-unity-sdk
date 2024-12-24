using System;
using Firebase;
using Firebase.Messaging;
using UnityEngine;

namespace TA.Firebase{
public class FirebaseInitializer {
    public Action<object,TokenReceivedEventArgs> onTokenReceived;
    public Action<object,MessageReceivedEventArgs> onMessageRecieved;

    public FirebaseInitializer(){
       FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
            if(task.Result == DependencyStatus.Available){
                Debug.Log("Firebase is ready to use");
                InitializeFCM();
            }else{
                Debug.LogError($"could not resolve firebase dependencies: {task.Result}");
            }
       }); 
    }

    private void InitializeFCM(){
        FirebaseMessaging.TokenReceived += (object sender, TokenReceivedEventArgs args) => {
            onTokenReceived?.Invoke(sender,args);
        };
        FirebaseMessaging.MessageReceived += (object sender, MessageReceivedEventArgs args) => {
            onMessageRecieved?.Invoke(sender, args);
        };
    }
}
}
