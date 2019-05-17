using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move {

  public string Name() {
    return "Tackle";
  }

  public void Act(Monster user, Monster target) {
    float damage = user.str * 2;

    if (target != null)
      target.DealDamage(damage);
  }
}
