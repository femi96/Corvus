using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bang : Move {

  private const float actDuration = 2f;
  private List<Unit> targetsHit;
  private Tile[] targetTiles;

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

    if (!EnoughEnergy(this, user))
      return true;

    if (a >= 0.2f) {
      foreach (Tile actTile in targetTiles) {
        if (!effectHappened) {
          effectHappened = true;
          GameObject effectGo = Unit.Instantiate(MovePrefabs.instance.bangPrefab, MovePrefabs.container);
          effectGo.transform.position = user.transform.position;
          effectGo.GetComponent<EffectMover>().velocity = 0.01f * Vector3.up;
          effectGo.transform.localScale = Vector3.zero;
          effectGo.GetComponent<EffectMover>().expand = Vector3.one / (0.8f * actDuration);
          effectGo.GetComponent<EffectDelegate>().methodToCall = OnHit;
          effectGo.GetComponent<Timeout>().duration = 0.8f * actDuration;
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
      StandardDamage(this, user, unit, DamageType.Magical);
      targetsHit.Add(unit);
    }
  }

  public override float GetDamage() {
    return Damage() * user.monster.GetAttribute(Attribute.Wis);
  }

  public override float Range() { return 2f; }

  public override float Damage() { return 200f; }

  public override float CritChance() { return 0.2f; }

  public override float EnergyGain() { return 0; }

  public override int EnergyCost() { return 50; }
}
