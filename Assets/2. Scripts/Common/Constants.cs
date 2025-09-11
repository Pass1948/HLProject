public enum SceneType
{
    Test,
}

public enum EventType
{
    // 이벤트 생길시 추가

    // Command 파이프라인 이벤트(필요한 것만 최소 추가)
    CommandBuffered,        // 커맨드가 플래닝 버퍼에 적재됨
    CommandCommitted,       // 버퍼 → 큐(또는 매크로)로 커밋됨
    CommandExecuted,        // 큐에서 실제 실행됨(모델 상태 변경 완료)
    CommandUndone           // 버퍼 취소 또는 실행 후 Undo 수행됨
}

public static class Path
{
    public const string Prefab = "Prefab/";
    public const string UI = Prefab + "UI/";
    public const string Character = Prefab + "Character/";
    public const string Enemy = Prefab + "Enemy/";
    public const string Item = Prefab + "Item/";
    public const string Map = Prefab + "Map/";
    public const string Camera = Prefab + "Camera/";
    public const string Data = "Data/";
    public const string Sound = "Sound/";
}

public static class Prefab
{
    // Character
    public const string Player = "Player";
    public const string Enemy = "Enemy";

    // camera
    public const string VirtualCamera = "VirtualCamera";

    // Map
    public const string Stage = "Stage";
    public const string Town = "Town";

    // UI
    public const string Canvas = "Canvas";
    public const string EventSystem = "EventSystem";
}

public static class PrefKey
{
    public const string Score = "Score";

}