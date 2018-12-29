using UnityEngine;

namespace WoP.CharacterControl
{
    /// <summary>
    /// 
    /// </summary>
    [DefaultExecutionOrder(CharacterControllerVelocity.ExecutionOrder - 1)]
    public class PlayerAim : MonoBehaviour, IAimer
    {
        [Tooltip("The transform that will be aimed.")]
        public Transform Trans;
        [Tooltip("How long after letting go of the aim stick before movement aims the character.")]
        public float AimDirectionDelay = 1;
        [Tooltip("How much motion must be detected by the aim stick before it registers.")]
        public float MotionThreshold = 0.1f;
        MovementTypes MovementType;


        float LastAimTime;
        Vector3 LastForward;
        Transform MainCameraTrans;

        public bool AimEnabled { get; set; } = true;
        public float AimX { protected get; set; }
        public float AimY { protected get; set; }
        public float MoveX { protected get; set; }
        public float MoveY { protected get; set; }


        void Start()
        {
            if(MainCameraTrans == null)
                MainCameraTrans = Camera.main.transform;

            
        }

        void Update()
        {
            Step(Time.deltaTime);
        }

        void Step(float dt)
        {
            float x = 0;
            float y = 0;
            float ax = 0;
            float ay = 0;
            if (AimEnabled)
            {
                x = MoveX;
                y = MoveY;
                ax = AimX;
                ay = AimY;
            }

            Vector3 aimDirection = Vector3.ClampMagnitude(new Vector3(ax, 0, ay), 1);

            if (Time.time - LastAimTime > AimDirectionDelay)
                AimNonTimerReset(new Vector2(x, y));
            //NOTE: This is now called directly from the PlayerInputReader so that we rotate *before* processing shooting commands.
            else Aim(new Vector2(ax, ay));
        }

        /// <summary>
        /// Can be called externally to set the rotation. It expects a perspective-corrected forward vector.
        /// </summary>
        /// <param name="correctedForward"></param>
        public void Aim(Vector3 correctedForward)
        {
            LastForward = correctedForward;
            Trans.forward = LastForward;
            LastAimTime = Time.time;
        }

        /// <summary>
        /// This can be called externally to handle aiming directly from the player input system.
        /// </summary>
        public void Aim(Vector2 aim)
        {
            var aimDirection = ControllerUtils.TransformByFacingSpace(Vector3.ClampMagnitude(new Vector3(aim.x, 0, aim.y), 1), MainCameraTrans, MovementType);

            if (aimDirection.magnitude > MotionThreshold)
            {
                LastForward = new Vector3(aimDirection.x, 0, aimDirection.z).normalized;
                Trans.forward = LastForward;
                LastAimTime = Time.time;
            }
        }

        /// <summary>
        /// This can be called externally to handle aiming directly from the player input system.
        /// </summary>
        public void AimNonTimerReset(Vector2 aim)
        {
            var aimDirection = ControllerUtils.TransformByFacingSpace(Vector3.ClampMagnitude(new Vector3(aim.x, 0, aim.y), 1), MainCameraTrans, MovementType);

            if (aimDirection.magnitude > MotionThreshold)
            {
                LastForward = new Vector3(aimDirection.x, 0, aimDirection.z).normalized;
                Trans.forward = LastForward;
            }
        }
    }
}
