using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using Invector;
using UnityEngine.EventSystems;

namespace Invector.CharacterController
{
    public abstract class vThirdPersonMotor : MonoBehaviour
    {
        #region Variables        

        #region Layers
        [Header("Elements qui comptent comme un sol")]
        public LayerMask groundLayer = 1 << 0;
        [Tooltip("Distance to became not grounded")]
       // [SerializeField]
        protected float groundMinDistance = 0.2f;
       // [SerializeField]
        protected float groundMaxDistance = 0.5f;
        #endregion

        #region Character Variables

        public enum LocomotionType
        {
            FreeWithStrafe,
            OnlyStrafe,
            OnlyFree
        }

        [Header("--- Parametres de mouvement ---")]
        [HideInInspector]
        public LocomotionType locomotionType = LocomotionType.FreeWithStrafe;
        //[Tooltip("lock the player movement")]
        [HideInInspector]
        public bool lockMovement;
        [Tooltip("Speed of the rotation on free directional movement")]
        //[SerializeField]
        [Header("Vitesse de Rotation du personnage")]
        public float freeRotationSpeed = 10f;
       // [Tooltip("Speed of the rotation while strafe movement")]
        [HideInInspector]
        public float strafeRotationSpeed = 10f;

        [Header("Parametres de Saut")]

        //[Tooltip("Check to control the character while jumping")]
        [HideInInspector]
        public bool jumpAirControl = true;
        //[Tooltip("How much time the character will be jumping")]
        [HideInInspector]
        public float jumpTimer = 0.3f;
        [HideInInspector]
        public float jumpCounter;
        [Header("Longueur de Saut (si on saute vers l'avant uniquement)")]
        [Tooltip("Add Extra jump speed, based on your speed input the character will move forward")]
        public float jumpForward = 3f;
        [Header("Hauteur de Saut")]
        [Tooltip("Add Extra jump height, if you want to jump only with Root Motion leave the value with 0.")]
        public float jumpHeight = 4f;
        [Header("Nombre de Sauts Aeriens")]
        [Tooltip("Number of air jump you can do")]
        public int maxDoubleJump = 1;

        //[Header("--- Movement Speed ---")]
        // [Tooltip("Check to drive the character using RootMotion of the animation")]
        [HideInInspector]
        public bool useRootMotion = false;
        [Header("Vitesse des Mouvements")]
        [Tooltip("Add extra speed for the locomotion movement, keep this value at 0 if you want to use only root motion speed. Is also the crouching walk speed")]
        public float freeWalkSpeed = 2.5f;
        [Tooltip("Add extra speed for the locomotion movement, keep this value at 0 if you want to use only root motion speed.")]
        public float freeRunningSpeed = 3f;
        [Tooltip("Add extra speed for the locomotion movement, keep this value at 0 if you want to use only root motion speed.")]
        public float freeSprintSpeed = 4f;

        [HideInInspector]
        [Tooltip("Add extra speed for the strafe movement, keep this value at 0 if you want to use only root motion speed.")]
        public float strafeWalkSpeed = 2.5f;
        [HideInInspector]
        [Tooltip("Add extra speed for the locomotion movement, keep this value at 0 if you want to use only root motion speed.")]
        public float strafeRunningSpeed = 3f;
        [HideInInspector]
        [Tooltip("Add extra speed for the locomotion movement, keep this value at 0 if you want to use only root motion speed.")]
        public float strafeSprintSpeed = 4f;

        //[Header("--- Grounded Setup ---")]
        [HideInInspector]
        [Tooltip("ADJUST IN PLAY MODE - Offset height limit for sters - GREY Raycast in front of the legs")]
        public float stepOffsetEnd = 0.45f;
        [HideInInspector]
        [Tooltip("ADJUST IN PLAY MODE - Offset height origin for sters, make sure to keep slight above the floor - GREY Raycast in front of the legs")]
        public float stepOffsetStart = 0.05f;
        [HideInInspector]
        [Tooltip("Higher value will result jittering on ramps, lower values will have difficulty on steps")]
        public float stepSmooth = 4f;
        [Tooltip("Max angle to walk")]
        //[SerializeField]
        protected float slopeLimit = 45f;       
        [Tooltip("Apply extra gravity when the character is not grounded")]
        //[SerializeField]
        public static float extraGravity = -10f;
        protected float groundDistance;
        public RaycastHit groundHit;

