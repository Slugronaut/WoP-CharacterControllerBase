using UnityEngine;
using UnityEngine.Events;

namespace WoP.CharacterControl
{
    /// <summary>
    /// 
    /// </summary>
    [DefaultExecutionOrder(CharacterControllerVelocity.ExecutionOrder-15)]
    public class PlayerJumping : MonoBehaviour, IJumper
    {
        public UpdateModes Mode = UpdateModes.FixedUpdate;
        public float JumpPower = 10;
        [Tooltip("The amount to reduce the jump velocity to when the jump input is let go early (while the entity is still moving upward).")]
        public float JumpCancelFactor = 0.25f;
        public UnityEvent OnJump;

        bool CancelJump;
        Vector3 OldGravity;
        IVelocityAccumulator VelAcc;
        IGravity Gravity;
        bool JumpLock; //locks the input so we can't jump again until the button is released

        public bool JumpEnabled { get; set; } = true;

        public bool JumpInput { private get; set; }

        public bool IsAirborn => throw new System.NotImplementedException();

        public bool IsJumping => throw new System.NotImplementedException();

        public bool IsFalling => throw new System.NotImplementedException();

        public bool JumpedThisFrame { get; private set; }

        public bool FallingFromJump => throw new System.NotImplementedException();

        public JumpEvent OnLanded => throw new System.NotImplementedException();

        public float JumpWindow => throw new System.NotImplementedException();

        public void ResetJumpState()
        {
            throw new System.NotImplementedException();
        }

        public Vector3 JumpVelocity { get; }

        

        void Awake()
        {
            VelAcc = GetComponent<IVelocityAccumulator>();
            Gravity = GetComponent<IGravity>();
            if (Gravity == null)
                throw new UnityException("PlayerJumping requires an IGravity component attached to the same GameObject.");
        }

        public void Step(float dt)
        {
            JumpedThisFrame = false;

            if (!JumpEnabled)
            {
                JumpInput = false;
                return;
            }

            if (!Gravity.IsGrounded)
            {
                if (!JumpInput && Gravity.GravityVelocity.y > 0)
                    CancelJump = true;

                if (Gravity.IsGroundedFudged)
                {
                    if (JumpInput && !JumpLock)
                        ApplyJumpImpulse();
                }
                //else JumpLock = Jump; //stops us from jumping once we become grounded again if we were holding the jump button
            }
            else
            {
                if (JumpInput && !JumpLock)
                    ApplyJumpImpulse();
                //JumpLock = JumpInput;
            }
            JumpLock = JumpInput;

        }

        void ApplyJumpImpulse()
        {
            JumpedThisFrame = true;
            OldGravity = Gravity.GravityVelocity;
            VelAcc.AddVelocity(-Gravity.GravityVelocity);
            Gravity.GravityVelocity = new Vector3(0.0f, JumpPower, 0.0f);
            JumpLock = true;
            OnJump.Invoke();
            VelAcc.AddVelocity(Gravity.GravityVelocity);
        }

        void ApplyFinalForces()
        {
            Step(Time.deltaTime);

            if (CancelJump)
            {
                Gravity.GravityVelocity *= JumpCancelFactor;
                CancelJump = false;
            }
        }

        public void Update()
        {
            if (Mode == UpdateModes.Update)
                ApplyFinalForces();
        }

        public void LateUpdate()
        {
            if (Mode == UpdateModes.LateUpdate)
                ApplyFinalForces();
        }

        public void FixedUpdate()
        {
            if (Mode == UpdateModes.FixedUpdate)
                ApplyFinalForces();
        }

        
    }
    
}
