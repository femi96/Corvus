using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TileHolder : MonoBehaviour {

  public List<Tile> tiles;
  public ClickSelection clickSelection;

  // Return if a unit of team can be moved to tile
  public abstract bool CanMoveTo(int team, Tile tile);

  void Start() {

  }
}
