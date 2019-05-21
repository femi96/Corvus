using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TargetType {
  Single, Multi
}

public abstract class Move {

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

  /* Abstract/virtual functions for specific overwrites */

  public virtual string GetName() {
    return "NULL";
  }

  public virtual float GetPower() {
    return 2.0f;
  }

  public virtual float GetScale(Monster user) {
    return user.attrs[Attr.Str];
  }

  public virtual TargetType GetTargetType() {
    return TargetType.Single;
  }
}
