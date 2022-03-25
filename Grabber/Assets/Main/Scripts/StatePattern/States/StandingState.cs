using System.Collections;
using System.Collections.Generic;
using TTP.Controllers;
using UnityEngine;

public class StandingState : State
    {
        private bool isMoving;

        public StandingState(Grabber grabber, StateMachine stateMachine)
            : base(grabber, stateMachine)
        {
        }

        public override void Enter()
        {
            base.Enter();
            _joystick.InputX = _joystick.InputZ = 0f;
            _joystick.Moving();
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

