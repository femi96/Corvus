using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player {

  public Party party;

  public Player() {
    GenerateParty();
  }

  private void GenerateParty() {
    Monster[] p = new Monster[6];

    for (int i = 0; i < 6; i += 1) {
      p[i] = new Monster("PlayerMember" + i);
    }

    party = new Party(p);
  }
}