using MoreMountains.CorgiEngine;
using UnityEngine;
using UnityEngine.U2D.IK;

namespace Ameba.CorgiEngine
{
    /// <summary>
    /// This class allows for a 2D character to grab its current weapon's handles, and look wherever it's aiming.
    /// </summary>
    [AddComponentMenu("Ameba/Corgi Extensions/WeaponsList/Weapon IK 2D")]
    public class WeaponIK2D : CorgiMonoBehaviour
    {
        /// the solver to use for the IK
        [Tooltip("The solver to use for the IK")]
        public Solver2D _solver2d = null;

        protected Animator _animator;
        protected Character _character = null;

        protected Transform _equippedWeaponTarget = null;


        protected virtual void Start()
        {
            _animator = this.gameObject.GetComponent<Animator>();
            _character = this.gameObject.GetComponent<Character>();
        }

        protected virtual void OnAnimatorIK(int layerIndex)
        {
            if (_animator == null) return;

            //if the IK is active, set the position and rotation to the character primary movement.
            if (_character != null && _equippedWeaponTarget != null)
            {
                Vector2 movement = _character.LinkedInputManager.PrimaryMovement;
                float angle = Mathf.Atan2(movement.y, movement.x) * Mathf.Rad2Deg;

                //TODO : obtain target position based on angles 
                Vector3 targetPosition = _equippedWeaponTarget.position;

                for (int i = 0; i < _solver2d.chainCount; i++)
                {
                    var chain = _solver2d.GetChain(i);
                    chain.target.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
                }
                _solver2d.transform.position = Vector3.Lerp(_solver2d.transform.position, targetPosition, Time.deltaTime);
            }
        }

        /// <summary>
        /// Binds the character hands to the equipped weapon target.
        /// Any of the hands can be null, in which case the weapon will be held with the other hand.
        /// </summary>
        /// <param name="leftHand">Left hand.</param>
        /// <param name="rightHand">Right hand.</param>
        public virtual void SetHandles(Transform leftHand, Transform rightHand) => _equippedWeaponTarget = (leftHand != null) ? leftHand : rightHand;


    }
}
