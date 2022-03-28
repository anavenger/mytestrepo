using System.Collections;
using System.Collections.Generic;
using TTP.Controllers;
using UnityEngine;
using Joystick = TTP.UserInput.Joystick;
using Button = TTP.UserInput.Button;

namespace TTP.State
{
    public abstract class State
    {
        protected Grabber _grabber;
        protected StateMachine _stateMachine;
        protected Joystick _joystick;
        protected Button _button;
        public State(Grabber grabber, StateMachine stateMachine)
        {
            _grabber = grabber;
            _stateMachine = stateMachine;
            _joystick = grabber.joystick;
            _button = grabber.button;
        }

        protected void DisplayCurrentState()
        {
            Debug.LogWarning($"CurrentState = {this}");
        }

        public virtual void Enter()
        {
            DisplayCurrentState();
        }

        public virtual void HandleInput()
        {

        }

        public virtual void LogicUpdate()
        {

        }

        public virtual void PhysicsUpdate()
        {

        }

        public virtual void Exit()
        {

        }
    }
}