using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Party {

  public int maxSize = 3;
  public List<Monster> monsters;

  public Party() {
    monsters = new List<Monster>();

    for (int i = 0; i < maxSize; i++) {
      if (Random.Range(0f, 1f) > 0.5f)
        monsters.Add(new Ashire());
      else
        monsters.Add(new Shen());
    }
  }
}