        #endregion

        #region Actions

        // movement bools
        [HideInInspector]
        public bool
            isGrounded,
            isStrafing,
            isSprinting,
            isSliding,
			isWalking,
			isCrouching;

        // action bools
        [HideInInspector]
        public bool
            isJumping;

        protected void RemoveComponents()
        {
            if (_capsuleCollider != null) Destroy(_capsuleCollider);
            if (_rigidbody != null) Destroy(_rigidbody);
            if (animator != null) Destroy(animator);
            var comps = GetComponents<MonoBehaviour>();
            for (int i = 0; i < comps.Length; i++)
            {
                Destroy(comps[i]);
            }
        }

        #endregion

        #region Direction Variables
        [HideInInspector]
        public Vector3 targetDirection;
        protected Quaternion targetRotation;
        [HideInInspector]
        public Quaternion freeRotation;
        [HideInInspector]
        public bool keepDirection;        

        #endregion

        #region Components               

        [HideInInspector]
        public Animator animator;                                   // access the Animator component
        [HideInInspector]
        public Rigidbody _rigidbody;                                // access the Rigidbody component
        [HideInInspector]
        public PhysicsMaterial maxFrictionPhysics, frictionPhysics, slippyPhysics;       // create PhysicMaterial for the Rigidbody
        [HideInInspector]
        public CapsuleCollider _capsuleCollider;                    // access CapsuleCollider information

        #endregion

        #region Hide Variables

        [HideInInspector]
        public float colliderHeight;                        // storage capsule collider extra information                
        [HideInInspector]
        public Vector2 input;                               // generate input for the controller        
        [HideInInspector]
        public float speed, direction, verticalVelocity;    // general variables to the locomotion
        [HideInInspector]
        public float velocity;                              // velocity to apply to rigidbody       

		#endregion


		[HideInInspector]
        public int doubleJump = 0;
		[Header("--- Bonus ---")]
		[Tooltip("Swimming speed -multiplied by walk speed-")]
		public float SwimSpeed = 2f;
		[Tooltip("Crouching speed -multiplied by walk speed-")]
		public float CrouchSpeed = 0.5f;
		[Tooltip("Flying speed -multiplied by walk speed-")]
		public float FlySpeed = 4f;
		[Tooltip("Flying is allowed for the character")]
		public bool FlyingAllowed;
		[HideInInspector]
		public bool isflying = false;
		[HideInInspector]
		public bool going_up;
		[HideInInspector]
		public bool going_down;
		[HideInInspector]
		public bool swimming = false;
        [HideInInspector]
        public GameObject dans_la_flotte;
		Vector3 initial_flotte_position;
		float initial_collider_height;
		Vector3 initial_collider_pos;

		#endregion

		public void Init()
        {
            // this method is called on the Start of the ThirdPersonController
            strafeRotationSpeed = freeRotationSpeed;
            strafeWalkSpeed = freeWalkSpeed;
            strafeRunningSpeed = freeRunningSpeed;
            strafeSprintSpeed = freeSprintSpeed;
            if (!dans_la_flotte) {
                dans_la_flotte = GameObject.FindObjectOfType<InWater>().gameObject;
                    }
            // access components
            animator = GetComponent<Animator>();

            // slides the character through walls and edges
            frictionPhysics = new PhysicsMaterial();
            frictionPhysics.name = "frictionPhysics";
            frictionPhysics.staticFriction = .25f;
            frictionPhysics.dynamicFriction = .25f;
            frictionPhysics.frictionCombine = PhysicsMaterialCombine.Multiply;

            // prevents the collider from slipping on ramps
            maxFrictionPhysics = new PhysicsMaterial();
            maxFrictionPhysics.name = "maxFrictionPhysics";
            maxFrictionPhysics.staticFriction = 1f;
            maxFrictionPhysics.dynamicFriction = 1f;
            maxFrictionPhysics.frictionCombine = PhysicsMaterialCombine.Maximum;

            // air physics 
            slippyPhysics = new PhysicsMaterial();
            slippyPhysics.name = "slippyPhysics";
            slippyPhysics.staticFriction = 0f;
            slippyPhysics.dynamicFriction = 0f;
            slippyPhysics.frictionCombine = PhysicsMaterialCombine.Minimum;

            // rigidbody info
            _rigidbody = GetComponent<Rigidbody>();

            // capsule collider info
            _capsuleCollider = GetComponent<CapsuleCollider>();

			initial_flotte_position = dans_la_flotte.GetComponent<BoxCollider>().center;
			initial_collider_height = this.gameObject.GetComponent<CapsuleCollider>().height;
			initial_collider_pos = this.gameObject.GetComponent<CapsuleCollider>().center;


		}

