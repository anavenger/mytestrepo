using UnityEngine;

namespace TTP.UserInput
{
    public class Joystick : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private Rigidbody joystickRigidbody;
        private Quaternion _rotation;

        public float InputZ { get; private set; }
        public float InputX { get; private set; }

        void Start()
        {
            _rotation = transform.rotation;
        }
     
        public void Move()
        {
            GetInput();
            RotateByInput();
        }

        public void MoveToIdle()
        {
            ResetInput();
            RotateByInput();
        }

        private void ResetInput()
        {
            InputX = InputZ = 0f;
        }

        private void GetInput()
        {
            InputZ = Input.GetAxis("Horizontal") * Time.deltaTime;
            InputX = Input.GetAxis("Vertical") * Time.deltaTime;
        }

        private void RotateByInput()
        {
            var rotation = new Quaternion(InputX * moveSpeed, _rotation.y, -InputZ * moveSpeed, _rotation.w);
            joystickRigidbody.MoveRotation(rotation.normalized);
        }
    }
}

