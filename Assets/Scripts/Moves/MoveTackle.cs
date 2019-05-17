using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTackle : Move {

  public override string GetName() {
    return "Tackle";
  }

  public override float GetPower() {
    return 2.0f;
  }

  public override float GetScale(Monster user) {
    return user.str;
  }

  public override TargetType GetTargetType() {
    return TargetType.Single;
  }
}
