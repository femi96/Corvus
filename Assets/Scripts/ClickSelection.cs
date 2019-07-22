using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickSelection : MonoBehaviour {

  public Unit selectedUnit;
  private Board board;

  public void OnClickUnit(Unit unit) {
    if (selectedUnit == null)
      // Select
      selectedUnit = unit;
    else {
      // Swap
      Tile tempTile = selectedUnit.currentTile;
      selectedUnit.MoveToTile(unit.currentTile);
      unit.MoveToTile(tempTile);
      selectedUnit = null;
    }
  }

  public void OnClickTile(Tile tile) {
    if (selectedUnit != null && board.battleState == BattleState.Off) {
      if (tile.unit != null) {
        // Swap if other
        tile.unit.MoveToTile(selectedUnit.currentTile);
      }

      // Move to tile
      selectedUnit.MoveToTile(tile);
      selectedUnit = null;
    }
  }

  void Start() {
    board = GetComponent<Board>();
  }

  void Update() {}
}