using MoreMountains.CorgiEngine;
using MoreMountains.Tools;
using UnityEngine;

namespace Ameba.CorgiEngine {
    /// <summary>
    /// Extends <c>CharacterDamageDash</c> to support specific force and damage multiplier depending on the direction 
    /// </summary>
    [AddComponentMenu("Ameba/CorgiEngine/CharacterAbilities/Character Damage Dash Ext")]
  public class CharacterDamageDashExt : CharacterDamageDash {

    /// whether or not upwards dashes have an specific DamageOnTouch definition
		[Tooltip(" whether or not upwards dashes have an specific DamageOnTouch definition")]
    public bool UpwardsDamageOnTouch = false;
    /// the DamageOnTouch object to activate when dashing upwards. If false, uses the default TargetDamageOnTouch definition
    [MMCondition("UpwardsDamageOnTouch", true)]
    [Tooltip("the DamageOnTouch object to activate when dashing upwards. If false, uses the default TargetDamageOnTouch definition")]
    public DamageOnTouch UpwardsTargetDamageOnTouch;
    ///  whether or not horizontal dashes have an specific DamageOnTouch definition
    [Tooltip(" whether or not upwards dashes have an specific DamageOnTouch definition")]
    public bool HorizontalDamageOnTouch = false;
    /// the DamageOnTouch object to activate when dashing horizontally. If false, uses the default TargetDamageOnTouch definition
    [MMCondition("HorizontalDamageOnTouch", true)]
    [Tooltip("the DamageOnTouch object to activate when dashing horizontally. If false, uses the default TargetDamageOnTouch definition")]
    public DamageOnTouch HorizontalTargetDamageOnTouch;

    public enum DiagonalDashDamageOptions { Default, Upwards, Horizontal }

    ///what damage on touch should be used to resolve diagonal dashes. This property defaults to the TargetDamageOnTouch definition
    [Tooltip("what damage on touch should be used to resolve diagonal dashes. This property defaults to the TargetDamageOnTouch definition")]
    public DiagonalDashDamageOptions DiagonalDashDamage = DiagonalDashDamageOptions.Default;

    protected DamageOnTouch _defaultDamageOnTouch = null;

    protected virtual void DetermineTargetDamageOnTouch() {
      ComputeDashDirection();
      //default
      TargetDamageOnTouch = _defaultDamageOnTouch;

      //check diagonal case
      if ( Mathf.Abs(_dashDirection.x) > DashDirectionMinThreshold && Mathf.Abs(_dashDirection.y) > DashDirectionMinThreshold) {
        switch (DiagonalDashDamage) {
          case DiagonalDashDamageOptions.Default:
            TargetDamageOnTouch = _defaultDamageOnTouch; break;
          case DiagonalDashDamageOptions.Upwards:
            TargetDamageOnTouch = UpwardsDamageOnTouch ? UpwardsTargetDamageOnTouch : _defaultDamageOnTouch; break;
          case DiagonalDashDamageOptions.Horizontal:
            TargetDamageOnTouch = HorizontalDamageOnTouch ? HorizontalTargetDamageOnTouch : _defaultDamageOnTouch; break;
        }
        //if diagonal fails, we check horizontal and vertical
      }
      else {
        //horizontal
        if (HorizontalDamageOnTouch) {
          TargetDamageOnTouch = (Mathf.Abs(_dashDirection.x) > DashDirectionMinThreshold) ? HorizontalTargetDamageOnTouch : _defaultDamageOnTouch;
        }
        //upwards
        if (UpwardsDamageOnTouch) {
          TargetDamageOnTouch = (Mathf.Abs(_dashDirection.y) > DashDirectionMinThreshold) ? UpwardsTargetDamageOnTouch : _defaultDamageOnTouch;
        }

      }
    }

    protected override void Initialization() {
      base.Initialization();

      //copy the TargetDamageOnTouch value
      if (TargetDamageOnTouch != null) {
        _defaultDamageOnTouch = Instantiate(TargetDamageOnTouch, TargetDamageOnTouch.transform.parent);
      }

      UpwardsTargetDamageOnTouch?.gameObject.SetActive(false);
      HorizontalTargetDamageOnTouch?.gameObject.SetActive(false);
    }

    public override void InitiateDash() {
      DetermineTargetDamageOnTouch();
      base.InitiateDash();
    }

    public override void StopDash() {
      base.StopDash();
      UpwardsTargetDamageOnTouch?.gameObject.SetActive(false);
      HorizontalTargetDamageOnTouch?.gameObject.SetActive(false);
    }
  }
}
