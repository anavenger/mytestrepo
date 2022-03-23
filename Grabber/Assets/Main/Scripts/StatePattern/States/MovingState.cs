using System.Collections;
using System.Collections.Generic;
using TTP.Controllers;
using UnityEngine;

public class MovingState : State
{
    public MovingState(Grabber grabber, StateMachine stateMachine) 
        : base(grabber, stateMachine) { }
    
    // public override void Enter()
    // {
    //     base.Enter();
    // }

    public override void HandleInput()
    {
        base.HandleInput();
        _joystick.GetInput();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        if (!_joystick.HasInput)
        {
            _stateMachine.ChangeState(_grabber.standingState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
        _joystick.Moving();
        _grabber.Moving();
        
    }
}
