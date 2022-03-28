using System.Collections;
using System.Collections.Generic;
using TTP.Controllers;
using UnityEngine;
using TTP.State;
public class GrabbingState : State
{
    bool isDown = false;
    public GrabbingState(Grabber grabber, StateMachine stateMachine) 
        : base(grabber, stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void LogicUpdate()
    {

    }

    public override void PhysicsUpdate()
    {
        isDown = _grabber.GoDown();
        
        if (isDown)
        {
            _grabber.CloseClaw();
        }

        _grabber.GoUp();
    }

    public override void Exit()
    {

    }

}
