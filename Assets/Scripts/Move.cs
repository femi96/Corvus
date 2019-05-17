using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TargetType {
  Single, Multi
}

public class Move {

  public void Act(Monster user, Monster target) {
    Act(user, new Monster[] { target });
  }

  public void Act(Monster user, Monster[] targets) {
    foreach (Monster target in targets) {
      if (target != null)
        target.DealDamage(GetDamage(user));
    }
  }

  public float GetDamage(Monster user) {
    return GetScale(user) * GetPower();
  }

  /* Abstract functions for specific overwrites */

  public string Name() {
    return "NULL";
  }

  public float GetPower() {
    return 2.0f;
  }

  public float GetScale(Monster user) {
    return user.str;
  }

  public TargetType GetTargetType() {
    return TargetType.Single;
  }
}
