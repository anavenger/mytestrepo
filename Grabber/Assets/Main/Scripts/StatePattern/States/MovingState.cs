using System.Collections;
using System.Collections.Generic;
using TTP.Controllers;
using UnityEngine;

    public class MovingState : State
    {
        private bool isMoving;
        public MovingState(Grabber grabber, StateMachine stateMachine) 
            : base(grabber, stateMachine) { }
    
        // public override void Enter()
        // {
        //     base.Enter();
        // }

        public override void HandleInput()
        {
            base.HandleInput();
            isMoving = Utils.DetectMoving();
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();
            if (!isMoving)
                _stateMachine.ChangeState(_grabber.standingState);
        }

        public override void Exit()
        {
            _joystick.InputX = _joystick.InputZ = 0f;
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
            _joystick.GetInput();
            _joystick.Moving();
            _grabber.Moving();
        
        }
    }

