using UnityEngine;
using System.Collections;

namespace Invector.CharacterController
{
    public class vThirdPersonController : vThirdPersonAnimator
    {
        protected virtual void Start()
        {
#if !UNITY_EDITOR
                Cursor.visible = false;
#endif
        }

        public virtual void Sprint(bool value)
        {                                   
            isSprinting = value;            
        }

		public virtual void Walking(bool value) {
			isWalking = value;
		}

		public virtual void Crouching(bool value) {
			if (isWalking == false && isSprinting == false) {
				isCrouching = value;
			}
			else {

				isCrouching = false;
			}
		}

		public virtual void Flying() {
			isflying = !isflying;

		}

		public virtual void Strafe()
        {
            if (locomotionType == LocomotionType.OnlyFree) return;
            isStrafing = !isStrafing;
        }

        public virtual void Jump()
        {
            // conditions to do this action
            bool jumpConditions = ((isGrounded && !isJumping) || (doubleJump < maxDoubleJump)) && !swimming && !isflying;
			// return if jumpCondigions is false
			if (jumpConditions) {
				// trigger jump behaviour
				jumpCounter = jumpTimer;
				isJumping = true;
				doubleJump += 1;
				// trigger jump animations            
				if (_rigidbody.linearVelocity.magnitude < 1)
					animator.CrossFadeInFixedTime("Jump", 0.1f);
				else
					animator.CrossFadeInFixedTime("JumpMove", 0.2f);
			}
			/*else if (swimming && !isflying) {
				//jumpCounter = 0.2f;
				//isJumping = true;
			}
			else if (isflying)
			{
				//jumpCounter = 0.2f;
				//isJumping = true;
			}*/
        }

        public virtual void RotateWithAnotherTransform(Transform referenceTransform)
        {
            var newRotation = new Vector3(transform.eulerAngles.x, referenceTransform.eulerAngles.y, transform.eulerAngles.z);
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(newRotation), strafeRotationSpeed * Time.fixedDeltaTime);
            targetRotation = transform.rotation;
        }
    }
}