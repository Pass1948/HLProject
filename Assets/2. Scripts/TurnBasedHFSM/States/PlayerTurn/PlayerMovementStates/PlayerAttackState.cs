using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackState : PlayerActionState
{
    class A_Windup : PlayerActionState
    {
       
        public override void OnEnter()
        {
        }
        public override void Tick(float dt)
        {
        
        }
    }

    class A_Execute : PlayerActionState
    {
      
        public override void OnEnter()
        {
         
        }
        public override void Tick(float dt)
        {
      
        }
    }

    class A_Recover : PlayerActionState
    {
  
        public override void OnEnter()
        {
        
        }
        public override void Tick(float dt)
        {
         
        }
    }
}
