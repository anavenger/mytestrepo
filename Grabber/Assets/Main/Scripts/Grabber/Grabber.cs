using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime;
using TTP.Toys;
using UnityEngine;
using TTP.Utilities;
using TTP.UserInput;
using UnityEngine.Serialization;

namespace TTP.Utilities
{ 
[System.Serializable]
    public struct STupple
    {
        public float _left;

        public float _right;
        private STupple((float left, float right) pair)
        {
            _left = pair.left;
            _right = pair.right;
        }

        public static implicit operator STupple((float left, float right) pair)
        {
            return new STupple(pair);
        }
    }
}
namespace TTP.Controllers
{
    public enum GrabberState
    {
        None,
        StartGrabbing,
        EndGrabbing
    }

    public enum ClawState
    {
        Opened,
        Closed
    }
    
    public class Grabber : MonoBehaviour
    {
        [SerializeField] private ScoreController scoreController;
        [SerializeField] private ObjectRetentionCheck objectRetentionCheck;
        
        public Joystick joystick;
        [SerializeField] private ButtonController buttonController;
        
        [SerializeField] private STupple x_Ancors;
        [SerializeField] private STupple z_Ancors;

        [SerializeField] private float moveSpeed = 0.5f;
        [SerializeField] private float downSpeed;
        
        private Rigidbody _movingPlatformRigidbody;
        [SerializeField] private Rigidbody pistonRigidbody;

        [Header("Piston Anchor")]
        [SerializeField] private float closeClawAnchorY = 0.35f;
        private Vector3 _pistonConnectedAnchor;
        private float _openClawAnchorY;
        private ConfigurableJoint _pistonConfigurableJoint;

        [Header("MovingPlatform Anchor")]
        [SerializeField] private float downAnchorY = 8.5f;
        private Vector3 _platformConnectedAnchor;
        private float _upAnchorY;
        private ConfigurableJoint _platformConfigurableJoint;

        private Vector3 _position;
        
        public GrabberState grabberState = GrabberState.None;
        public ClawState clawState = ClawState.Opened;


        #region Variables After Refactor

        public Transform StartTransform { get; private set; }
        
        private StateMachine _stateMachine;
        public IdleState idleState;
        public StandingState standingState;
        public MovingState movingState;
        public GrabbingState grabbingState;

        #endregion
        
        private void Start()
        {
            Init();
            _stateMachine = new StateMachine();
            InitStates(_stateMachine);
            
            //buttonController.OnStartGrabbingAction += Grab;
            //buttonController.OnOpenClawAction += OpenClaw;
            
            _stateMachine.Initialize(idleState);
        }
        
        private void Update()
        {
            _stateMachine.CurrentState.HandleInput();
            _stateMachine.CurrentState.LogicUpdate();
        }
        
        void FixedUpdate()
        {
            _stateMachine.CurrentState.PhysicsUpdate();
            
            // Moving();
            //GrabbingProcess();
        }
        
        

        private void Init()
        {
            StartTransform = transform;
            InitGrabberAnchors();
        }

        private void InitGrabberAnchors()
        {
            _movingPlatformRigidbody = GetComponent<Rigidbody>();
            _openClawAnchorY = InitAnchor(pistonRigidbody, out _pistonConfigurableJoint, out _pistonConnectedAnchor);
            _upAnchorY = InitAnchor(_movingPlatformRigidbody, out _platformConfigurableJoint, out _platformConnectedAnchor);
        }

        private float InitAnchor(Rigidbody rb, out ConfigurableJoint joint, out Vector3 anchor)
        {
            joint = rb.GetComponent<ConfigurableJoint>();
            anchor = joint.connectedAnchor;
            return anchor.y;
        }

        private void InitStates(StateMachine stateMachine)
        {
            idleState = new IdleState(this, stateMachine);
            movingState = new MovingState(this, stateMachine);
            standingState = new StandingState(this, stateMachine);
        }
        
        public void Moving()
        {
            _position = transform.position;
            float x = Mathf.Clamp(_position.x + joystick.InputZ * moveSpeed, x_Ancors._left, x_Ancors._right);
            float z = Mathf.Clamp(_position.z + joystick.InputX * moveSpeed, z_Ancors._left, z_Ancors._right);
            
            Vector3 mInput = new Vector3(x, _position.y, z);
            _movingPlatformRigidbody.MovePosition(mInput);
        }
        
        private void Grab()
        {
            if (grabberState == GrabberState.None && clawState == ClawState.Opened)
            {
                grabberState = GrabberState.StartGrabbing;
                //SetActiveObjectDestroyer(true);
            }
            else if (grabberState == GrabberState.StartGrabbing)
            {
                CloseClaw();
            }
        }

        private void OpenClaw()
        {
            if (grabberState == GrabberState.EndGrabbing && clawState == ClawState.Closed)
            {
                _pistonConfigurableJoint.connectedAnchor = new Vector3(_pistonConnectedAnchor.x, _openClawAnchorY, _pistonConnectedAnchor.z);
                
                grabberState = GrabberState.None;
                clawState = ClawState.Opened;
            }
        }

        private void GrabbingProcess()
        {
            if (grabberState == GrabberState.StartGrabbing)
            {
                if (_platformConnectedAnchor.y < downAnchorY && clawState == ClawState.Opened)
                {
                    _platformConfigurableJoint.connectedAnchor = Vector3.Lerp(new Vector3(_pistonConnectedAnchor.x,_platformConnectedAnchor.y + downSpeed ,_platformConnectedAnchor.z),
                        new Vector3(_platformConnectedAnchor.x, downAnchorY, _platformConnectedAnchor.z),
                        downSpeed * Time.deltaTime);
                    
                    _platformConnectedAnchor = _platformConfigurableJoint.connectedAnchor;
                }
                else
                {
                    CloseClaw();
                }

                if (clawState == ClawState.Closed && grabberState != GrabberState.EndGrabbing)
                {
                    if (_platformConnectedAnchor.y > _upAnchorY)
                    {
                        _platformConfigurableJoint.connectedAnchor = Vector3.Lerp(new Vector3(_pistonConnectedAnchor.x,_platformConnectedAnchor.y - downSpeed ,_platformConnectedAnchor.z),
                            new Vector3(_platformConnectedAnchor.x, _upAnchorY, _platformConnectedAnchor.z),
                            downSpeed * Time.deltaTime);
                        
                        _platformConnectedAnchor = _platformConfigurableJoint.connectedAnchor;
                    }
                    else
                    {
                        grabberState = GrabberState.EndGrabbing;
                       //SetActiveObjectDestroyer(false);
                        if (!objectRetentionCheck.CheckRetention())
                        {
                            OpenClaw();
                        }
                    }
                }
            }
            else if(grabberState == GrabberState.EndGrabbing)
            {
                if (!objectRetentionCheck.CheckRetention())
                {
                    OpenClaw();
                }
            }
        }

        private void CloseClaw()
        {
            _pistonConfigurableJoint.connectedAnchor = new Vector3(_pistonConnectedAnchor.x, closeClawAnchorY, _pistonConnectedAnchor.z);
            clawState = ClawState.Closed;
        }
        
    }
}