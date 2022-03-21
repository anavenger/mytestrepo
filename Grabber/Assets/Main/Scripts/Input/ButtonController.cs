using System;
using UnityEngine;

namespace TTP.UserInput
{
    [RequireComponent(typeof(Animator))]
    public class ButtonController : MonoBehaviour
    {
        private Animator _animator;
        private const string ButtonClickTrigger = "ButtonClick";

        public Action OnStartGrabbingAction;
        public Action OnOpenClawAction;
    
        void Start()
        {
            _animator = GetComponent<Animator>();
        }
    
        void Update()
        {
            ButtonClick();
        }

        private void ButtonClick()
        {
            bool click = false;
            
            
            if (Input.GetKeyDown(KeyCode.Space))
            {
                click = true;
                OnStartGrabbingAction?.Invoke();
            }
            else if(Input.GetKeyDown(KeyCode.LeftShift))
            {
                click = true;
                OnOpenClawAction?.Invoke();
            }
            
            if(!click) return;
            
            _animator.ResetTrigger(ButtonClickTrigger);
            _animator.SetTrigger(ButtonClickTrigger);
        }
    }
}


