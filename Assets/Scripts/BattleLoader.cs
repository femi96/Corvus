using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BattleLoader {

  public static Party AllyParty;
  public static Party EnemyParty;

  public static void GenerateParties() {
    Monster[] p0 = new Monster[6];
    Monster[] p1 = new Monster[6];

    for (int i = 0; i < 6; i += 1) {
      p0[i] = new Monster("Ally" + i);
      p1[i] = new Monster("Enemy" + i);
    }

    AllyParty = new Party(p0);
    EnemyParty = new Party(p1);
  }
}