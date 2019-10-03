using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyTileHolder : TileHolder {

  public bool setupDone = false;

  public List<Unit> units;
  private int teamInt = 0;

  public void Setup(Party party, int team) {

    units = new List<Unit>();
    clickSelection = FindObjectsOfType<ClickSelection>()[0];
    teamInt = team;

    // Setup tiles
    tiles = new List<Tile>();
    GameObject baseTile = null;

    foreach (Transform child in transform) {
      baseTile = child.gameObject;
    }

    int j = 0;

    for (int i = 1; i < party.maxSize; i++) {
      if (j < 0)
        j = -j;
      else
        j = -(j + 1);

      Vector3 newPos = new Vector3(j * TileHolder.TileWidth, 0, 0) + baseTile.transform.position;
      Instantiate(baseTile, newPos, baseTile.transform.rotation, transform);
    }

    foreach (Transform child in transform) {
      Tile tile = child.GetComponent<Tile>();

      if (tile != null) {
        tile.tileName = "Team" + tiles.Count;
        tiles.Add(tile);
        tile.tileHolder = this;
      }
    }

    // Setup units
    int unitPlaceCounter = 0;

    foreach (Monster m in party.monsters) {
      GameObject go = Instantiate(MonsterPrefabs.instance.unitPrefab, transform);
      Unit u = go.GetComponent<Unit>();
      u.Setup(m);
      units.Add(u);
      u.MoveToTile(tiles[unitPlaceCounter]);
      unitPlaceCounter++;
      u.team = teamInt;
    }

    setupDone = true;
  }

  public override bool CanMoveTo(int team, Tile tile) {
    return team == teamInt;
  }
}
