
using MoreMountains.CorgiEngine;
using UnityEngine;

namespace Ameba.CorgiEngine {

  /// <summary>
  /// Extends the functionality of <see cref="ProjectileWeapon"/> by providing events for tracking
  /// the weapon and spawned projectiles lifecycle and interactions. 
  /// </summary>>
  public class ProjectileWeaponExt : ProjectileWeapon {

    public delegate void OnProjectileDelegate(ProjectileWeapon weapon, GameObject projectile);
    public event OnProjectileDelegate OnProjectileSpawned;
    public event OnProjectileDelegate OnProjectileHit;
    public event OnProjectileDelegate OnProjectileHitDamageable;
    public event OnProjectileDelegate OnProjectileHitNonDamageable;
    public event OnProjectileDelegate OnProjectileKill;

    public delegate void OnWeaponDelegate(ProjectileWeapon weapon);
    public event OnWeaponDelegate OnWeaponHit;
    public event OnWeaponDelegate OnWeaponHitDamageable;
    public event OnWeaponDelegate OnWeaponHitNonDamageable;
    public event OnWeaponDelegate OnWeaponKill;
    public event OnWeaponDelegate OnWeaponMiss;

    public override GameObject SpawnProjectile(Vector3 spawnPosition, int projectileIndex, int totalProjectiles, bool triggerObjectActivation = true) {
      // Call the base method to spawn the projectile
      GameObject projectileGO = base.SpawnProjectile(spawnPosition, projectileIndex, totalProjectiles, triggerObjectActivation);
      // Invoke the OnProjectileSpawned event if the projectile was successfully spawned
      if (projectileGO != null) {
        OnProjectileSpawned?.Invoke(this, projectileGO);

        if (projectileGO.TryGetComponent<Projectile>(out Projectile projectile)) {

          if (projectile.TryGetComponent<DamageOnTouch>(out DamageOnTouch damageOnTouch)) {
            // Subscribe to the DamageOnTouch events to track hits and kills
            damageOnTouch.OnHit += () => OnProjectileDOTHit(projectile);
            damageOnTouch.OnHitDamageable += () => OnProjectileDOTHitDamageable(projectile);
            damageOnTouch.OnHitNonDamageable += () => OnProjectileDOTHitNonDamageable(projectile);
            damageOnTouch.OnKill += () => OnProjectileDOTKill(projectile);
          }
          else {
            Debug.LogWarning($"Projectile {projectile.name} spawned by {this.name} does not have a DamageOnTouch component. No hit events will be tracked.");
          }
        }
      }
      return projectileGO;
    }

    public override void WeaponHit() {
      base.WeaponHit();
      OnWeaponHit?.Invoke(this);
    }

    public override void WeaponHitDamageable() {
      base.WeaponHitDamageable();
      OnWeaponHitDamageable?.Invoke(this);
    }

    public override void WeaponHitNonDamageable() {
      base.WeaponHitNonDamageable();
      OnWeaponHitNonDamageable?.Invoke(this);
    }

    public override void WeaponKill() {
      base.WeaponKill();
      OnWeaponKill?.Invoke(this);
    }
    public override void WeaponMiss() {
      base.WeaponMiss();
      OnWeaponMiss?.Invoke(this);
    }

    private void OnProjectileDOTHit(Projectile projectile) => OnProjectileHit?.Invoke(this, projectile.gameObject);

    private void OnProjectileDOTHitDamageable(Projectile projectile) => OnProjectileHitDamageable?.Invoke(this, projectile.gameObject);

    private void OnProjectileDOTHitNonDamageable(Projectile projectile) => OnProjectileHitNonDamageable?.Invoke(this, projectile.gameObject);

    private void OnProjectileDOTKill(Projectile projectile) => OnProjectileKill?.Invoke(this, projectile.gameObject);

  }
}