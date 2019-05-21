using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveSweep : Move {

  public override string GetName() {
    return "Sweep";
  }

  public override float GetPower() {
    return 1.0f;
  }

  public override float GetScale(Monster user) {
    return user.attrs[Attr.Str];
  }

  public override TargetType GetTargetType() {
    return TargetType.Multi;
  }
}
