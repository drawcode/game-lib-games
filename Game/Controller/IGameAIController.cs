public interface IGameAIController {
    void Awake();
    void directAI();
    void handlePeriodic();
    void handleUpdate();
    void init();
    void load(string code, string type = "enemy");
    void preload();
    void Start();
    void Update();
}