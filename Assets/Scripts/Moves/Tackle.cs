using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tackle : Move {

  private const float actDuration = 2f;
  private List<Unit> targetsHit;
  private Tile[] targetTiles;

  private bool effectHappened;

  public override void Setup(Unit unit) {
    user = unit;
    targetTiles = new Tile[] { user.attackTarget.currentTile };
    targetsHit = new List<Unit>();
    effectHappened = false;
  }

  public override bool Step(float actionTime) {
    float a = Mathf.Min(actionTime / actDuration, 1.0f);

    if (a >= 0.2f) {
      foreach (Tile actTile in targetTiles) {
        if (!effectHappened) {
          effectHappened = true;
          GameObject effectGo = Unit.Instantiate(MovePrefabs.instance.tacklePrefab, MovePrefabs.container);
          effectGo.transform.position = user.transform.position;
          Vector3 vel = (actTile.transform.position - user.currentTile.transform.position).normalized;
          effectGo.GetComponent<EffectMover>().velocity = vel * 2f;
          effectGo.GetComponent<EffectDelegate>().methodToCall = OnHit;
          effectGo.GetComponent<Timeout>().duration = 0.5f * actDuration;
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
      StandardDamage(this, user, unit, DamageType.Physical);
      targetsHit.Add(unit);
    }
  }

  public override float GetDamage() {
    return Damage() * user.monster.GetAttribute(Attribute.Str);
  }

  public override float Range() { return 2f; }

  public override float Damage() { return 50f; }

  public override float CritChance() { return 0.1f; }

  public override float EnergyGain() { return 10f; }

  public override int EnergyCost() { return 0; }
}
