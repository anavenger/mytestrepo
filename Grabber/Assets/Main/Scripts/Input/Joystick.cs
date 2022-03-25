using UnityEngine;

namespace TTP.UserInput
{
    public class Joystick : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private Rigidbody joystickRigidbody;
        private Quaternion rotation;

        public float InputZ { get; set; }
        public float InputX { get; set; }

        public bool HasInput
        {
            get
            {
                GetInput();
                Debug.Log($"InputZ = {InputZ}");
                Debug.Log($"InputX = {InputX}");
                if ((InputZ > 0.001f || InputZ < 0.001f) || (InputX > 0.001f || InputX < 0.001f))
                    return true;
                
                return false;
            }
        }
        
        void Start()
        {
            rotation = transform.rotation;
        }

        // private void FixedUpdate()
        // {
        //     Moving();
        // }

        public void GetInput()
        {
            InputZ = Input.GetAxis("Horizontal") * Time.deltaTime;
            InputX = Input.GetAxis("Vertical") * Time.deltaTime;
            //Debug.Log($"InputZ = {InputZ} / InputX  = {InputX }");
        }

        public void Moving()
        {
            var mInput = new Quaternion(InputX * moveSpeed, rotation.y, -InputZ * moveSpeed, rotation.w);
            joystickRigidbody.MoveRotation(mInput.normalized);
        }
    }
}

