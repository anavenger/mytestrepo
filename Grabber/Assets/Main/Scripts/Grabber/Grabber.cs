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
    public class Grabber : MonoBehaviour
    {
        [SerializeField] private ScoreController scoreController; 
        [SerializeField] private ObjectRetentionCheck objectRetentionCheck;
        
        public Joystick joystick; 
        public Button button;
        
        [SerializeField] private STupple x_Ancors;
        [SerializeField] private STupple z_Ancors;

        [SerializeField] private float moveSpeed = 20f;
        [SerializeField] private float downSpeed = 0.03f;
        [SerializeField] private float moveBackSpeed = 0.03f;
                
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

        #region Variables After Refactor

        public Transform StartTransform { get; private set; }
        
        private StateMachine _stateMachine;
        
        public IdleState IdleState;
        public StandingState StandingState;
        public MovingState MovingState;
        public GoingDownState GoingDownState;
        public GoingUpState GoingUpState;
        public GrabbingState GrabbingState;
        public GoingBackState GoingBackState;

        #endregion
        
        private void Start()
        {
            Init();
        }
        
        private void Update()
        {
            _stateMachine.CurrentState.HandleInput();
            _stateMachine.CurrentState.LogicUpdate();
        }
        
        private void FixedUpdate()
        {
            _stateMachine.CurrentState.PhysicsUpdate();
        }
        
        private void Init()
        {
            StartTransform = transform;
            _stateMachine = new StateMachine();
            
            InitGrabberAnchors();
            InitStates(_stateMachine);
            _stateMachine.Initialize(IdleState);
        }

        private void InitStates(StateMachine stateMachine)
        {
            IdleState = new IdleState(this, stateMachine);
            MovingState = new MovingState(this, stateMachine);
            StandingState = new StandingState(this, stateMachine);
            GoingDownState = new GoingDownState(this, stateMachine);
            GrabbingState = new GrabbingState(this, stateMachine);
            GoingUpState = new GoingUpState(this, stateMachine);
            GoingBackState = new GoingBackState(this, stateMachine);
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
        
        public void Move()
        {
            _position = transform.position;
            float x = Mathf.Clamp(_position.x + joystick.InputZ * moveSpeed, x_Ancors._left, x_Ancors._right);
            float z = Mathf.Clamp(_position.z + joystick.InputX * moveSpeed, z_Ancors._left, z_Ancors._right);
            
            Vector3 mInput = new Vector3(x, _position.y, z);
            _platformRB.MovePosition(mInput);
        }

        public void MoveBack()
        {
            _position = transform.position;
            Vector3 moveVector = new Vector3();
            if (_position.x > x_Ancors._left)
            {
                moveVector = Vector3.Lerp(
                    new Vector3(_position.x - moveBackSpeed, _position.y, _position.z),
                    new Vector3(x_Ancors._left, _position.y, _position.z),
                    moveBackSpeed * Time.fixedDeltaTime);
                _platformRB.MovePosition(moveVector);
            }
            if (_position.z < z_Ancors._right)
            {
                moveVector = Vector3.Lerp(
                    new Vector3(_position.x, _position.y, _position.z + moveBackSpeed),
                    new Vector3(_position.x, _position.y, z_Ancors._right),
                    moveBackSpeed * Time.fixedDeltaTime);
                _platformRB.MovePosition(moveVector);
            }
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
        
        public bool GoUp()
        {
            bool upCompleted = true;
            
            if (_platformConnectedAnchor.y > _upAnchorY)
            {
                _platformConfigurableJoint.connectedAnchor = Vector3.Lerp(
                    new Vector3(_pistonConnectedAnchor.x, _platformConnectedAnchor.y - downSpeed, _platformConnectedAnchor.z),
                    new Vector3(_platformConnectedAnchor.x, _upAnchorY, _platformConnectedAnchor.z),
                    downSpeed * Time.fixedDeltaTime);

                _platformConnectedAnchor = _platformConfigurableJoint.connectedAnchor;
                upCompleted = false;
            }
            return upCompleted;
        }
        
        public void CloseClaw()
        {
            _pistonConfigurableJoint.connectedAnchor = new Vector3(_pistonConnectedAnchor.x, closedClawAnchorY, _pistonConnectedAnchor.z);
        }
        
        public void OpenClaw()
        {
            _pistonConfigurableJoint.connectedAnchor = new Vector3(_pistonConnectedAnchor.x, _openedClawAnchorY, _pistonConnectedAnchor.z);
        }
    }
}