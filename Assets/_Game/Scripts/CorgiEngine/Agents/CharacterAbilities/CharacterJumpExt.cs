using UnityEngine;
using MoreMountains.CorgiEngine;
using MoreMountains.Tools;
using Ameba.CorgiEngine.Extensions;
using Ameba.Tools;

namespace Ameba.CorgiEngine
{

    /// <summary>
	/// This class extends from <c>CharacterJump</c> adding controls to authorize jumping in specific MovementStates
	/// </summary>
	[MMHiddenProperties("AbilityStopFeedbacks")]
    [AddComponentMenu("Ameba/CorgiEngine/CharacterAbilities/CharacterJump Ext")]
    [RequireComponent(typeof(CharacterDash))]
    public class CharacterJumpExt : CharacterJump
    {

        [Header("Dash Jump")]
        [Tooltip("The dash force factor applied to the dash-jump.")]
        [Range(0.1f, 2f)]
        public float DashForceFactor = 0.6f;

        protected bool _jumpFromDash = false;
        protected CharacterDash _characterDash = null;
        protected bool _jumpFromJetpack = false;
        protected bool _jumpFromPushing = false;

        public override string HelpBoxText() { return "This component extends the CharacterJump ability adding controls to authorize jumping in specific MovementStates"; }

        protected override void Initialization()
        {
            base.Initialization();
            _characterDash = _character?.FindAbility<CharacterDash>();
        }

        /// <summary>
        /// Extends <code>CharacterJump.ProcessAbility</code> to apply additional forces, like dash-jumping
        /// </summary>
        public override void ProcessAbility()
        {
            base.ProcessAbility();
            //if we're jumping, we process MovementStates
            if (this.IsCurrentState(CharacterStates.MovementStates.Jumping))
            {
                ProcessMovementStates();
            }
        }

        /// <summary>
        /// Per frame, performs additional checks on MovementStates
        /// </summary>
        protected virtual void ProcessMovementStates()
        {
            if (_jumpFromDash)
            {
                Vector2 _dashDirection = _character.IsFacingRight ? Vector2.right : Vector2.left;
                _controller.AddHorizontalForce(_dashDirection.x * _characterDash.DashForce * DashForceFactor);
            }
        }

        /// <summary>
        /// Overrides <c>CharacterJump.EvaluateJumpConditions</c> removing the checks over <c>CharacterStates.MovementStates</c>. 
        /// If you want to prevent jumping on MovementStates like Dashing, Jetpacking or Pushing, add them into the <c>BlockingMovementStates</c> property in the Unity Editor.
        /// <para>Overrides <see cref="CharacterJump.EvaluateJumpConditions"/></para>
        /// </summary>
        /// <returns><c>true</c>, if jump conditions was evaluated, <c>false</c> otherwise.</returns>
        /// 
        protected override bool EvaluateJumpConditions() {
            bool eval = base.EvaluateJumpConditions();
            if (!eval)
            {
                eval = EvalMovementStatesConditions();
            }
            return eval;
        }

        /// <summary>
        /// Evaluate MovementStates jumping conditions
        /// </summary>
        /// <returns></returns>
        protected virtual bool EvalMovementStatesConditions()
        {
            if (AbilityAuthorized && JumpAuthorized)
            {
                if (this.IsCurrentState(CharacterStates.MovementStates.Dashing))
                {
                    JumpFromDash();
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Sets conditions to Jump while dashing
        /// </summary>
        protected virtual void JumpFromDash()
        {
            _controller.State.IsCollidingBelow = (_controller.StandingOn != null);
            _characterDash.StopDash();
            _jumpFromDash = true;
        }

    public override void JumpStop()
        {
            base.JumpStop();
            SetMovementStatesFlags();
        }

        /// <summary>
        /// Resets MovementStates flags
        /// </summary>
        protected virtual void SetMovementStatesFlags()
        {
            _jumpFromDash = _jumpFromDash && !CanJumpStop;
            _jumpFromJetpack = _jumpFromJetpack && !CanJumpStop;
            _jumpFromPushing = _jumpFromPushing && !CanJumpStop;
        }
    }
}
