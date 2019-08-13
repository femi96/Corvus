using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BattleState { Off, On };

public class Board : MonoBehaviour {

  private List<Tile> tiles;
  public Unit[] units;
  public BattleState battleState;
  public bool debugToggleBattle;
  public bool debugLoopBattle;

  public ClickSelection clickSelection;
  public Transform tileContainer;

  void Start() {
    clickSelection = GetComponent<ClickSelection>();
    BoardSetup();
  }

  private void BoardSetup() {
    battleState = BattleState.Off;
    tiles = new List<Tile>();

    foreach (Transform child in tileContainer) {
      Tile tile = child.GetComponent<Tile>();

      if (tile != null) {
        tile.tileName = "Fren" + tiles.Count;
        tiles.Add(tile);
        tile.board = this;
      }
    }

    Debug.Log("Tiles gathered. Tile count: " + tiles.Count);
    Debug.Log("Units gathered. Unit count: " + units.Length);

    foreach (Tile tile in tiles) {
      tile.SetNeighbors(tiles);
    }

    foreach (Unit unit in units) {
      unit.board = this;
    }
  }

  private void StartBattle() {

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
}
