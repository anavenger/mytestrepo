using System.Collections;
using System.Collections.Generic;
using TTP.Controllers;
using UnityEngine;
using TTP.State;
public class StandingState : State
    {
        private bool isMoving;
        private bool buttonClicked;

        public StandingState(Grabber grabber, StateMachine stateMachine)
            : base(grabber, stateMachine)
        {
        }

        public override void Enter()
        {
            base.Enter();
        }

        public override void HandleInput()
        {
            base.HandleInput();
            isMoving = InputUtils.DetectMoving();
            buttonClicked = InputUtils.DetectButtonClick();
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();
            if (isMoving)
            {
                _stateMachine.ChangeState(_grabber.movingState);
            }             
            else if (buttonClicked)
            {
                _stateMachine.ChangeState(_grabber.grabbingState);
            }
        }
    }

