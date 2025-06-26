using UnityEngine;
#if UNITY_5_3_OR_NEWER
using UnityEngine.SceneManagement;
#endif

namespace Invector.CharacterController
{
    public class vThirdPersonInput : MonoBehaviour
    {
        #region variables

        [Header("Axes de mouvements(voir l'input manager dans les settings)")]
        public string horizontalInput = "Horizontal";
        public string verticallInput = "Vertical";
        [Header("Touche de saut")]
        public KeyCode jumpInput = KeyCode.Space;
        [Header("Touche de mouvements lateraux")]
        public KeyCode strafeInput = KeyCode.Tab;
        [Header("Touche de sprint")]
        public KeyCode sprintInput = KeyCode.LeftShift;
        [Header("Touche de marche")]
        public KeyCode walkInput = KeyCode.CapsLock;
        [Header("Touche pour s'accroupir")]
        public KeyCode crouchInput = KeyCode.LeftControl;
        [Header("Touche de vol")]
        public KeyCode flyInput = KeyCode.F;

		[Header("Controls camera")]
        public string rotateCameraXInput ="Mouse X";
        public string rotateCameraYInput = "Mouse Y";

        protected vThirdPersonCamera tpCamera;                // acess camera info        
        [HideInInspector]
        public string customCameraState;                    // generic string to change the CameraState        
        [HideInInspector]
        public string customlookAtPoint;                    // generic string to change the CameraPoint of the Fixed Point Mode        
        [HideInInspector]
        public bool changeCameraState;                      // generic bool to change the CameraState        
        [HideInInspector]
        public bool smoothCameraState;                      // generic bool to know if the state will change with or without lerp  
        [HideInInspector]
        public bool keepDirection;                          // keep the current direction in case you change the cameraState

        protected vThirdPersonController cc;                // access the ThirdPersonController component  

		#endregion

		protected virtual void Start()
        {
            CharacterInit();
        }

        protected virtual void CharacterInit()
        {
            cc = GetComponent<vThirdPersonController>();
            if (cc != null)
                cc.Init();

            tpCamera = FindObjectOfType<vThirdPersonCamera>();
            if (tpCamera) tpCamera.SetMainTarget(this.transform);

            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        protected virtual void LateUpdate()
        {
            if (cc == null) return;             // returns if didn't find the controller		    
            InputHandle();                      // update input methods
            UpdateCameraStates();               // update camera states
        }

        protected virtual void FixedUpdate()
        {
            cc.AirControl();
            CameraInput();
        }

        protected virtual void Update()
        {
            cc.UpdateMotor();                   // call ThirdPersonMotor methods               
            cc.UpdateAnimator();                // call ThirdPersonAnimator methods		               
        }

        protected virtual void InputHandle()
        {
            ExitGameInput();
            CameraInput();

            if (!cc.lockMovement)
            {
                MoveCharacter();
                StrafeInput();
                JumpInput();
				WalkInput();
				SprintInput();
				CrouchInput();
				FlyInput();
				SwimInput();
				
			}
        }

        #region Basic Locomotion Inputs      

        protected virtual void MoveCharacter()
        {            
            cc.input.x = Input.GetAxis(horizontalInput);
            cc.input.y = Input.GetAxis(verticallInput);
		}

        protected virtual void StrafeInput()
        {
            if (Input.GetKeyDown(strafeInput))
                cc.Strafe();
        }

		public virtual void WalkInput() {

			if (Input.GetKeyDown(walkInput)) {
					cc.Walking(true);
			}
			else if (Input.GetKeyUp(walkInput)) {
					cc.Walking(false);
			}

		}

		public virtual void CrouchInput() {

			if (Input.GetKeyDown(crouchInput)) {
				cc.Crouching(true);
			}
			else if (Input.GetKeyUp(crouchInput)) {
				cc.Crouching(false);
			}

		}

		public virtual void FlyInput() {

			if (Input.GetKeyDown(flyInput) && cc.FlyingAllowed)
				cc.Flying();

			if (cc.isflying) {
				if (Input.GetKey(jumpInput)) {
					cc.going_up = true;
				}
				else if (Input.GetKeyUp(jumpInput)) {
					cc.going_up = false;
				}
				else if (Input.GetKey(crouchInput)) {
					cc.going_down = true;
				}
				else if (Input.GetKeyUp(crouchInput)) {
					cc.going_down = false;
				}

			}
			else if(!cc.swimming) {
				cc.going_down = false;
				cc.going_up = false;
			}
		}

		protected virtual void SprintInput()
        {
			if (Input.GetKeyDown(sprintInput)) {
					cc.Sprint(true);
			}
			else if (Input.GetKeyUp(sprintInput)) {
					cc.Sprint(false);
			}
        }

        protected virtual void JumpInput()
        {
            if (!cc.swimming || !cc.FlyingAllowed)
            {
                if (Input.GetKeyDown(jumpInput))
                    cc.Jump();
            }
        }

		protected virtual void SwimInput() {

			if (cc.swimming) {
				if (Input.GetKey(jumpInput)) {
					cc.going_up = true;
				}
				else if (Input.GetKeyUp(jumpInput)) {
					cc.going_up = false;
				}
				else if (Input.GetKey(crouchInput)) {
					cc.going_down = true;
				}
				else if (Input.GetKeyUp(crouchInput)) {
					cc.going_down = false;
				}
			}
			else if(!cc.isflying) {
				cc.going_down = false;
				cc.going_up = false;
			}
		}

        protected virtual void ExitGameInput()
        {
            // just a example to quit the application 
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (!Cursor.visible)
                    Cursor.visible = true;
                else
                    Application.Quit();
            }
        }

        #endregion

        #region Camera Methods

        protected virtual void CameraInput()
        {
            if (tpCamera == null)
                return;
            var Y = Input.GetAxis(rotateCameraYInput);
            var X = Input.GetAxis(rotateCameraXInput);

            tpCamera.RotateCamera(X, Y);

            // tranform Character direction from camera if not KeepDirection
            if (!keepDirection)
                cc.UpdateTargetDirection(tpCamera != null ? tpCamera.transform : null);
            // rotate the character with the camera while strafing        
            RotateWithCamera(tpCamera != null ? tpCamera.transform : null);            
        }

        protected virtual void UpdateCameraStates()
        {
            // CAMERA STATE - you can change the CameraState here, the bool means if you want lerp of not, make sure to use the same CameraState String that you named on TPCameraListData
            if (tpCamera == null)
            {
                tpCamera = FindObjectOfType<vThirdPersonCamera>();
                if (tpCamera == null)
                    return;
                if (tpCamera)
                {
                    tpCamera.SetMainTarget(this.transform);
                    tpCamera.Init();
                }
            }            
        }

        protected virtual void RotateWithCamera(Transform cameraTransform)
        {
            if (cc.isStrafing && !cc.lockMovement && !cc.lockMovement)
            {                
                cc.RotateWithAnotherTransform(cameraTransform);                
            }
        }

        #endregion     
    }
}