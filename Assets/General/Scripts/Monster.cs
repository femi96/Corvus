using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Attribute { Str, Agi, Wis, Vit, Rea, Wil };

public abstract class Monster {

  /* Stores individual, persistant monster information
  */

  public List<Move> baseMoves;
  public List<Move> specMoves;
  public Dictionary<Attribute, int> attributes;

  public Monster() {
    baseMoves = new List<Move>();
    specMoves = new List<Move>();
    attributes = new Dictionary<Attribute, int>();
    SetAttributes();
  }

  public abstract string GetName();

  public abstract GameObject GetPrefab();

  public abstract void SetAttributes();

  public int GetAttribute(Attribute attr) {
    return attributes[attr];
  }

  // TODO: Make these effected by status effects
  public int MaxHealth() {
    return 50 +
           37 * attributes[Attribute.Vit] +
           14 * attributes[Attribute.Str] +
           5 * attributes[Attribute.Agi];
  }

  public float MoveSpeed() {
    return 1f +
           0.05f * attributes[Attribute.Rea];
  }

  public float AttackSpeed() {
    return 1f +
           0.05f * attributes[Attribute.Agi];
  }

  public float Endurance() {
    return 10f +
           2f * attributes[Attribute.Vit] +
           0.4f * attributes[Attribute.Str];
  }

  public float Resistance() {
    return 10f +
           2f * attributes[Attribute.Wil] +
           0.3f * attributes[Attribute.Wis] +
           0.2f * attributes[Attribute.Vit];
  }

  public float Evasion() {
    return 10f +
           3f * attributes[Attribute.Rea];
  }

  public float EnergyMod() {
    return 1f +
           0.1f * attributes[Attribute.Wis] +
           0.025f * attributes[Attribute.Wil];
  }

  public float CritMod() {
    return 1f +
           0.1f * attributes[Attribute.Wil] +
           0.025f * attributes[Attribute.Rea];
  }

  // moves

  // types
}