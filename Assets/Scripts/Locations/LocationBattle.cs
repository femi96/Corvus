using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LocationBattle : Location {

  private int id;
  private Party enemyParty;

  public LocationBattle() {
    id = Random.Range(0, 1000);
    GenerateEnemy();
  }

  public override string GetName() {
    return "Battle " + id;
  }

  public override void Visit(Map map) {
    // initiate battle here

    map.StartBattle(map.player.party, enemyParty);
  }

  public override void Clear(int result) {
    if (result == 0) {
      // end run
    } else {
      // continue or reward
    }
  }

  private void GenerateEnemy() {
    Monster[] p = new Monster[6];

    for (int i = 0; i < 6; i += 1) {
      p[i] = new Monster("Enemy" + i);
    }

    enemyParty = new Party(p);
  }
}
