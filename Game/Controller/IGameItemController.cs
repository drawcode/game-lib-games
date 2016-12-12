public interface IGameItemController {
    void direct();
    void handlePeriodic();
    void handleUpdate();
    void init();
    void load(string code);
    void preload();
    void Start();
    void Update();
}