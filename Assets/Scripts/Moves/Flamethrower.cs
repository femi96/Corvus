using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flamethrower : Move {

  private const float actDuration = 5f;
  private Dictionary<Unit, float> targetsHit;
  private Tile targetTile;

  private float effectTime;
  private float flameFreq = 1f / 16f;

  public override void Setup(Unit unit) {
    user = unit;
    targetTile = user.attackTarget.currentTile;
    targetsHit = new Dictionary<Unit, float>();
    energyPaid = false;
    effectTime = 0.1f;
  }

  public override bool Step(float actionTime) {
    float a = Mathf.Min(actionTime / actDuration, 1.0f);

    if (!EnoughEnergy(this, user))
      return true;

    if (a >= effectTime && effectTime < 0.9f) {
      effectTime += flameFreq;
      NewEffect();
    }

    if (a >= 1.0f) {
      return true;
    }

    return false;
  }

  private void NewEffect() {
    GameObject effectGo = Unit.Instantiate(MovePrefabs.instance.flamePrefab, MovePrefabs.container);
    effectGo.transform.position = user.transform.position;
    Vector3 dir = (targetTile.transform.position - user.currentTile.transform.position).normalized;
    Vector3 sph = Random.onUnitSphere;
    Vector3 circ = sph - Vector3.Dot(sph, dir) * dir;
    circ *= 0.25f;
    Vector3 dirOffset = (targetTile.transform.position + circ - user.currentTile.transform.position).normalized;
    effectGo.GetComponent<EffectMover>().velocity = 3f * dirOffset;
    effectGo.GetComponent<EffectMover>().acceleration = -1f * dirOffset;
    effectGo.GetComponent<EffectDelegate>().methodToCall = OnHit;
    effectGo.GetComponent<Timeout>().duration = 3f;
  }

  private void OnHit(Unit unit) {
    float hitDeltaSec = actDuration * flameFreq;

    if (user.team != unit.team) {
      if (!targetsHit.ContainsKey(unit)) {
        SteadyDamage(this, user, unit, DamageType.Magical);
        targetsHit.Add(unit, Time.time);
      } else if (targetsHit[unit] <= Time.time - hitDeltaSec) {
        SteadyDamage(this, user, unit, DamageType.Magical);
        targetsHit[unit] = Time.time;
      }
    }
  }

  public override float GetDamage() {
    return Damage() * user.monster.GetAttribute(Attribute.Wis) * flameFreq;
  }

  public override bool UseAttackSpeed() {
    return false;
  }

  public override float Range() { return 4f; }

  public override float Damage() { return 200f; }

  public override float CritChance() { return 0.1f; }

  public override float EnergyGain() { return 0f; }

  public override int EnergyCost() { return 50; }
}
