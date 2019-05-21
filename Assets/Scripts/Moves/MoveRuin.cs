using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveRuin : Move {

  public override string GetName() {
    return "Ruin";
  }

  public override void Act(Monster user, Monster[] targets, int stage) {
    if (stage == 0) {
      foreach (Monster target in targets) {
        target.DealDamage(GetPower() * GetScale(user));
      }
    }

    if (stage == 1) {
      foreach (Monster target in targets) {
        target.ApplyStatus(new Status(Attr.Wis, 1));
      }
    }
  }

  public override float GetPower() {
    return 2.0f;
  }

  public override float GetScale(Monster user) {
    return user.GetAttribute(Attr.Wis);
  }

  public override TargetType[] GetTargetTypes() {
    return new TargetType[] { TargetType.Single, TargetType.Self };
  }
}
