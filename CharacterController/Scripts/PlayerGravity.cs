using UnityEngine;


namespace WoP.CharacterControl
{
    /// <summary>
    /// 
    /// </summary>
    [DefaultExecutionOrder(CharacterControllerVelocity.ExecutionOrder-20)]
    [RequireComponent(typeof(CharacterController))]
    public class PlayerGravity : MonoBehaviour, IGravity
    {
        public UpdateModes Mode = UpdateModes.FixedUpdate;
        [Tooltip("Scaling factor applied to Unity's Physics gravity constant.")]
        public float GravityScale = 1.0f;
        [Tooltip("Maximum speed that can be obtained due to gravity.")]
        public float TerminalFallSpeed = 200.0f;
        [Tooltip("How long, in seconds, after falling is a jump still able to be performed.")]
        public float GroundedFudgeTime = 0.1f;
        [Tooltip("How long can we go without the CharacterControler reporting a grounded state before we have to check ourselves using raycasts?")]
        public float FallTimeThreshold = 0.15f;

        [Tooltip("How far to scan when checking for nearby ground to approximate grounded state.")]
        public float GroundCheckDist = 0.2f;
        [Tooltip("Radius size of the scan when checking for nearby ground to approximate grounded state.")]
        public float GroundCheckRadius = 0.5f;
        [Tooltip("Offset to start the raycast from when doing nearby grounded checks.")]
        public Vector3 GroundCheckOffset = new Vector3(0.0f, 0.1f, 0.0f);
        [Tooltip("Layers to check against when raycasting for nearby grounded states.")]
        public LayerMask Layers;
        

        Vector3 Gravity;
        CharacterController Controller;
        IVelocityAccumulator VelAcc;
        Transform Trans;
        float StartFallTime;
        float LastGroundedTime;


        public bool GravityEnabled { get; set; } = true;

        public bool IsGrounded
        {
            get
            {
                return Controller.isGrounded;
            }
        }

        public bool IsGroundedWithinThreshold
        {
            get
            {
                if (!WasGroundedRecently)
                    return Physics.SphereCast(new Ray(Trans.position + GroundCheckOffset, Vector3.down), GroundCheckRadius, GroundCheckDist, Layers, QueryTriggerInteraction.Ignore);
                else return true;
            }
        }

        /// <summary>
        /// Helper tool for determining if we are falling for a significantly amount of time or if the controller
        /// is likely reporting a fall due to travling on bumpy or sloped surfaces.
        /// </summary>
        /// <returns></returns>
        public bool WasGroundedRecently
        {
            get
            {
                if (Controller.isGrounded)
                {
                    StartFallTime = 0;
                    return false;
                }

                if (StartFallTime == 0)
                {
                    StartFallTime = Time.time;
                    return false;
                }
                else if (Time.time - StartFallTime < FallTimeThreshold)
                    return true;

                return false;
            }
        }

        /// <summary>
        /// For a short period after falling starts froma  grounded state, can we still react as though we are still grounded?
        /// </summary>
        public bool IsGroundedFudged
        {
            get
            {
                return Time.time - LastGroundedTime < GroundedFudgeTime || IsGroundedWithinThreshold;
            }
        }

        public Vector3 GravityVelocity
        {
            get => Gravity;
            set => Gravity = value;
        }

        void Awake()
        {
            Controller = GetComponent<CharacterController>();
            VelAcc = GetComponent<IVelocityAccumulator>();
            Trans = transform;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="dt"></param>
        public void Step(float dt)
        {
            if (!GravityEnabled)
                return;

            if(IsGrounded)
            {
                LastGroundedTime = Time.time;
                Gravity = new Vector3(0, -0.5f, 0); //need a tiny bit of gravity for CharacterController.isGrounded to actually work
            }
            else
            {
                Gravity += Physics.gravity * GravityScale * dt;
                if (Gravity.y > TerminalFallSpeed)
                    Gravity.y = TerminalFallSpeed;
            }
        }

        public void Update()
        {
            if (Mode == UpdateModes.Update)
            {
                Step(Time.deltaTime);
                VelAcc.AddVelocity(GravityVelocity);
            }
        }

        public void LateUpdate()
        {
            if (Mode == UpdateModes.LateUpdate)
            {
                Step(Time.deltaTime);
                VelAcc.AddVelocity(GravityVelocity);
            }
        }

        public void FixedUpdate()
        {
            if (Mode == UpdateModes.FixedUpdate)
            {
                Step(Time.deltaTime);
                VelAcc.AddVelocity(GravityVelocity);
            }
        }
    }
}
