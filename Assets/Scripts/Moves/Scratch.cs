﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scratch : Move {

  private const float actDuration = 0.4f;
  private Unit unit;
  private List<Unit> targetsHit;
  private Tile[] targetTiles;

  private bool effectHappened;

  public override void Setup(Unit unit) {
    this.unit = unit;
    targetTiles = new Tile[] { unit.currentTile.neighbors[Random.Range(0, unit.currentTile.neighbors.Count)] };
    targetsHit = new List<Unit>();
    effectHappened = false;
  }

  public override bool Step(float actionTime) {
    float a = Mathf.Min(actionTime / actDuration, 1.0f);

    if (a >= 0.25f) {
      foreach (Tile actTile in targetTiles) {
        if (!effectHappened) {
          GameObject effectGo = Unit.Instantiate(MovePrefabs.instance.scratchPrefab, MovePrefabs.container);
          effectGo.transform.position = actTile.transform.position;
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
    if (unit != this.unit && !targetsHit.Contains(unit)) {
      unit.DealDamage(GetDamage());
      targetsHit.Add(unit);
    }
  }

  private float GetDamage() {
    return 10 + unit.energy;
  }
}