		public virtual void UpdateMotor()
        {
            CheckGround();
            ControlLocomotion();
			ControlCrouching();
			ControlFlying();
            if (!swimming)
                ControlJumpBehaviour();
            else
                SwimmingBehavious();

        }
        

        #region Locomotion 

        protected bool freeLocomotionConditions
        {
            get
            {
                if (locomotionType.Equals(LocomotionType.OnlyStrafe)) isStrafing = true;
                return !isStrafing && !locomotionType.Equals(LocomotionType.OnlyStrafe) || locomotionType.Equals(LocomotionType.OnlyFree);
            }
        }

        void ControlLocomotion()
        {
            if (freeLocomotionConditions)
                FreeMovement();     // free directional movement
            else
                StrafeMovement();   // move forward, backwards, strafe left and right
        }

		void ControlCrouching() {

			if (isCrouching) {

				dans_la_flotte.GetComponent<BoxCollider>().center = new Vector3 (initial_flotte_position.x, initial_flotte_position.y -7f, initial_flotte_position.z);
				this.GetComponent<CapsuleCollider>().height = initial_collider_height / 2;
				this.GetComponent<CapsuleCollider>().center = new Vector3(initial_collider_pos.x, initial_collider_pos.y / 2f, initial_collider_pos.z);
			}
			else {

				dans_la_flotte.GetComponent<BoxCollider>().center = initial_flotte_position;
				this.GetComponent<CapsuleCollider>().height = initial_collider_height;
				this.GetComponent<CapsuleCollider>().center = initial_collider_pos;
			}


		}

		void ControlFlying() {

			if (!isflying) {
				GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
                this.GetComponent<CapsuleCollider>().direction = 1;
                return;
			}

            if (Mathf.Abs(_rigidbody.linearVelocity.x) > 1 || Mathf.Abs(_rigidbody.linearVelocity.z) > 1) {
                this.GetComponent<CapsuleCollider>().direction = 2;
            }
            else {
                this.GetComponent<CapsuleCollider>().direction = 1;
            }

            if (going_up) {
				GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
				// apply extra force to the jump height   
				var vel = _rigidbody.linearVelocity;
				vel.y = jumpHeight * (FlySpeed / 2);
				_rigidbody.linearVelocity = vel;
			}
			else if (going_down) {
				GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
				// apply extra force to the jump height   
				var vel = _rigidbody.linearVelocity;
				vel.y = -jumpHeight * (FlySpeed / 2);
				_rigidbody.linearVelocity = vel;
			}
			else {

                GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
			}


		}

		void StrafeMovement()
        {
            var _speed = Mathf.Clamp(input.y, -1f, 1f);
            var _direction = Mathf.Clamp(input.x, -1f, 1f);
            speed = _speed;
            direction = _direction;
			if (speed != 0) {
				if (isSprinting)
					speed += 0.5f;
			}
            if (direction >= 0.7 || direction <= -0.7 || speed <= 0.1) isSprinting = false;
        }

