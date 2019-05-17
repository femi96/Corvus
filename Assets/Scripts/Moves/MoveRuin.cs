using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveRuin : Move {

  public override string GetName() {
    return "Ruin";
  }

  public override float GetPower() {
    return 2.0f;
  }

  public override float GetScale(Monster user) {
    return user.wis;
  }

  public override TargetType GetTargetType() {
    return TargetType.Single;
  }
}
