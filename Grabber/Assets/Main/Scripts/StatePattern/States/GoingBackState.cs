using System.Collections;
using System.Collections.Generic;
using TTP.Controllers;
using TTP.State;
using UnityEngine;

public class GoingBackState : State
{
    public GoingBackState(Grabber grabber, StateMachine stateMachine) 
        : base(grabber, stateMachine)
    {
    }
    
    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
        _grabber.MoveBack();
    }
}
