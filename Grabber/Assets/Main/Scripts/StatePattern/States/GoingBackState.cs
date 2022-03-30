using System.Collections;
using System.Collections.Generic;
using TTP.Controllers;
using TTP.State;
using UnityEngine;

public class GoingBackState : State
{
    private bool _isBack;
    public GoingBackState(Grabber grabber, StateMachine stateMachine) 
        : base(grabber, stateMachine)
    {
    }
    
    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
        _isBack = _grabber.MoveBack();
        if(_isBack)
            _stateMachine.ChangeState(_grabber.OpeningClawState);
    }

    public override void Exit()
    {
        base.Exit();
        _isBack = false;
        _grabber.ResetBackMove();
    }
}
