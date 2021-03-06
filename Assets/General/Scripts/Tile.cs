﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GroundType { Normal };

public class Tile : MonoBehaviour {

  public TileHolder tileHolder;

  public List<Tile> neighbors;  // List of neighbor tiles created using tile position
  public Unit unit;             // Unit currently on tile. Null when moving off, X when X is moving on
  public GroundType ground;     // Ground effect

  public string tileName = "None";

  void Start() {}

  void OnMouseOver() {
    if (Input.GetMouseButtonDown(0))
      tileHolder.clickSelection.OnClickTile(this);
  }

  private void UpdateRep() {
    // Change visual of gameobject based on ground type
  }

  public void SetNeighbors(List<Tile> tiles) {
    neighbors = new List<Tile>();

    foreach (Tile tile in tiles) {
      if (tile == this)
        continue;

      float dist = (tile.transform.position - transform.position).magnitude;

      if (dist < 2)
        neighbors.Add(tile);
    }
  }

  public float DistanceTo(Tile other) {
    return (transform.position - other.transform.position).magnitude;
  }

  void Update() {}
}
