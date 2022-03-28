using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime;
using TTP.Toys;
using UnityEngine;
using TTP.Utilities;
using TTP.UserInput;
using UnityEngine.Serialization;
using TTP.State;

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
        [SerializeField] private ScoreController scoreController; //убрать отсюда
        [SerializeField] private ObjectRetentionCheck objectRetentionCheck;// будет не нужно
        
        public Joystick joystick;// скорее всего граберу не нужно знать о джойстике (джойстик нужен на этапе движения), то же самое кнопка - она нужна только 
        public Button button;
        
        [SerializeField] private STupple x_Ancors;
        [SerializeField] private STupple z_Ancors;

        [SerializeField] private float moveSpeed = 20f;
        [SerializeField] private float downSpeed = 0.03f;
                
        [Header("Hand Piston")]
        [SerializeField] private Rigidbody pistonRB;
        [SerializeField] private float closedClawAnchorY = 0.35f;
        private float _openedClawAnchorY;
        private Vector3 _pistonConnectedAnchor;
        private ConfigurableJoint _pistonConfigurableJoint;

        [Header("Moving Platform")]
        [SerializeField] private float downAnchorLimit = 8.5f;
        private Rigidbody _platformRB;
        private float _upAnchorY;
        private Vector3 _platformConnectedAnchor;
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
            _platformRB = GetComponent<Rigidbody>();
            _openedClawAnchorY = InitAnchor(pistonRB, out _pistonConfigurableJoint, out _pistonConnectedAnchor);
            _upAnchorY = InitAnchor(_platformRB, out _platformConfigurableJoint, out _platformConnectedAnchor);
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
            grabbingState = new GrabbingState(this, stateMachine);
        }
        
        public void Move()
        {
            _position = transform.position;
            float x = Mathf.Clamp(_position.x + joystick.InputZ * moveSpeed, x_Ancors._left, x_Ancors._right);
            float z = Mathf.Clamp(_position.z + joystick.InputX * moveSpeed, z_Ancors._left, z_Ancors._right);
            
            Vector3 mInput = new Vector3(x, _position.y, z);
            _platformRB.MovePosition(mInput);
        }
        
        public bool GoDown()
        {
            bool downCompleted = true;

            if (_platformConnectedAnchor.y < downAnchorLimit)
            {
                _platformConfigurableJoint.connectedAnchor = Vector3.Lerp(
                        new Vector3(_pistonConnectedAnchor.x, _platformConnectedAnchor.y + downSpeed, _platformConnectedAnchor.z),
                        new Vector3(_platformConnectedAnchor.x, downAnchorLimit, _platformConnectedAnchor.z),
                        downSpeed * Time.fixedDeltaTime);

                _platformConnectedAnchor = _platformConfigurableJoint.connectedAnchor;
                downCompleted = false;
            }
            return downCompleted;
        }

        public void CloseClaw()
        {
            _pistonConfigurableJoint.connectedAnchor = new Vector3(_pistonConnectedAnchor.x, closedClawAnchorY, _pistonConnectedAnchor.z);
        }

        public void GoUp()
        {
            if (_platformConnectedAnchor.y > _upAnchorY)
            {
                _platformConfigurableJoint.connectedAnchor = Vector3.Lerp(new Vector3(_pistonConnectedAnchor.x, _platformConnectedAnchor.y - downSpeed, _platformConnectedAnchor.z),
                    new Vector3(_platformConnectedAnchor.x, _upAnchorY, _platformConnectedAnchor.z),
                    downSpeed * Time.fixedDeltaTime);

                _platformConnectedAnchor = _platformConfigurableJoint.connectedAnchor;
            }
        }

        public void OpenClaw()
        {
            _pistonConfigurableJoint.connectedAnchor = new Vector3(_pistonConnectedAnchor.x, _openedClawAnchorY, _pistonConnectedAnchor.z);
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

        

        private void GrabbingProcess()
        {
            if (grabberState == GrabberState.StartGrabbing)
            {
                if (_platformConnectedAnchor.y < downAnchorLimit && clawState == ClawState.Opened)
                {
                    _platformConfigurableJoint.connectedAnchor = Vector3.Lerp(new Vector3(_pistonConnectedAnchor.x,_platformConnectedAnchor.y + downSpeed ,_platformConnectedAnchor.z),
                        new Vector3(_platformConnectedAnchor.x, downAnchorLimit, _platformConnectedAnchor.z),
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



        
        
    }
}