using UnityEngine;


namespace WoP.CharacterControl
{
    /// <summary>
    /// 
    /// </summary>
    [DefaultExecutionOrder(CharacterControllerVelocity.ExecutionOrder-10)]
    public class PlayerMovement : MonoBehaviour, IMover
    {
        public UpdateModes Mode = UpdateModes.FixedUpdate;
        public float RunSpeed = 10;
        public float Acceleration = 15;
        [Tooltip("Can be used to scale the motion on a particular axis. Used primarily as a means of making z-axis motion visually appear to be as fast as x-axis motion even when heavy perspective and forshortening is applied.")]
        public Vector3 MotionAxisScaling = Vector3.one;
        [Tooltip("If motionscaling is applied, should it be re-normalized?")]
        public bool NormalizedMotionScaling;
        public bool ClampVel = true;
        public MovementTypes MoveType;

        Transform MainCameraTrans;
        IVelocityAccumulator VelAcc;
        Vector3 LastMoveDir;
        Vector3 CurrRotVel;

        public bool MoveEnabled { get; set; } = true;
        public float InputX { private get; set; }
        public float InputY { private get; set; }



        void Awake()
        {
            VelAcc = GetComponent<IVelocityAccumulator>();
            MainCameraTrans = Camera.main.transform;
        }

        public void Step(float dt)
        {
            float x = MoveEnabled ? InputX : 0;
            float y = MoveEnabled ? InputY : 0;
            Vector3 moveDir = Vector3.ClampMagnitude(new Vector3(x, 0, y), 1);
            //smooth the velocity change when movement angle changes
            moveDir= Vector3.SmoothDamp(LastMoveDir, moveDir, ref CurrRotVel, 1 / Acceleration);
            LastMoveDir = moveDir;

            if (moveDir.sqrMagnitude < 0.01) moveDir = Vector3.zero;
            Vector3 vel;
            if (NormalizedMotionScaling)
                vel = Vector3.Scale(MotionAxisScaling, moveDir).normalized * RunSpeed;
            else vel = RunSpeed * Vector3.Scale(MotionAxisScaling, moveDir);

            if (ClampVel)
                vel = Vector3.ClampMagnitude(vel, RunSpeed);
            vel = ControllerUtils.TransformByFacingSpace(vel, MainCameraTrans, MoveType);
            VelAcc.AddVelocity(vel);

            #region Apply Final Motion
            /*
            if (Halt)
            {
                //apply negative accel
                if (HaltVel.sqrMagnitude > 0.1f)
                    HaltVel = Vector3.SmoothDamp(HaltVel, Vector3.zero, ref HaltVel, 1 / (Acceleration * 5));
                else HaltVel = Vector3.zero;
                if (!DisableGravity)
                {
                    HaltVel += Gravity;
                    vel += Gravity;
                }
                Control.Move(HaltVel * dt);
            }
            else
            {
                HaltVel = vel;
                if (!DisableGravity)
                    vel += Gravity;
                Control.Move(vel * dt);

                //this section controls the direction faced
                if (!LockDirection)
                {
                    #region Aiming Mode Handling
                    if (AimType == AimTypes.SingleStick)
                    {
                        if (RotationType == RotationTypes.Bilateral)
                        {
                            if (InputX < -DirectionThreshold)
                                LastSide = -1;
                            else if (InputX > DirectionThreshold)
                                LastSide = 1;
                            Trans.forward = Vector3.right * LastSide;
                        }
                        else if (RotationType == RotationTypes.Omnidirectional)
                        {
                            if (moveDirection.sqrMagnitude > MotionThreshold)
                            {
                                LastForward = new Vector3(moveDirection.x, 0, moveDirection.z).normalized;
                                Trans.forward = LastForward;
                            }
                        }
                        else Debug.Log("Unsupported player rotation mode.");
                    }
                    else if (AimType == AimTypes.TwinStick)
                    {
                        if (Time.time - LastAimTime > AimDirectionDelay)
                            Aim(new Vector2(x, y));
                        //NOTE: This is now called directly from the PlayerInputReader so that we rotate *before* processing shooting commands.
                        else Aim(new Vector2(ax, ay));
                    }
                    #endregion
                }
            }
            */
            #endregion

        }

        public void Update()
        {
            if (Mode == UpdateModes.Update)
                Step(Time.deltaTime);
        }

        public void LateUpdate()
        {
            if (Mode == UpdateModes.LateUpdate)
                Step(Time.deltaTime);
        }

        public void FixedUpdate()
        {
            if (Mode == UpdateModes.FixedUpdate)
                Step(Time.deltaTime);
        }
    }
}