        public virtual void FreeMovement()
        {
            // set speed to both vertical and horizontal inputs
            speed = Mathf.Abs(input.x) + Mathf.Abs(input.y);            
            speed = Mathf.Clamp(speed, 0, 1f);
			// add 0.5f on sprint to change the animation on animator
			if (speed != 0) {  //need this line to avoid auto walking when pressing sprint or walk button
				if (isSprinting)
					speed += 0.5f;
				if (isWalking)
					speed -= 0.5f;
				if (swimming)
					speed = speed * SwimSpeed;
				if (isCrouching)
					speed = speed * CrouchSpeed;
				if (isflying)
					speed = speed * FlySpeed;
			}
                        
            if (input != Vector2.zero && targetDirection.magnitude > 0.1f)
            {
                Vector3 lookDirection = targetDirection.normalized;
                freeRotation = Quaternion.LookRotation(lookDirection, transform.up);
                var diferenceRotation = freeRotation.eulerAngles.y - transform.eulerAngles.y;
                var eulerY = transform.eulerAngles.y;

                // apply free directional rotation while not turning180 animations
                if (isGrounded || (!isGrounded && jumpAirControl))
                {
                    if (diferenceRotation < 0 || diferenceRotation > 0) eulerY = freeRotation.eulerAngles.y;
                    var euler = new Vector3(transform.eulerAngles.x, eulerY, transform.eulerAngles.z);
                    transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(euler), freeRotationSpeed * Time.deltaTime);
                }               
            }
        }
        protected void ControlSpeed(float velocity)
        {
            if (Time.deltaTime == 0) return;

            if (useRootMotion)
            {
                Vector3 v = (animator.deltaPosition * (velocity > 0 ? velocity : 1f)) / Time.deltaTime;
                v.y = _rigidbody.linearVelocity.y;
                _rigidbody.linearVelocity = Vector3.Lerp(_rigidbody.linearVelocity, v, 20f * Time.deltaTime);
            }
            else
            {
                var velY = transform.forward * velocity * speed;
                velY.y = _rigidbody.linearVelocity.y;
                var velX = transform.right * velocity * direction;
                velX.x = _rigidbody.linearVelocity.x;

                if (isStrafing)
                {
                    Vector3 v = (transform.TransformDirection(new Vector3(input.x, 0, input.y)) * (velocity > 0 ? velocity : 1f));
                    v.y = _rigidbody.linearVelocity.y;
                    _rigidbody.linearVelocity = Vector3.Lerp(_rigidbody.linearVelocity, v, 20f * Time.deltaTime);
                }
                else
                {
                    _rigidbody.linearVelocity = velY;
                    _rigidbody.AddForce(transform.forward * (velocity * speed) * Time.deltaTime, ForceMode.VelocityChange);
                }
            }
        }

        #endregion

        #region Jump Methods

        protected void ControlJumpBehaviour()
        {
            if (!isJumping) return;

            jumpCounter -= Time.deltaTime;
            if (jumpCounter <= 0)
            {
                jumpCounter = 0;
                isJumping = false;
            }
            // apply extra force to the jump height   
            var vel = _rigidbody.linearVelocity;
            vel.y = jumpHeight;
            _rigidbody.linearVelocity = vel;
        }

       protected void SwimmingBehavious()
        {
            if (isGrounded) {
                this.GetComponent<CapsuleCollider>().direction = 1;
                return;
            }
			if (!swimming) {
                this.GetComponent<CapsuleCollider>().direction = 1;
                return;
            }

            if (Mathf.Abs(_rigidbody.linearVelocity.x) > 1 || Mathf.Abs(_rigidbody.linearVelocity.z) > 1) {
                this.GetComponent<CapsuleCollider>().direction = 2;
            }
            else {
                this.GetComponent<CapsuleCollider>().direction = 1;
            }

            if (going_up) {
				// apply extra force to the jump height   
				var vel = _rigidbody.linearVelocity;
				vel.y = jumpHeight/2;
				_rigidbody.linearVelocity = vel;
			}
			else if (going_down) {
				// apply extra force to the jump height   
				var vel = _rigidbody.linearVelocity;
				vel.y = -jumpHeight/2;
				_rigidbody.linearVelocity = vel;
			}
			else {
                ///// Si on change la gravitée du monde, il faut changer la valeur de vel.y ici pour aller avec
				var vel = _rigidbody.linearVelocity;
                vel.y = 0.1962f;
				_rigidbody.linearVelocity = vel;
            }

		}

