using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveSweep : Move {

  public override string GetName() {
    return "Sweep";
  }

  public override void Act(Monster user, Monster[] targets, int stage) {
    if (stage == 0) {
      foreach (Monster target in targets) {
        target.DealDamage(GetPower() * GetScale(user));
      }
    }

    if (stage == 1) {
      foreach (Monster target in targets) {
        target.DealDamage(GetPower() * GetScale(user));
      }
    }
  }

  public override float GetPower() {
    return 1.0f;
  }

  public override float GetScale(Monster user) {
    return user.GetAttribute(Attr.Str);
  }

  public override TargetType[] GetTargetTypes() {
    return new TargetType[] { TargetType.Multi, TargetType.Single };
  }
}
