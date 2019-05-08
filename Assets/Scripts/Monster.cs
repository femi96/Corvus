using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EType {
  Normal, Fire
}

public class Monster {

  // Biographic info
  public string name;
  public int speciesID;
  public EType type;
  public int monID;

  // Attributes
  public int str;
  public int agi;
  public int wis;
  public int rea;
  public int vit;
  public int wil;

  // Derives stats
  public int maxHealth;
  public int initiative;
  public int evasion;
  public int vitalResist;
  public int mentalResist;
  public int normalResist;

  // Battle stats
  public int currentInitiative = 0;
  public int location = 0;
  public GameObject body;

  public Monster(string name = "None", int species = 0) {
    this.name = name;
    this.speciesID = species;
    this.monID = Random.Range(0, 10);

    type = EType.Normal;
    str = 5;
    agi = 5;
    wis = 5;
    rea = 5;
    vit = 5;
    wil = 5;

    CalculateCurrentStats();
  }

  private void CalculateCurrentStats() {
    maxHealth = 20 + (vit * 3) + str;
    initiative = 0 + rea;
    evasion = 0 + agi + rea;
    vitalResist = vit;
    mentalResist = wis;
    normalResist = wil;
  }
}
