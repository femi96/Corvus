using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Monster {

  public List<Move> baseMoves;
  public List<Move> specMoves;

  public Monster() {
    baseMoves = new List<Move>();
    specMoves = new List<Move>();
  }

  // model
  public abstract GameObject GetPrefab();

  // moves

  // types
}