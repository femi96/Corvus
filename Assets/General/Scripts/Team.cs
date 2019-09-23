using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Team : TileHolder {

  public List<Unit> units;
  public int teamSizeLimit;
  public int teamInt = 0;
  private int unitPlaceCounter = 0;

  void Start() {
    TeamSetup();
  }

  private void TeamSetup() {
    clickSelection = FindObjectsOfType<ClickSelection>()[0];
    tiles = new List<Tile>();

    foreach (Transform child in transform) {
      Tile tile = child.GetComponent<Tile>();

      if (tile != null) {
        tile.tileName = "Team" + tiles.Count;
        tiles.Add(tile);
        tile.tileHolder = this;
      }
    }

    teamSizeLimit = tiles.Count;

    foreach (Unit u in units) {
      u.MoveToTile(tiles[unitPlaceCounter]);
      unitPlaceCounter++;
      u.team = teamInt;
    }
  }

  public override bool CanMoveTo(int team, Tile tile) {
    return team == teamInt;
  }
}
