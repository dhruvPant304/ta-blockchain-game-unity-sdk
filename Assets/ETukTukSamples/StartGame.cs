using UnityEngine;
using TA.Game;
using TA.Services;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class StartGame : MonoBehaviour {
    GameService gameService;

    void Start(){
        gameService = ServiceLocator.Instance.GetService<GameService>();
        GetComponent<Button>().onClick.AddListener(gameService.StartGameSession);
    }

    void OnDisable(){
        GetComponent<Button>().onClick.RemoveListener(gameService.StartGameSession);
    }
}
