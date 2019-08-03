using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scratch : Move {

  private const float actDuration = 0.5f;
  private List<Unit> targetsHit;
  private Tile[] targetTiles;

  private bool effectHappened;

  public override void Setup(Unit unit) {
    user = unit;
    targetTiles = new Tile[] { user.currentTile.neighbors[Random.Range(0, user.currentTile.neighbors.Count)] };
    targetsHit = new List<Unit>();
    effectHappened = false;
  }

  public override bool Step(float actionTime) {
    float a = Mathf.Min(actionTime / actDuration, 1.0f);

    if (a >= 0.2f) {
      foreach (Tile actTile in targetTiles) {
        if (!effectHappened) {
          effectHappened = true;
          GameObject effectGo = Unit.Instantiate(MovePrefabs.instance.scratchPrefab, MovePrefabs.container);
          effectGo.transform.position = user.transform.position;
          Vector3 vel = actTile.transform.position - user.currentTile.transform.position;
          effectGo.GetComponent<EffectMover>().velocity = vel * 4f;
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
      bool crit = false;
      float critDamage = user.CritMod();

      if (Random.Range(0f, 1f) < CritChance() * user.CritMod())
        crit = true;

      unit.DealDamage(GetDamage(), DamageType.Physical, crit, critDamage);
      targetsHit.Add(unit);
    }
  }

  private float GetDamage() {
    return Damage() * user.GetAttribute(Attribute.Agi);
  }

  public override float Damage() { return 10f; }

  public override float CritChance() { return 0.2f; }
}
