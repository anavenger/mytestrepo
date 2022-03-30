using System.Collections;
using System.Collections.Generic;
using TTP.Controllers;
using TTP.State;
using UnityEngine;

public class GoingUpState : State
{
    private bool _isUp = false;
    
    public GoingUpState(Grabber grabber, StateMachine stateMachine) 
        : base(grabber, stateMachine)
    {
    }

    public override void PhysicsUpdate()
    {
        _isUp = _grabber.GoUp();
        
        if (_isUp)
            _stateMachine.ChangeState(_grabber.GoingBackState);
    }
}
