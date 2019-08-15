using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ashire : Monster {

  public Ashire() : base()  {
    baseMoves.Add(new Scratch());
    baseMoves.Add(new Tackle());
    specMoves.Add(new Bang());
    specMoves.Add(new Pierce());
  }

  public override string GetName() {
    return "Ashire";
  }

  public override GameObject GetPrefab() {
    return MonsterPrefabs.instance.ashirePrefab;
  }

  public override void SetAttributes() {
    foreach (Attribute attr in System.Enum.GetValues(typeof(Attribute)))
      attributes[attr] = 3;

    attributes[Attribute.Agi] = 5;
    attributes[Attribute.Wis] = 4;
    attributes[Attribute.Wil] = 4;
  }
}
