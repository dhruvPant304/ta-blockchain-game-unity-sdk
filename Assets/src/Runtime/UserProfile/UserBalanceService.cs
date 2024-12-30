using TA.Services;
using TA.APIClient.ResponseData;
using System;
using Cysharp.Threading.Tasks;
using TA.APIClient;
using System.Collections.Generic;

namespace TA.UserProfile.Balance{
    public class UserBalanceService : Service<UserBalanceService> {
        private UserBalanceData _syncedBalance;
        private Dictionary<string, int> _chachedTransactions = new();

        public Action<UserBalanceData> OnBalanceSync;
        public Action OnBalanceSyncFailed;

        public Action<string> OnCurrencyBalanceUpdate; 

        private APIService _api;
        private UserProfileService _profile;

        void Start(){
            _api = ServiceLocator.Instance.GetService<APIService>();
            _profile = ServiceLocator.Instance.GetService<UserProfileService>();
        }

        public int GetBalanceInt(string type) {
            try{
                if(!_chachedTransactions.ContainsKey(type)){
                    return (int)_syncedBalance.GetFild<int>(type);
                }
                return (int)_syncedBalance.GetFild<int>(type) + _chachedTransactions[type] ;
            }
            catch (Exception e){
                throw new Exception($"Failed to get balance for \"{type}\": {e}");
            }
        }

        public void Spend(string currencyName, int value){
            if(!_chachedTransactions.ContainsKey(currencyName)){
                _chachedTransactions[currencyName] = 0;
            } 
            _chachedTransactions[currencyName] -= value;
            OnCurrencyBalanceUpdate?.Invoke(currencyName);
        }

        public UserBalanceData LastSyncedBalance(){
            return _syncedBalance;
        }

        public async UniTask<StaticRequestResponse<BaseAPIResponse>> UpdateUserBalance(){
            var response = await _api.SendFetchUserBalanceRequest(_profile.LoginUserData.token);
            _chachedTransactions = new();
            if(response.IsSuccess){
                _syncedBalance = response.Response.data;
                OnBalanceSync?.Invoke(response.Response.data);
            } else{
                OnBalanceSyncFailed?.Invoke();
            }
            return APIService.CreateBaseResponse(response.IsSuccess, response.Response.message);
        }

    }
}
