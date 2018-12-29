using UnityEngine;


namespace WoP.CharacterControl
{
    /// <summary>
    /// Used to accumulate a total velocity that is applied to a character controller.
    /// </summary>
    [DefaultExecutionOrder(ExecutionOrder)]
    [RequireComponent(typeof(CharacterController))]
    public class CharacterControllerVelocity : MonoBehaviour, IVelocityAccumulator
    {
        public const int ExecutionOrder = 1000;

        public UpdateModes Mode = UpdateModes.FixedUpdate;
        CharacterController Controller;
        Vector3 Velocity;

        void Awake()
        {
            Controller = GetComponent<CharacterController>();
        }

        public void AddVelocity(Vector3 vel)
        {
            Velocity += vel;
        }

        public void OverrideVelocity(Vector3 vel)
        {
            Velocity = vel;
        }

        public void Update()
        {
            if (Mode == UpdateModes.Update)
            {
                Controller.Move(Velocity * Time.deltaTime);
                Velocity = Vector3.zero;
            }

        }

        public void LateUpdate()
        {
            if (Mode == UpdateModes.LateUpdate)
            {
                Controller.Move(Velocity * Time.deltaTime);
                Velocity = Vector3.zero;
            }
        }

        public void FixedUpdate()
        {
            if (Mode == UpdateModes.FixedUpdate)
            {
                Controller.Move(Velocity * Time.deltaTime);
                Velocity = Vector3.zero;
            }
        }
    }

    
}
