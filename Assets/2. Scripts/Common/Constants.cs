public enum SceneType
{
    Test,
    Test2
}

public enum EventType    // �̺�Ʈ ����� �߰�
{

    // Command ���������� �̺�Ʈ(�ʿ��� �͸� �ּ� �߰�)
    CommandBuffered,        // Ŀ�ǵ尡 �÷��� ���ۿ� �����
    CommandCommitted,       // ���� �� ť(�Ǵ� ��ũ��)�� Ŀ�Ե�
    CommandExecuted,        // ť���� ���� �����(�� ���� ���� �Ϸ�)
    CommandUndone           // ���� ��� �Ǵ� ���� �� Undo �����
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
    public const string Player = Prefab + "Player/";
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

public static class TileID
{
    public const int Terrain = 0;
    public const int Wall = 1;
    public const int Player = 2;
    public const int Motor = 3;
    public const int Obstacle = 4;
    public const int Enemy = 5;
}