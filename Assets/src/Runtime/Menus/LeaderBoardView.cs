using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using TA.Leaderboard;
using TA.Services;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static TMPro.TMP_Dropdown;
using TA.APIClient;

namespace TA.Menus{
public class LeaderBoardView : MonoBehaviour {

    [Header("list Pool")]
    [SerializeField] LeaderBoardEntry listEntryPrefab;
    [SerializeField] int poolSize;

    [Header("Content Filds")]
    [SerializeField] LeaderBoardEntry userEntry;
    [SerializeField] TextMeshProUGUI prizepool;

    [Header("Content List View")]
    [SerializeField] Transform contentRoot;
    [SerializeField] ScrollRect scrollView;
    [SerializeField] float loadNewPageScrollThreshold = 0.1f;

    [Header("Selectors")]
    [SerializeField] LeaderBoardTypeSelector leaderBoardTypeSelector;
    [SerializeField] TMP_Dropdown leaderBoardNameDropdown;

    LeaderboardService _leaderBoardService;

    List<LeaderBoardEntry> _pool = new();
    List<LeaderBoardEntry> _active = new();

    Transform poolTransform;
    APIConfig _config;

    LeaderBoard _activeLeaderBoard;
    int _activePage = 0;
    bool _ended = false;

    void Start(){
        _leaderBoardService = ServiceLocator.Instance.GetService<LeaderboardService>();
        _config = ServiceLocator.Instance.GetService<APIConfigProviderService>().APIConfig;

        InitializePool();
        leaderBoardTypeSelector.OnSelection += LoadSelectedLeaderBoard;
        leaderBoardNameDropdown.onValueChanged.AddListener((val) => LoadSelectedLeaderBoard(leaderBoardTypeSelector.Selection));
        scrollView.onValueChanged.AddListener(OnScrollValueChanged);

        Hide();
    }

    async void OnShow(){
        await _leaderBoardService.FetchMaster();

        InitializeOptions();
        SelectActiveLeaderBoard();
        if(_config.hasHighScoreLeaderboard && !_config.hasTotalScoreLeaderboard){
            LoadSelectedLeaderBoard("high");
        }
        else{
            LoadSelectedLeaderBoard("total");
        }
    }

    void InitializeOptions(){
        var options = new List<OptionData>();
        foreach( var name in _leaderBoardService.GetAllLeaderBoardNames()){
            options.Add(new OptionData(){
                        text = name
                    });
        }
        leaderBoardNameDropdown.ClearOptions();
        leaderBoardNameDropdown.AddOptions(options);
    }

    void SelectActiveLeaderBoard(){
        int idx = 0;
        var activeName = _leaderBoardService.GetActiveName();
        foreach(var option in leaderBoardNameDropdown.options){
            if(option.text == activeName){
                break;
            }
            idx++;
        }
        leaderBoardNameDropdown.SetValueWithoutNotify(idx);
    }

    void OnScrollValueChanged(Vector2 position){
        if(position.y <= loadNewPageScrollThreshold){
            LoadNextPageOnActiveLeaderBoard().Forget();
        }
    }

    void AddToPool(LeaderBoardEntry entry){
        _pool.Add(entry);
        entry.gameObject.SetActive(false);
        entry.transform.SetParent(poolTransform);
    }

    void InstantiatePooled(){
        var element = Instantiate(listEntryPrefab);
        AddToPool(element);
    }

    LeaderBoardEntry SpawnFromPool(){
        if(_pool.Count == 0){
            InstantiatePooled();
        }

        var element = _pool[0];
        _pool.Remove(element);
        element.gameObject.SetActive(true);
        element.transform.SetParent(contentRoot);
        element.transform.localScale = listEntryPrefab.transform.localScale;
        _active.Add(element);
        return element;
    }

    void ReturnToPool(LeaderBoardEntry entry){
        entry.gameObject.SetActive(false);
        entry.transform.SetParent(poolTransform);
        _pool.Add(entry);
    } 

    void InitializePool(){
        var pool = new GameObject("List Pool");
        pool.transform.SetParent(transform);
        poolTransform = pool.transform;
        for(int i=0; i<poolSize; i++){
            InstantiatePooled();
        }
    }

    async void LoadSelectedLeaderBoard(string selection){
        var selectedName = leaderBoardNameDropdown.options[leaderBoardNameDropdown.value].text;

        if(_config.hasHighScoreLeaderboard && !_config.hasTotalScoreLeaderboard){
            _activeLeaderBoard = _leaderBoardService.GetHighScoreLeaderBoard(selectedName);
        }
        else if(_config.hasTotalScoreLeaderboard && !_config.hasHighScoreLeaderboard){
            _activeLeaderBoard = _leaderBoardService.GetTotalScoreLeaderBoard(selectedName);
        }
        else if(_config.hasTotalScoreLeaderboard && _config.hasHighScoreLeaderboard){
            if(selection == "high"){
                _activeLeaderBoard = _leaderBoardService.GetHighScoreLeaderBoard(selectedName);
            }
            if (selection == "total"){
                _activeLeaderBoard = _leaderBoardService.GetTotalScoreLeaderBoard(selectedName);
            }
        }
        else{
            return;
        }

        _activePage = 0;
        _ended = false;

        ClearActiveLeaderboard();
        SetTotalPrizePool();
        await SetUserStatsOnActiveLeaderBoard();
        LoadNextPageOnActiveLeaderBoard().Forget();
    }

    void SetTotalPrizePool(){
        if(_activeLeaderBoard == null) return;
        prizepool.text = _config.totalRewardTextPrefix + _activeLeaderBoard.totalPrizePool.ToString() + _config.totalRewardSuffix;
    }

    async UniTask SetUserStatsOnActiveLeaderBoard(){
        if(_activeLeaderBoard == null) return;
        var userEntry = await _activeLeaderBoard.GetUserStats();
        if(userEntry == null){
            this.userEntry.gameObject.SetActive(false);
        }
        else {
            this.userEntry.gameObject.SetActive(true);
            this.userEntry.SetValues(
                    userEntry.rank.ToString(),
                    $"YOU({userEntry.user.username})",
                    userEntry.score.ToString(),
                    userEntry.reward.ToString() + _config.rewardUnit
                );
        }
    }

    bool _loadingNextPage;
    async UniTask LoadNextPageOnActiveLeaderBoard(){
        if(_ended) return;
        if(_activeLeaderBoard == null) return;
        if(_loadingNextPage) return;
        leaderBoardNameDropdown.interactable = false;
        leaderBoardTypeSelector.interactable = false;
        _loadingNextPage = true;

        _activePage += 1;
        var page = await _activeLeaderBoard.GetPage(_activePage);
        if(page.Count == 0){
            _ended = true;
        }

        foreach(var entry in page){
            if(_activeLeaderBoard.userEntry != null){
                if(entry.user.username == _activeLeaderBoard.userEntry.user.username){
                    continue;
                }
            }
            var entryUI = SpawnFromPool();
            entryUI.SetValues(
                    entry.rank.ToString(),
                    entry.user.username,
                    entry.score.ToString(),
                    entry.reward.ToString() + _config.rewardUnit
                );
        }

        leaderBoardNameDropdown.interactable = true;
        leaderBoardTypeSelector.interactable = true;
        _loadingNextPage = false;
    }

    void ClearActiveLeaderboard(){
        foreach (var entryUI in _active){
            ReturnToPool(entryUI);
        }

        _active.Clear();
    }

    public void Show() {
        gameObject.SetActive(true);
        OnShow();
    }

    public void Hide(){
        gameObject.SetActive(false);
    }
}}
