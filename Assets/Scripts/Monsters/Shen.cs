using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shen : Monster {

  public Shen() : base()  {
    baseMoves.Add(new Scratch());
    baseMoves.Add(new Tackle());
  }

  public override GameObject GetPrefab() {
    return MonsterPrefabs.instance.shenPrefab;
  }
}
