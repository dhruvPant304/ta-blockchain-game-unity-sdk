using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using TA.Leaderboard;
using TA.Services;
using TA.UserProfile;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static TMPro.TMP_Dropdown;

namespace TA.Menus{
public class LeaderBoardView : MonoBehaviour {

    [Header("list Pool")]
    [SerializeField] LeaderBoardEntry listEntryPrefab;
    [SerializeField] LeaderBoardEntry userEntry;
    [SerializeField] int poolSize;

    [Header("Content View")]
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

    LeaderBoard _activeLeaderBoard;
    int _activePage = 0;
    bool _ended = false;

    void Start(){
        _leaderBoardService = ServiceLocator.Instance.GetService<LeaderboardService>();
 
        InitializePool();
        leaderBoardTypeSelector.OnSelection += (s) => LoadSelectedLeaderBoard();
        leaderBoardNameDropdown.onValueChanged.AddListener((val) => LoadSelectedLeaderBoard());
        scrollView.onValueChanged.AddListener(OnScrollValueChanged);

        Hide();
    }

    async void OnShow(){
        await _leaderBoardService.FetchMaster();

        InitializeOptions();
        SelectActiveLeaderBoard();
        LoadSelectedLeaderBoard();

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

    void LoadSelectedLeaderBoard(){
        var selectedName = leaderBoardNameDropdown.options[leaderBoardNameDropdown.value].text;
        if(leaderBoardTypeSelector.Selection == "high"){
           _activeLeaderBoard = _leaderBoardService.GetHighScoreLeaderBoard(selectedName);
        }
        if (leaderBoardTypeSelector.Selection == "total"){
            _activeLeaderBoard = _leaderBoardService.GetTotalScoreLeaderBoard(selectedName);
        }

        _activePage = 0;
        _ended = false;

        ClearActiveLeaderboard();
        SetUserStatsOnActiveLeaderBoard().Forget();
        LoadNextPageOnActiveLeaderBoard().Forget();
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
                    userEntry.reward.ToString()
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
            var entryUI = SpawnFromPool();
            entryUI.SetValues(
                    entry.rank.ToString(),
                    entry.user.username,
                    entry.score.ToString(),
                    entry.reward.ToString()
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
