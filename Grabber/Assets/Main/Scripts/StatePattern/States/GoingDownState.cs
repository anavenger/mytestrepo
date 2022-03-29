using System.Collections;
using System.Collections.Generic;
using TTP.Controllers;
using TTP.State;
using UnityEngine;

public class GoingDownState : State
{
    private bool _isDown = false;
    
    public GoingDownState(Grabber grabber, StateMachine stateMachine) : base(grabber, stateMachine)
    { }
    
    public override void PhysicsUpdate()
    {
        _isDown = _grabber.GoDown();
        
        if (_isDown)
        {
            _stateMachine.ChangeState(_grabber.GrabbingState);
        }
    }
}
