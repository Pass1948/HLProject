using System;
using UnityEngine;
public enum Suit { Spade, Heart, Diamond, Club }

//탄환 데이터
[Serializable]
public class Ammo
{
    public Suit suit;

    //1~13까지 범위지정
    [Range(1, 13)]
    //1=A, 11=J, 12=Q, 13=K
    public int rank; 
}

