using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Encounter {

  public Party party;
  public GameObject boardPrefab;

  public Encounter() {
    party = new Party();
    boardPrefab = BoardPrefabs.instance.board1Prefab;
  }
}
