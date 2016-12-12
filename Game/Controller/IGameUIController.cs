using UnityEngine;

public interface IGameUIController {
    void Awake();
    void HandleInGameAudio();
    void HandleInGameAudio(bool play);
    void handleNetworkedButtons(string buttonName);
    void handleTouchLaunch(Vector2 move);
    void hideCustomSafety();
    void hideCustomSmarts();
    void hideGameMode();
    void hideMain();
    void LoadData();
    bool NavigateBack(string buttonName);
    void OnButtonClickEventHandler(string buttonName);
    void OnDisable();
    void OnEnable();
    void showCustomSafety();
    void showCustomSmarts();
    void showGameMode();
    void showMain();
    void showProducts(string productType);
    void showUITip(string title, string description);
    void Start();
    void ToggleUI();
    void Update();
}