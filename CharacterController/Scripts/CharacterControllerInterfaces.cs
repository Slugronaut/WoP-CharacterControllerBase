using UnityEngine;
using UnityEngine.Events;

namespace WoP.CharacterControl
{
    #region Interfaces
    /*
    public interface ICharacterController
    {
        bool IsGrounded { get; }
        bool IsGroundedWithinThreshold { get; }
    }
    */

    public interface IVelocityAccumulator
    {
        void AddVelocity(Vector3 vel);
        void OverrideVelocity(Vector3 vel);
    }

    public interface IMover
    {
        bool MoveEnabled { get; set; }

        float InputX { set; }
        float InputY { set; }
        //bool LockDirection { get; set; }
    }

    public interface IAimer
    {
        bool AimEnabled { get; set; }

        float AimX { set; }
        float AimY { set; }
        float MoveX { set; }
        float MoveY { set; }
        void Aim(Vector3 precorrectedAimDir);
        void Aim(Vector2 aimDir);
    }

    public interface IAttacker
    {
        bool AttackEnabled { get; set; }
    }

    public interface IGravity
    {
        bool GravityEnabled { get; set; }
        bool IsGrounded { get; }
        bool IsGroundedWithinThreshold { get; }
        bool WasGroundedRecently { get; }
        bool IsGroundedFudged { get; }
        Vector3 GravityVelocity { get; set; }
        void Step(float deltaTime);
    }

    public interface IJumper
    {
        bool JumpEnabled { get; set; }

        bool JumpInput { set; }
        bool IsAirborn { get; }
        bool IsJumping { get; }
        bool IsFalling { get; }
        bool JumpedThisFrame { get; }
        bool FallingFromJump { get; }
        JumpEvent OnLanded { get; }
        void ResetJumpState();
        float JumpWindow { get; }
        Vector3 JumpVelocity { get; }

        void Step(float deltaTime);
    }
#   endregion


    public enum UpdateModes
    {
        Manual,
        Update,
        LateUpdate,
        FixedUpdate,
    }

    /// <summary>
    /// Due to a special-case constrait in WoP, I needed the
    /// ability to 'skew' the movement of the player based on the relative camera angle.
    /// Most projects can ignore this and simply default to using 'Orthagonal'.
    /// </summary>
    public enum MovementTypes
    {
        Orthagonal,
        Oblique,
    }

    #region Events

    [System.Serializable]
    public class JumpEvent : UnityEvent<IJumper> { }
    
    #endregion


    public static class ControllerUtils
    {
        /// <summary>
        /// Applies rotation to a movement vector so that the z-axis motion always appears vertical to a facing-target.
        /// 
        /// This was used in WoP to make vertical motion appear to be vertical on the screen even though the camera
        /// was actually tilted at a 10 degree angle with the view matrix skewed to make the x-axis appear horizontal.
        /// </summary>
        /// <param name="vel"></param>
        /// <param name="faceTarget"></param>
        /// <returns></returns>
        public static Vector3 TransformByFacingSpace(Vector3 vel, Transform faceTarget, MovementTypes type)
        {
            //translate to camera relative motion
            if (vel.sqrMagnitude > 0.01f)
            {
                if (type == MovementTypes.Orthagonal)
                    return (Quaternion.AngleAxis(faceTarget.eulerAngles.y, Vector3.up) * vel);
                else if (type == MovementTypes.Oblique)
                {
                    //this is trickier - we need rotate vertical motion and avoid rotating horizontal
                    //motion but ensure we maintain the same total magnitude.
                    var hor = new Vector3(vel.x, 0);
                    var vert = new Vector3(0, 0, vel.z);
                    var comp = Quaternion.AngleAxis(faceTarget.eulerAngles.y, Vector3.up) * vert;
                    return (comp + hor).normalized * vel.magnitude;
                }
            }

            return vel;
        }
    }

}
