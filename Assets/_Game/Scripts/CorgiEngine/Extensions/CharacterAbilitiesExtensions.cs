using MoreMountains.CorgiEngine;

namespace Ameba.CorgiEngine.Extensions
{
    public static class CharacterAbilitiesExtension
    {
        public static bool IsWallJumping(this CharacterAbility ability) => IsCurrentState(ability, CharacterStates.MovementStates.WallJumping);
        public static bool WasWallJumping(this CharacterAbility ability) => IsPreviousState(ability, CharacterStates.MovementStates.WallJumping);

        //Shortcut to compare current MovementState
        public static bool IsCurrentState(this CharacterAbility ability,
            CharacterStates.MovementStates movementState)
        {
            Character _character = ability.gameObject.GetComponentInParent<Character>();
            return _character.MovementState.CurrentState == movementState;
        }

        //Shortcut to compare previous MovementState
        public static bool IsPreviousState(this CharacterAbility ability,
            CharacterStates.MovementStates movementState)
        {
            Character _character = ability.gameObject.GetComponentInParent<Character>();
            return _character.MovementState.PreviousState == movementState;
        }

        //Shortcut to compare current CharacterConditions
        public static bool IsCurrentCondition(this CharacterAbility ability,
            CharacterStates.CharacterConditions condition)
        {
            Character _character = ability.gameObject.GetComponentInParent<Character>();
            return _character.ConditionState.CurrentState == condition;
        }

        //Shortcut to compare previous CharacterConditions
        public static bool IsPreviousCondition(this CharacterAbility ability,
            CharacterStates.CharacterConditions condition)
        {
            Character _character = ability.gameObject.GetComponentInParent<Character>();
            return _character.ConditionState.PreviousState == condition;
        }
    }

}
