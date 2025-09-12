using System;
using UnityEngine;

public enum Suit { Spade, Heart, Diamond, Club }
public enum Rank { A = 1, Two, Three, Four, Five, Six, Seven, Eight, Nine, Ten, J = 11, Q = 12, K = 13 }

//탄환 데이터
[Serializable]
public class Ammo
{
    public Suit suit;
    public Rank rank;
    public string Id => $"{suit}_{rank}";
}

