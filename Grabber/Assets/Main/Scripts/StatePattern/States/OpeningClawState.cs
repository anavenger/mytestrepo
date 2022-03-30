using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TTP.Controllers;
using TTP.State;

public class OpeningClawState : State
{
    public OpeningClawState(Grabber grabber, StateMachine stateMachine) : base(grabber, stateMachine)
    {
    }
    
    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
        _grabber.OpenClaw();
        _stateMachine.ChangeState(_grabber.IdleState);
    }
}
