using UnityEngine;

namespace WoP.CharacterControl
{
    /// <summary>
    /// Demonstrates a simple use-case for the character controller components.
    /// Input should always be processed *before* the individual controller components.
    /// </summary>
    [DefaultExecutionOrder(CharacterControllerVelocity.ExecutionOrder-100)]
    public class PlayerInputReader : MonoBehaviour
    {
        public IGravity Gravity;
        public IJumper Jumper;
        public IMover Mover;
        public IAimer Aimer;

        static bool Quitting;



        private void Awake()
        {
            Application.quitting += HandleQuit;

            //NOTE: Silly me, forgot that default unity inspector apparently doesn't show
            //controls for interfaces - just hacking this together automagically.
            Gravity = GetComponent<IGravity>();
            Jumper = GetComponent<IJumper>();
            Mover = GetComponent<IMover>();
            Aimer = GetComponent<IAimer>();
        }


        public void Update()
        {
            float x = Input.GetAxis("Horizontal");
            float y = Input.GetAxis("Vertical");
            float ax = 0;// Input.GetAxis("AimH");
            float ay = 0;// Input.GetAxis("AimV");

            Jumper.JumpInput = Input.GetButton("Fire1");
            Mover.InputX = x;
            Mover.InputY = y;
            Aimer.MoveX = x;
            Aimer.MoveY = y;
            Aimer.AimX = ax;
            Aimer.AimY = ay;
            Aimer.Aim(new Vector2(ax, ay)); //this gives us immediate results but we still want to push to the inputs of the aimer as you see above
        }

        void HandleQuit()
        {
            Quitting = true;
        }


    }
}
