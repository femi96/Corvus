using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Move {

  public Unit user;

  // Every setup must assign user
  public abstract void Setup(Unit unit);

  // Return true when finished with move
  public abstract bool Step(float actionTime);


  public abstract float Damage();

  public abstract float CritChance();

  public float GetCritChance() {
    return CritChance() * user.monster.CritMod();
  }

  public abstract float EnergyGain();

  public int GetEnergyGain() {
    return Mathf.RoundToInt(EnergyGain() * user.monster.EnergyMod());
  }

  public abstract int EnergyCost();
}