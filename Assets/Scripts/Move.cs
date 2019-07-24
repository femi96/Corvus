using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Move {

  public abstract void Setup(Unit unit);

  // Return true when finished
  public abstract bool Step(float actionTime);
}