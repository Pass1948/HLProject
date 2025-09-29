using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MossSandals : RangeItem
{
   protected override void OnEnable()
   {
      base.OnEnable();    
      AddMoveRange(relicItems, 3002);
   }
   private void OnDisable()
   {
      RemoveMoveRange(relicItems, 3002);
   }
}
