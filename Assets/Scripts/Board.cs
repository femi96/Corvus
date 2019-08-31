using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BattleState { Off, On };

public class Board : TileHolder {

  public List<Unit> units;
  public int unitLimit;
  public Team[] teams;
  private int[] teamCounts;
  public BattleState battleState;
  public bool debugToggleBattle;
  public bool debugLoopBattle;

  public Transform tileContainer;

  void Start() {
    clickSelection = GetComponent<ClickSelection>();
    BoardSetup();
  }

  private void BoardSetup() {
    battleState = BattleState.Off;
    tiles = new List<Tile>();
    units = new List<Unit>();

    foreach (Transform child in tileContainer) {
      Tile tile = child.GetComponent<Tile>();

      if (tile != null) {
        tile.tileName = "Fren" + tiles.Count;
        tiles.Add(tile);
        tile.tileHolder = this;
      }
    }

    foreach (Tile tile in tiles) {
      tile.SetNeighbors(tiles);
    }

    teamCounts = new int[teams.Length];
  }

  public void StartBattle() {

    battleState = BattleState.On;
  }

  private void EndBattle() {

    foreach (Transform child in MovePrefabs.container)
      Destroy(child.gameObject);

    foreach (Unit unit in units) {
      unit.ResetUnit();
      unit.MoveToTile(unit.currentTile);
    }

    battleState = BattleState.Off;
  }

  void Update() {
    BattleStep();

    if (debugToggleBattle) {
      if (battleState == BattleState.Off)
        StartBattle();
      else
        EndBattle();

      debugToggleBattle = false;
    }
  }

  private void BattleStep() {
    if (battleState == BattleState.Off)
      return;

    // Call effects that take place during battle
    foreach (Unit unit in units) {
      unit.BattleTimeStep();
    }

    TryBattleDone();
  }

  private void TryBattleDone() {
    List<int> alive = new List<int>();

    foreach (Unit unit in units) {
      if (unit.IsAlive() && !alive.Contains(unit.team))
        alive.Add(unit.team);
    }

    if (alive.Count == 1) {
      int winner = alive[0];
      Debug.Log("Team " + winner + " wins");
      EndBattle();

      if (debugLoopBattle)
        StartBattle();
    }
  }

  public bool TryAddUnit(Unit unit, bool ignoreLimit = false) {
    if (teamCounts[unit.team] < unitLimit || ignoreLimit) {
      teamCounts[unit.team] += 1;
      units.Add(unit);
      return true;
    }

    return false;
  }

  public void RemoveUnit(Unit unit) {
    teamCounts[unit.team] -= 1;
    units.Remove(unit);
  }

  public List<Tile> GetTilesInRange(Tile centerTile, float range) {
    Vector3 pos = centerTile.transform.position;
    List<Tile> tilesInRange = new List<Tile>();

    foreach (Tile tile in tiles)
      if ((tile.transform.position - pos).magnitude <= range)
        tilesInRange.Add(tile);

    return tilesInRange;
  }

  public List<Tile> GetShortestPathTo(Tile srcTile, List<Tile> dstTiles) {
    Tile curTile = srcTile;
    Queue<Tile> frontier = new Queue<Tile>();
    Dictionary<Tile, List<Tile>> paths = new Dictionary<Tile, List<Tile>>();

    frontier.Enqueue(curTile);
    paths.Add(curTile, new List<Tile>());

    while (frontier.Count > 0) {
      curTile = frontier.Dequeue();

      foreach (Tile nextTile in curTile.neighbors) {
        if (!paths.ContainsKey(nextTile) && nextTile.unit == null) {
          frontier.Enqueue(nextTile);
          List<Tile> nextPath = new List<Tile>(paths[curTile]);
          nextPath.Add(nextTile);
          paths.Add(nextTile, nextPath);

          if (dstTiles.Contains(nextTile))
            return paths[nextTile];
        }
      }
    }

    return paths[srcTile];
  }
}
