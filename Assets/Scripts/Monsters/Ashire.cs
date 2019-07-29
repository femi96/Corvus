using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ashire : Monster {

  public Ashire() : base()  {
    baseMoves.Add(new Scratch());
    baseMoves.Add(new Tackle());
  }

  public override GameObject GetPrefab() {
    return MonsterPrefabs.instance.ashirePrefab;
  }
}
