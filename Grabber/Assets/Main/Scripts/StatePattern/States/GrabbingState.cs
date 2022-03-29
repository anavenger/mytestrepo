using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TTP.Controllers;
using UnityEngine;
using TTP.State;
public class GrabbingState : State
{
    private float delay = 2f;
    private float currentTime = 0f;
    
    public GrabbingState(Grabber grabber, StateMachine stateMachine) 
        : base(grabber, stateMachine)
    {
    }

    public override void PhysicsUpdate()
    {
        _grabber.CloseClaw();
        _stateMachine.ChangeState(_grabber.GoingUpState);
    }
}
