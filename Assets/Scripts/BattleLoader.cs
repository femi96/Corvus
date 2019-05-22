using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BattleLoader {

  public static Party AllyParty;
  public static Party EnemyParty;

  public static void GenerateEnemy() {
    Monster[] p = new Monster[6];

    for (int i = 0; i < 6; i += 1) {
      p[i] = new Monster("Enemy" + i);
    }

    EnemyParty = new Party(p);
  }
}