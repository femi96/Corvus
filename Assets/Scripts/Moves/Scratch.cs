using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scratch : Move {

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

    if (a >= 0.5f) {
      foreach (Tile actTile in targetTiles) {
        if (!effectHappened) {
          effectHappened = true;
          GameObject effectGo = Unit.Instantiate(MovePrefabs.instance.scratchPrefab, MovePrefabs.container);
          Vector3 toTarget = (actTile.transform.position - user.currentTile.transform.position).normalized;

          Vector3 tanDir = (3f * Vector3.down + 3f * toTarget).normalized;
          Vector3 curveDir = -toTarget;
          Vector3 position = user.currentTile.transform.position + 0.5f * toTarget + 2.5f * Vector3.up;

          effectGo.transform.position = position;
          effectGo.GetComponent<EffectMover>().velocity = tanDir * 4f;
          effectGo.GetComponent<EffectMover>().acceleration = curveDir * 4f;
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
    return Damage() * user.monster.GetAttribute(Attribute.Agi);
  }

  public override float Range() { return 2f; }

  public override float Damage() { return 30f; }

  public override float CritChance() { return 0.2f; }

  public override float EnergyGain() { return 5f; }

  public override int EnergyCost() { return 0; }
}
