using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floor : MonoBehaviour {

  public Player player;
  public bool load = false;

  void Start() {}

  void Update() {
    if (load) {
      load = false;
      LoadEncounter(new Encounter());
    }
  }

  void LoadEncounter(Encounter encounter) {
    /*
    get party from player
    get party from encounter
    load in board based on encounter
    add party tile holders based on parties
    */
    Party playerParty = player.party;
    Party enemyParty = encounter.party;

    Board board = FindObjectsOfType<Board>()[0];
    PartyTileHolder playerBench = board.parties[0];
    PartyTileHolder enemyBench = board.parties[1];
    playerBench.Setup(playerParty, 0);
    enemyBench.Setup(enemyParty, 1);
  }
}
