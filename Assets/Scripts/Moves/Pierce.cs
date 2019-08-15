using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pierce : Move {

  private const float actDuration = 1f;
  private List<Unit> targetsHit;
  private Tile[] targetTiles;

  private bool energyPaid;
  private bool effectHappened;

  public override void Setup(Unit unit) {
    user = unit;
    targetTiles = new Tile[] { user.attackTarget.currentTile };
    targetsHit = new List<Unit>();
    energyPaid = false;
    effectHappened = false;
  }

  public override bool Step(float actionTime) {
    float a = Mathf.Min(actionTime / actDuration, 1.0f);

    if (!energyPaid) {
      if (user.currentEnergy >= EnergyCost()) {
        user.currentEnergy -= EnergyCost();
        energyPaid = true;
      } else
        return false;
    }

    if (a >= 0.2f) {
      foreach (Tile actTile in targetTiles) {
        if (!effectHappened) {
          effectHappened = true;
          GameObject effectGo = Unit.Instantiate(MovePrefabs.instance.piercePrefab, MovePrefabs.container);
          effectGo.transform.position = user.transform.position;
          Vector3 vel = actTile.transform.position - user.currentTile.transform.position;
          effectGo.GetComponent<EffectMover>().velocity = vel * 2f;
          effectGo.GetComponent<EffectDelegate>().methodToCall = OnHit;
          effectGo.GetComponent<Timeout>().duration = 1.5f * actDuration;
        }
      }
    }

    if (a >= 1.0f) {
      return true;
    }

    return false;
  }

  private void OnHit(Unit unit) {
    if (user.team != unit.team && !targetsHit.Contains(unit)) {
      bool crit = false;
      float critDamage = user.monster.CritMod();

      if (Random.Range(0f, 1f) < GetCritChance())
        crit = true;

      unit.DealDamage(GetDamage(), DamageType.Physical, crit, critDamage);
      user.currentEnergy += GetEnergyGain();
      targetsHit.Add(unit);
    }
  }

  private float GetDamage() {
    return Damage() * user.monster.GetAttribute(Attribute.Wis);
  }

  public override float Range() { return 10; }

  public override float Damage() { return 40f; }

  public override float CritChance() { return 0.2f; }

  public override float EnergyGain() { return 0; }

  public override int EnergyCost() { return 50; }
}
