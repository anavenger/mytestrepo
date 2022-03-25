using System.Collections;
using System.Collections.Generic;
using TTP.Controllers;
using UnityEngine;
using UnityEngine.InputSystem;
using TTP.UserInput;
using Joystick = TTP.UserInput.Joystick;

    public class IdleState : State
    {
        private bool isMoving;

        public IdleState(Grabber grabber, StateMachine stateMachine)
            : base(grabber, stateMachine)
        {
        }

        public override void Enter()
        {
            base.Enter();
           // _joystick.InputX = _joystick.InputZ = 0f;
        }

        public override void HandleInput()
        {
            base.HandleInput();
            isMoving = Utils.DetectMoving();
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();
            if (isMoving)
                _stateMachine.ChangeState(_grabber.movingState);
        }
    }
