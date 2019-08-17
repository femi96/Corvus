using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Move {

  public Unit user;
  public bool energyPaid;

  // Every setup must assign user
  public abstract void Setup(Unit unit);

  // Return true when finished with move
  public abstract bool Step(float actionTime);


  public abstract float Range();

  public abstract float Damage();

  public abstract float GetDamage();

  public abstract float CritChance();

  public float GetCritChance() {
    return CritChance() * user.monster.CritMod();
  }

  public float GetCritDamage() {
    return user.monster.CritMod();
  }

  public abstract float EnergyGain();

  public int GetEnergyGain() {
    return Mathf.RoundToInt(EnergyGain() * user.monster.EnergyMod());
  }

  public abstract int EnergyCost();

  public static bool EnoughEnergy(Move move, Unit user) {
    if (move.energyPaid)
      return true;

    if (user.currentEnergy >= move.EnergyCost()) {
      user.currentEnergy -= move.EnergyCost();
      move.energyPaid = true;
      return true;
    }

    return false;
  }

  public static void StandardDamage(Move move, Unit user, Unit target, DamageType type) {

    float damage = move.GetDamage();
    damage *= Random.Range(0.95f, 1.05f);

    // Crit
    bool crit = Random.Range(0f, 1f) < move.GetCritChance();

    if (crit)
      damage *= move.GetCritDamage();

    // Evasion
    bool miss = Random.Range(0f, 1f) < target.monster.Evasion() / 100f;

    if (miss)
      damage = 0;

    target.DealDamage(damage, type, crit, miss);

    if (!miss)
      user.currentEnergy += move.GetEnergyGain();
  }
}