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

namespace TTP.Controllers
{
    public class Grabber : MonoBehaviour
    {
        [SerializeField] private ScoreController scoreController; 
        [SerializeField] private ObjectRetentionCheck objectRetentionCheck;
        
        public Joystick joystick; 
        public Button button;

        [SerializeField] private Tupple<float> anchorsX;
        [SerializeField] private Tupple<float> anchorsZ;

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
        public OpeningClawState OpeningClawState;

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
            OpeningClawState = new OpeningClawState(this, stateMachine);
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
            float x = Mathf.Clamp(_position.x + joystick.InputZ * moveSpeed, anchorsX.min, anchorsX.max);
            float z = Mathf.Clamp(_position.z + joystick.InputX * moveSpeed, anchorsZ.min, anchorsZ.max);
            
            Vector3 mInput = new Vector3(x, _position.y, z);
            _platformRB.MovePosition(mInput);
        }

        // public void MoveBack()
        // {
        //     _position = transform.position;
        //     Vector3 moveVector = new Vector3();
        //     if (_position.x > x_Ancors.min)
        //     {
        //         moveVector = Vector3.Lerp(
        //             new Vector3(_position.x - moveBackSpeed, _position.y, _position.z),
        //             new Vector3(x_Ancors.min, _position.y, _position.z),
        //             moveBackSpeed * Time.fixedDeltaTime);
        //         _platformRB.MovePosition(moveVector);
        //     }
        //     if (_position.z > z_Ancors.min)
        //     {
        //         moveVector = Vector3.Lerp(
        //             new Vector3(_position.x, _position.y, _position.z - moveBackSpeed),
        //              new Vector3(_position.x, _position.y, z_Ancors.min),
        //             moveBackSpeed * Time.fixedDeltaTime);
        //         _platformRB.MovePosition(moveVector);
        //     }
        // }

        private bool _isBackToX;
        private bool _isBackToZ;
        
        public bool MoveBack()
        {
            _position = transform.position;
            
            if (!_isBackToX)
                _isBackToX = MoveBack(true, anchorsX.min);
            if (!_isBackToZ)
                _isBackToZ = MoveBack(false, anchorsZ.min);
            
            return _isBackToX && _isBackToZ;
        }

        public void ResetBackMove()
        {
            _isBackToX = _isBackToZ = false;
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
            SetPistonAnchor(closedClawAnchorY);
        }
        
        public void OpenClaw()
        {
            SetPistonAnchor(_openedClawAnchorY);
        }
        
        private bool MoveBack(bool movingToX, float minPosition)
        {
            bool moveBackCompleted;
            
            var posX1 = (movingToX) ? _position.x - moveBackSpeed : _position.x;
            var posZ1 = (!movingToX) ? _position.z - moveBackSpeed : _position.z;
            
            var posX2 = (movingToX) ? minPosition: _position.x;
            var posZ2 = (!movingToX) ? minPosition : _position.z;

            var comparePosition = (movingToX) ? _position.x : _position.z;
            
            if (comparePosition > minPosition)
            {
                var newPosition = Vector3.Lerp(
                    new Vector3(posX1, _position.y, posZ1),
                    new Vector3(posX2, _position.y, posZ2),
                    moveBackSpeed * Time.fixedDeltaTime);
                
                _platformRB.MovePosition(newPosition);
                return moveBackCompleted = false;
            }

            comparePosition = minPosition;
            return moveBackCompleted = true;
        }

        private void SetPistonAnchor(float valueY)
        {
            _pistonConfigurableJoint.connectedAnchor = new Vector3(_pistonConnectedAnchor.x, valueY, _pistonConnectedAnchor.z);
        }
    }
}