        public void AirControl()
        {
            if (isGrounded) return;
            if (!jumpFwdCondition) return;

            var velY = transform.forward * jumpForward * speed;
            velY.y = _rigidbody.linearVelocity.y;
            var velX = transform.right * jumpForward * direction;
            velX.x = _rigidbody.linearVelocity.x;            

            if (jumpAirControl)
            {
                if (isStrafing)
                {
                    _rigidbody.linearVelocity = new Vector3(velX.x, velY.y, _rigidbody.linearVelocity.z);
                    var vel = transform.forward * (jumpForward * speed) + transform.right * (jumpForward * direction);
                    _rigidbody.linearVelocity = new Vector3(vel.x, _rigidbody.linearVelocity.y, vel.z);
                }
                else
                {
                    var vel = transform.forward * (jumpForward * speed);
                    _rigidbody.linearVelocity = new Vector3(vel.x, _rigidbody.linearVelocity.y, vel.z);
                }
            }
            else
            {
                var vel = transform.forward * (jumpForward);
                _rigidbody.linearVelocity = new Vector3(vel.x, _rigidbody.linearVelocity.y, vel.z);
			}
        }

        protected bool jumpFwdCondition
        {
            get
            {
                Vector3 p1 = transform.position + _capsuleCollider.center + Vector3.up * -_capsuleCollider.height * 0.5F;
                Vector3 p2 = p1 + Vector3.up * _capsuleCollider.height;
                return Physics.CapsuleCastAll(p1, p2, _capsuleCollider.radius * 0.5f, transform.forward, 0.6f, groundLayer).Length == 0;
            }
        }

        #endregion

        #region Ground Check

        void CheckGround()
        {
            CheckGroundDistance();

            // change the physics material to very slip when not grounded or maxFriction when is
            if (isGrounded && input == Vector2.zero)
                _capsuleCollider.material = maxFrictionPhysics;
            else if (isGrounded && input != Vector2.zero)
                _capsuleCollider.material = frictionPhysics;
			else if(swimming)
				_capsuleCollider.material = frictionPhysics;
			else
                _capsuleCollider.material = slippyPhysics;

            var magVel = (float)System.Math.Round(new Vector3(_rigidbody.linearVelocity.x, 0, _rigidbody.linearVelocity.z).magnitude, 2);
            magVel = Mathf.Clamp(magVel, 0, 1);

            var groundCheckDistance = groundMinDistance;
            if (magVel > 0.25f) groundCheckDistance = groundMaxDistance;

            // clear the checkground to free the character to attack on air                
            var onStep = StepOffset();

            if (groundDistance <= 0.05f)
            {
				if (swimming)
					swimming = false;
				isGrounded = true;
                doubleJump = 0;
				if(!swimming)
					Sliding();
            }
            else
            {
                if (groundDistance >= groundCheckDistance)
                {
                    isGrounded = false;
                    // check vertical velocity
                    verticalVelocity = _rigidbody.linearVelocity.y;
                    // apply extra gravity when falling
                    if (!swimming)
                    {
                        if (!onStep && !isJumping)
                            _rigidbody.AddForce(transform.up * extraGravity * Time.deltaTime, ForceMode.VelocityChange);
                        else if (!onStep && !isJumping)
                        {
                            _rigidbody.AddForce(transform.up * (extraGravity * 2 * Time.deltaTime), ForceMode.VelocityChange);
                        }
                    }
                  //  else
                   // {
                       // if (!onStep && !isJumping)
                           // _rigidbody.AddForce(transform.up * -0.8f * Time.deltaTime, ForceMode.VelocityChange);
                       // else if (!onStep && !isJumping)
                        //{
                           // _rigidbody.AddForce(transform.up * (-0.8f * 2 * Time.deltaTime), ForceMode.VelocityChange);
                       // }
                   // }
                }
            }

        }

        void CheckGroundDistance()
        {
            if (_capsuleCollider != null)
            {
                // radius of the SphereCast
                float radius = _capsuleCollider.radius * 0.9f;
                var dist = 10f;
                // position of the SphereCast origin starting at the base of the capsule
                Vector3 pos = transform.position + Vector3.up * (_capsuleCollider.radius);
                // ray for RayCast
                Ray ray1 = new Ray(transform.position + new Vector3(0, colliderHeight / 2, 0), Vector3.down);
                // ray for SphereCast
                Ray ray2 = new Ray(pos, -Vector3.up);
                // raycast for check the ground distance
                if (Physics.Raycast(ray1, out groundHit, colliderHeight / 2 + 2f, groundLayer))
                    dist = transform.position.y - groundHit.point.y;
                // sphere cast around the base of the capsule to check the ground distance
                if (Physics.SphereCast(ray2, radius, out groundHit, _capsuleCollider.radius + 2f, groundLayer))
                {
                    // check if sphereCast distance is small than the ray cast distance
                    if (dist > (groundHit.distance - _capsuleCollider.radius * 0.1f))
                        dist = (groundHit.distance - _capsuleCollider.radius * 0.1f);
                }
                groundDistance = (float)System.Math.Round(dist, 2);
            }
        }

