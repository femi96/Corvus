using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scratch : Move {

  private const float actDuration = 1f;
  private Unit unit;
  private Tile[] targetTiles;
  private List<Unit> targetsHit;
  private GameObject actGo;

  public override void Setup(Unit unit) {
    this.unit = unit;
    targetTiles = new Tile[] { unit.currentTile.neighbors[Random.Range(0, unit.currentTile.neighbors.Count)] };
    targetsHit = new List<Unit>();
  }

  public override bool Step(float actionTime) {
    float a = Mathf.Min(actionTime / actDuration, 1.0f);

    if (a >= 0.5f) {
      foreach (Tile actTile in targetTiles) {
        if (actTile.unit != null && !targetsHit.Contains(actTile.unit))
          actTile.unit.DealDamage(10);

        if (actGo == null) {
          actGo = Unit.Instantiate(MovePrefabs.instance.scratchPrefab, unit.transform);
          actGo.transform.position = actTile.transform.position;
        }
      }
    }

    if (a >= 1.0f) {
      Unit.Destroy(actGo);
      return true;
    }

    return false;
  }
}
