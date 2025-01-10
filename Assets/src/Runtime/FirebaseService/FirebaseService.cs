#if ENABLE_FIREBASE_MESSAGING
using Firebase.Messaging;
using UnityEngine;
using TA.APIClient;
using TA.UserProfile;
using TA.APIClient.RequestData;
using Cysharp.Threading.Tasks;
using UnityEngine.Android;
using Unity.Notifications.iOS;
using TA.APIClient.ResponseData;
using TA.Authentication;
#endif
using TA.Services;

namespace TA.Firebase{
    public class FirebaseService : Service<FirebaseService>{
#if ENABLE_FIREBASE_MESSAGING
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
            if(FCMToken != null){
                Debug.Log($"FCM Token received: {FCMToken}");
                _profile.OnAuthSuccess += SetupNotifcations;
                _web3.OnLogout += RemoveDeviceToken;
            }
        }

        void SetupNotifcations(LoginSessionData data){
            string platform; 
            RequestNotificationPremissions().Forget();
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

        void RemoveDeviceToken(){
            _api.SendDeleteDevice(_profile.LoginToken).Forget();
        }

        async UniTask RequestNotificationPremissions(){
             if (Application.platform == RuntimePlatform.Android){
                if (!Permission.HasUserAuthorizedPermission("android.permission.POST_NOTIFICATIONS")){
                    Permission.RequestUserPermission("android.permission.POST_NOTIFICATIONS");
                }
            }else if (Application.platform == RuntimePlatform.IPhonePlayer){
                await RequestNotificationPremissions();
            }
        }

        async UniTask RequestIosAuthorization(){
            var authorizationOption = AuthorizationOption.Alert 
                | AuthorizationOption.Badge 
                | AuthorizationOption.Sound;

            var req = new AuthorizationRequest(authorizationOption, true);
            await UniTask.WaitUntil(() => req.IsFinished);
            Debug.Log("IOS notification permission granted");
        }
#endif
    }
}
