using System;
using GoogleSheet.Core.Type;

public enum SceneType
{
    Title,
    Test,
}

public enum EventType
{
    PlayerAction,
    PlayerAttack,
    PlayerKick,
    PlayerMove, 
    EnemyTurnStart,
    EnemyTurnEnd,
    EnemyHit,
    VehicleOnMounted,
    VehicleOnDismounted,
    VehicleOnRepaired,
    VehicleOnDestroyed,
    OnGoldChanged,
}
[UGS(typeof(RarityType))]
public enum RarityType // 아이템 레어도 타입
{
    Common,
    Normal,
    Rare,
    Elite,
    Legendary,
}
[UGS(typeof(ItemType))]
public enum ItemType   // 유물, 화약 구분
{
    Relic,
    Artifact,
    GunPowder,
}


public enum ShopItemType
{
    Bullet,
    SpecialTotem,
    PowderBundle,
    RemoveBullet,
    Heal,
    Reroll,
}

public enum Rare
{
    Nomal,
    Rare,
    Unique,
    Legendary,
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
    public const string Weapon = Prefab + "Weapons/";
    public const string Mouse = Prefab + "Mouse/";
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
    public const int Vehicle = 3;
    public const int Obstacle = 4;
    public const int Enemy = 5;
}

