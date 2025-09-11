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

}

public static class Prefab
{

}

public static class MonsterAnimParam
{

}