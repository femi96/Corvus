using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ashire : Monster {

  public Ashire() : base()  {
    baseMoves.Add(new Scratch());
    specMoves.Add(new Flamethrower());
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

    attributes[Attribute.Agi] = 4;
    attributes[Attribute.Wis] = 4;
    attributes[Attribute.Wil] = 5;
  }

  public override Affinity[] GetAffinity() {
    return new Affinity[1] { Affinity.Fei };
  }
}