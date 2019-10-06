using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum BattleState { Off, On };

public class Board : TileHolder {

  public List<Unit> units;
  public int unitLimit;
  public PartyTileHolder[] parties;
  private int[] teamUnitCounts;
  private int[] winCounts;
  private int round;
  public BattleState battleState;
  public Player player;

  public bool debugToggleBattle;
  public bool debugLoopBattle;

  public Tile[] teamTiles0;
  public Tile[] teamTiles1;

  private GameObject startBattleButton;
  private Text unitCountText;
  private Text winCountText;
  private Text roundText;

  private bool setupDone = false;

  void Start() {}

  private void BoardSetup() {
    clickSelection = FindObjectsOfType<ClickSelection>()[0];
    player = FindObjectsOfType<Player>()[0];
    battleState = BattleState.Off;
    tiles = new List<Tile>();
    units = new List<Unit>();

    foreach (Transform child in transform) {
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

    teamUnitCounts = new int[parties.Length];
    winCounts = new int[parties.Length];

    round = 1;

    // Get UI pointers
    startBattleButton = UIPointers.instance.startBattleButton;
    unitCountText = UIPointers.instance.unitCountText;
    winCountText = UIPointers.instance.winCountText;
    roundText = UIPointers.instance.roundText;

    unitCountText.text = teamUnitCounts[0] + " / " + unitLimit;
    winCountText.text = winCounts[0] + " vs " + winCounts[1];
    roundText.text = "Round: " + round;

    setupDone = true;
  }

  public void StartBattle() {
    if (units.Count == 0)
      return;

    PlaceEnemyUnits();

    battleState = BattleState.On;
    startBattleButton.SetActive(false);
  }

  private void EndBattle() {
    round += 1;
    roundText.text = "Round: " + round;

    foreach (Transform child in MovePrefabs.container)
      Destroy(child.gameObject);

    foreach (Unit unit in units) {
      unit.ResetUnit();
      unit.MoveToTile(unit.currentTile);
    }

    battleState = BattleState.Off;
    startBattleButton.SetActive(true);
  }

  void Update() {

    if (!setupDone) {
      bool partySetupDone = true;

      foreach (PartyTileHolder p in parties)
        partySetupDone = partySetupDone && p.setupDone;

      if (partySetupDone)
        BoardSetup();

      return;
    }

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
      winCounts[winner] += 1;
      winCountText.text = winCounts[0] + " vs " + winCounts[1];
      int damage = 5;
      player.DealDamage(damage);

      if (winner == 0)
        Debug.Log("Round won!");
      else
        Debug.Log("Round lost! Take " + damage + " damage!");

      if (winCounts[winner] == 3) {
        Debug.Log("Team " + winner + " wins match best of 5, end encounter");
        EndBattle();
        EndEncounter();
        return;
      }

      EndBattle();

      if (debugLoopBattle)
        StartBattle();
    }
  }

  public bool TryAddUnit(Unit unit, bool ignoreLimit = false) {
    if (teamUnitCounts[unit.team] < unitLimit || ignoreLimit) {
      teamUnitCounts[unit.team] += 1;
      units.Add(unit);
      unitCountText.text = teamUnitCounts[0] + " / " + unitLimit;
      return true;
    }

    return false;
  }

  public void RemoveUnit(Unit unit) {
    teamUnitCounts[unit.team] -= 1;
    units.Remove(unit);
    unitCountText.text = teamUnitCounts[0] + " / " + unitLimit;
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

  public override bool CanMoveTo(int team, Tile tile) {
    if (team == 0)
      return Array.Exists(teamTiles0, e => e == tile);
    else if (team == 1)
      return Array.Exists(teamTiles1, e => e == tile);

    return false;
  }

  private void PlaceEnemyUnits() {
    // Move all units to bench
    foreach (Unit unit in parties[1].units) {
      foreach (Tile tile in parties[1].tiles) {
        if (tile.unit == null) {
          unit.MoveToTile(tile);
          break;
        }
      }
    }

    // Randomly select units up to limit
    List<Unit> unitsToPlace = new List<Unit>();
    List<Unit> unitsCouldPlace = new List<Unit>();

    foreach (Unit unit in parties[1].units) {
      unitsCouldPlace.Add(unit);
    }

    while (unitsToPlace.Count < unitLimit && unitsCouldPlace.Count > 0) {
      int i = UnityEngine.Random.Range(0, unitsCouldPlace.Count);
      unitsToPlace.Add(unitsCouldPlace[i]);
      unitsCouldPlace.RemoveAt(i);
    }

    // Randomly place on board
    List<Tile> tilesAvailable = new List<Tile>();

    foreach (Tile tile in teamTiles1) {
      tilesAvailable.Add(tile);
    }

    foreach (Unit unit in unitsToPlace) {
      int j = UnityEngine.Random.Range(0, tilesAvailable.Count);
      unit.MoveToTile(tilesAvailable[j]);
      tilesAvailable.RemoveAt(j);
    }
  }

  private void EndEncounter() {
    foreach (Unit unit in units)
      Destroy(unit.uiHover);

    foreach (Unit unit in parties[0].units)
      Destroy(unit.uiHover);

    foreach (Unit unit in parties[1].units)
      Destroy(unit.uiHover);

    FindObjectsOfType<Floor>()[0].EndEncounter();
  }
}