        float GroundAngle()
        {
            var groundAngle = Vector3.Angle(groundHit.normal, Vector3.up);
            return groundAngle;
        }      

        void Sliding()
        {
            var onStep = StepOffset();
            var groundAngleTwo = 0f;
            RaycastHit hitinfo;
            Ray ray = new Ray(transform.position, -transform.up);

            if (Physics.Raycast(ray, out hitinfo, 1f, groundLayer))
            {
                groundAngleTwo = Vector3.Angle(Vector3.up, hitinfo.normal);
            }

            if (GroundAngle() > slopeLimit + 1f && GroundAngle() <= 85 &&
                groundAngleTwo > slopeLimit + 1f && groundAngleTwo <= 85 &&
                groundDistance <= 0.05f && !onStep)
            {
                isSliding = true;
                isGrounded = false;
                var slideVelocity = (GroundAngle() - slopeLimit) * 2f;
                slideVelocity = Mathf.Clamp(slideVelocity, 0, 10);
                _rigidbody.linearVelocity = new Vector3(_rigidbody.linearVelocity.x, -slideVelocity, _rigidbody.linearVelocity.z);
            }
            else
            {
                isSliding = false;
                isGrounded = true;
            }
        }

        bool StepOffset()
        {
            if (input.sqrMagnitude < 0.1 || !isGrounded) return false;

            var _hit = new RaycastHit();
            var _movementDirection = isStrafing && input.magnitude > 0 ? (transform.right * input.x + transform.forward * input.y).normalized : transform.forward;
            Ray rayStep = new Ray((transform.position + new Vector3(0, stepOffsetEnd, 0) + _movementDirection * ((_capsuleCollider).radius + 0.05f)), Vector3.down);

            if (Physics.Raycast(rayStep, out _hit, stepOffsetEnd - stepOffsetStart, groundLayer) && !_hit.collider.isTrigger)
            {
                if (_hit.point.y >= (transform.position.y) && _hit.point.y <= (transform.position.y + stepOffsetEnd))
                {
                    var _speed = isStrafing ? Mathf.Clamp(input.magnitude, 0, 1) : speed;
                    var velocityDirection = isStrafing ? (_hit.point - transform.position) : (_hit.point - transform.position).normalized;
                    _rigidbody.linearVelocity = velocityDirection * stepSmooth * (_speed * (velocity > 1 ? velocity : 1));
                    return true;
                }
            }
            return false;
        }

        #endregion

        #region Camera Methods

        public virtual void RotateToTarget(Transform target)
        {
            if (target)
            {
                Quaternion rot = Quaternion.LookRotation(target.position - transform.position);
                var newPos = new Vector3(transform.eulerAngles.x, rot.eulerAngles.y, transform.eulerAngles.z);
                targetRotation = Quaternion.Euler(newPos);
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(newPos), strafeRotationSpeed * Time.deltaTime);
            }
        }

        /// <summary>
        /// Update the targetDirection variable using referenceTransform or just input.Rotate by word  the referenceDirection
        /// </summary>
        /// <param name="referenceTransform"></param>
        public virtual void UpdateTargetDirection(Transform referenceTransform = null)
        {
            if (referenceTransform)
            {
                var forward = keepDirection ? referenceTransform.forward : referenceTransform.TransformDirection(Vector3.forward);
                forward.y = 0;

                forward = keepDirection ? forward : referenceTransform.TransformDirection(Vector3.forward);
                forward.y = 0; //set to 0 because of referenceTransform rotation on the X axis

                //get the right-facing direction of the referenceTransform
                var right = keepDirection ? referenceTransform.right : referenceTransform.TransformDirection(Vector3.right);

                // determine the direction the player will face based on input and the referenceTransform's right and forward directions
                targetDirection = input.x * right + input.y * forward;
            }
            else
                targetDirection = keepDirection ? targetDirection : new Vector3(input.x, 0, input.y);
        }

        #endregion

    }
}