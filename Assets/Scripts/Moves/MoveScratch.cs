using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveScratch : Move {

  public override string GetName() {
    return "Scratch";
  }

  public override float GetPower() {
    return 2.0f;
  }

  public override float GetScale(Monster user) {
    return user.attrs[Attr.Agi];
  }

  public override TargetType GetTargetType() {
    return TargetType.Single;
  }
}
