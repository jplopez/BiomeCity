using MoreMountains.CorgiEngine;
using UnityEngine;

namespace Ameba.CorgiEngine.Events
{

    /// <summary>
    /// Use this event listener to reset Jumps and Dashes when the character enters WallClinging state
    /// </summary>
    /// 
    [AddComponentMenu("Ameba/Corgi Extensions/EventListeners/WallClingingEventListener")]
    public class WallClingingEventListener : BaseMMEventListener<MMCharacterEvent>
    {
        [Tooltip("If true, CharacterJump.NumberOfJumpsLeft is set to CharacterJump.NumberOfJumps")]
        public bool ResetJumps = true;
        [Tooltip("If true, CharacterDash.SuccessiveDashesLeft is set to CharacterDashes.SuccessiveDashAmount. This flag is ignored if Dashes are unlimited")]
        public bool ResetDash = true;

        protected override bool EvalEventType(MMCharacterEvent evt) => (evt.EventType == MMCharacterEventTypes.WallCling
                && evt.Moment == MMCharacterEvent.Moments.Start);

        protected override void HandleEvent(MMCharacterEvent evt)
        {
            base.HandleEvent(evt);
            Character target = evt.TargetCharacter;
            //reset dashes if flag is true and dashes are limited
            if (ResetDash
                && target.gameObject.TryGetComponent<CharacterDash>(out CharacterDash _characterDash)
                && _characterDash.LimitedDashes)
            {
                _characterDash.SuccessiveDashesLeft = _characterDash.SuccessiveDashAmount;
            }

            //reset jumps if flag is true
            if (ResetJumps
                && target.gameObject.TryGetComponent<CharacterJump>(out CharacterJump _characterJump))
            {
                _characterJump.NumberOfJumpsLeft = _characterJump.NumberOfJumps;
            }
        }
    }
}

