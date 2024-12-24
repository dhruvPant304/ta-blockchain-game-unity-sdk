using Firebase.Messaging;
using UnityEngine;
using TA.APIClient;
using TA.Services;
using TA.UserProfile;
using TA.APIClient.RequestData;
using Cysharp.Threading.Tasks;
using UnityEngine.Android;

namespace TA.Firebase{
    public class FirebaseService : Service<FirebaseService>{
        public string FCMToken {get; private set;}
        APIService _api;
        UserProfileService _profile;

        void Start(){
            _api = ServiceLocator.Instance.GetService<APIService>();
            _profile = ServiceLocator.Instance.GetService<UserProfileService>();

            var firebaseMessaging = new FirebaseInitializer();
            firebaseMessaging.onTokenReceived += OnToken; 
        }

        async void OnToken(object sender, TokenReceivedEventArgs args){
            FCMToken = args.Token;
            Debug.Log($"FCM Token received: {FCMToken}");

            string platform; 

            switch(Application.platform){
                case RuntimePlatform.Android:
                    platform = "android";
                    break;
                case RuntimePlatform.IPhonePlayer:
                    platform = "ios";
                    break;
                default:
                    platform = "unidentified";
                    break;
            }

            var deviceToken = new DeviceId{
                deviceType = platform,
                deviceId = SystemInfo.deviceUniqueIdentifier,
                token = FCMToken
            };

            await UniTask.WaitUntil(() => _profile.LoggedIn);
            RequestNotificationPremissions();
           _api.SendUpdateDeviceTokenRequest(_profile.LoginToken, deviceToken).Forget(); 
        }

        void RequestNotificationPremissions(){
             if (Application.platform == RuntimePlatform.Android){
                if (!Permission.HasUserAuthorizedPermission("android.permission.POST_NOTIFICATIONS")){
                    Permission.RequestUserPermission("android.permission.POST_NOTIFICATIONS");
                }
            }
        }
    }
}
