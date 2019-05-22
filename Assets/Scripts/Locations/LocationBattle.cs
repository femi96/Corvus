using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LocationBattle : Location {

  private int id;

  public LocationBattle() {
    id = Random.Range(0, 1000);
  }

  public override string GetName() {
    return "Battle " + id;
  }

  public override void Visit(Player player) {
    // initiate battle here

    BattleLoader.GenerateEnemy();
    BattleLoader.AllyParty = player.party;
  }

  public override void Clear(int result) {
    if (result == 0) {
      // end run
    }
  }
}
