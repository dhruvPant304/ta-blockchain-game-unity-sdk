#if ENABLE_FIREBASE_MESSAGING
using Firebase.Messaging;
using UnityEngine;
using TA.APIClient;
using TA.Services;
using TA.UserProfile;
using TA.APIClient.RequestData;
using Cysharp.Threading.Tasks;
using UnityEngine.Android;
using TA.APIClient.ResponseData;
using TA.Authentication;

namespace TA.Firebase{
    public class FirebaseService : Service<FirebaseService>{
        public string FCMToken {get; private set;}
        APIService _api;
        UserProfileService _profile;
        Web3AuthService _web3;

        void Start(){
            _api = ServiceLocator.Instance.GetService<APIService>();
            _profile = ServiceLocator.Instance.GetService<UserProfileService>();

            var firebaseMessaging = new FirebaseInitializer();
            firebaseMessaging.onTokenReceived += OnToken; 
        }

        void OnToken(object sender, TokenReceivedEventArgs args){
            FCMToken = args.Token;
            Debug.Log($"FCM Token received: {FCMToken}");
            _profile.OnAuthSuccess += OnLogin;
            _web3.OnLogout += OnLogout;
        }

        void OnLogin(LoginSessionData data){
            string platform; 
            RequestNotificationPremissions();
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

           _api.SendUpdateDeviceTokenRequest(_profile.LoginToken, deviceToken).Forget(); 
        }

        void OnLogout(){
            _api.SendDeleteDevice(_profile.LoginToken).Forget();
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
#endif
