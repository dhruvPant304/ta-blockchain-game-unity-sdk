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
    [SerializeField] LeaderBoardEntry userEntryPrefab;
    [SerializeField] int poolSize;

    [Header("Content View")]
    [SerializeField] Transform contentRoot;
    [SerializeField] ScrollRect scrollView;
    [SerializeField] float loadNewPageScrollThreshold = 0.1f;

    [Header("Selectors")]
    [SerializeField] LeaderBoardTypeSelector leaderBoardTypeSelector;
    [SerializeField] TMP_Dropdown leaderBoardNameDropdown;

    UserProfileService _userProfileService;
    LeaderboardService _leaderBoardService;

    List<LeaderBoardEntry> _pool = new();
    List<LeaderBoardEntry> _active = new();

    Transform poolTransform;

    LeaderBoard _activeLeaderBoard;
    int _activePage = 0;
    bool _ended = false;

    void Start(){
        _userProfileService = ServiceLocator.Instance.GetService<UserProfileService>();
        _leaderBoardService = ServiceLocator.Instance.GetService<LeaderboardService>();
 
        InitializePool();
        leaderBoardTypeSelector.OnSelection += (s) => LoadSelectedLeaderBoard();
        leaderBoardNameDropdown.onValueChanged.AddListener((val) => LoadSelectedLeaderBoard());
        scrollView.onValueChanged.AddListener(OnScrollValueChanged);

        Hide();
    }

    async void OnShow(){
        await _leaderBoardService.FetchMaster();
        var activeName = _leaderBoardService.GetActiveName();

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

        ClearActiveLeaderboard();
        LoadNextPageOnActiveLeaderBoard().Forget();
    }

    async UniTask LoadNextPageOnActiveLeaderBoard(){
        if(_ended) return;
        if(_activeLeaderBoard == null) return;
        leaderBoardNameDropdown.interactable = false;
        leaderBoardTypeSelector.interactable = false;

        var page = await _activeLeaderBoard.GetPage(_activePage + 1);
        _activePage += 1;
        if(page.Count == 0){
            _ended = true;
            return;
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
