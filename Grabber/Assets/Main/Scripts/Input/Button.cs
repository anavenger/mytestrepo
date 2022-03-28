using System;
using UnityEngine;

namespace TTP.UserInput
{
    [RequireComponent(typeof(Animator))]
    public class Button : MonoBehaviour
    {
        private Animator _animator;
        private const string ButtonClickTrigger = "ButtonClick";

        void Start()
        {
            _animator = GetComponent<Animator>();
        }

        private void ButtonClick()
        {
            bool click = false;
            
            
            if (Input.GetKeyDown(KeyCode.Space))
            {
                click = true;
            }
            else if(Input.GetKeyDown(KeyCode.LeftShift))
            {
                click = true;
            }
            
            if(!click) return;
            
            _animator.ResetTrigger(ButtonClickTrigger);
            _animator.SetTrigger(ButtonClickTrigger);
        }
    }
}


