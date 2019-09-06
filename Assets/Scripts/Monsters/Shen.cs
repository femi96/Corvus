using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shen : Monster {

  public Shen() : base()  {
    baseMoves.Add(new Scratch());
    baseMoves.Add(new Tackle());
    specMoves.Add(new Bang());
    specMoves.Add(new Pierce());
  }

  public override string GetName() {
    return "Shen";
  }

  public override GameObject GetPrefab() {
    return MonsterPrefabs.instance.shenPrefab;
  }

  public override void SetAttributes() {
    foreach (Attribute attr in System.Enum.GetValues(typeof(Attribute)))
      attributes[attr] = 3;

    attributes[Attribute.Str] = 5;
    attributes[Attribute.Vit] = 4;
    attributes[Attribute.Rea] = 4;
  }
}
