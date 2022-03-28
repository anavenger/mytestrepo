using System.Collections;
using System.Collections.Generic;
using TTP.Controllers;
using UnityEngine;
using TTP.State;
public class MovingState : State
{
    private bool isMoving;

    public MovingState(Grabber grabber, StateMachine stateMachine) : base(grabber, stateMachine)
    { 
    }

    public override void HandleInput()
    {
        base.HandleInput();
        isMoving = InputUtils.DetectMoving();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        if (!isMoving)
            _stateMachine.ChangeState(_grabber.standingState);
    }

    public override void Exit()
    {
        _joystick.MoveToIdle();
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
        _joystick.Move();
        _grabber.Move();
    }
}

