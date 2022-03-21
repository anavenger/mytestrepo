using UnityEngine;

namespace TTP.UserInput
{
    public class JoystickController : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private Rigidbody joystickRigidbody;

        
        private Quaternion rotation;

        public float JoystickInputZ { get; private set; }

        public float JoystickInputX { get; private set; }
        
        void Start()
        {
            rotation = transform.rotation;
        }

        private void FixedUpdate()
        {
            Moving();
        }

        private void Moving()
        {
            JoystickInputZ = Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime;
            JoystickInputX = Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime;
            var mInput = new Quaternion(JoystickInputX, rotation.y, -JoystickInputZ , rotation.w);
            mInput  = mInput .normalized;
            joystickRigidbody.MoveRotation(mInput );
        }
    }
}

