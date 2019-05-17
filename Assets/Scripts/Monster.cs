using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EType {
  Normal, Fire
}

public class Monster {
  /* Public variables may be read but should not be written
    from outside of Monster, with marked exceptions*/

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

  // Moves
  public Move[] moves;

  // Battle stats
  /* Battle script may set these variables */
  public int currentInitiative = 0;
  public int currentHealth = 0;

  // Context for knowing its board position & gameObject body
  /* Battle script may set these variables */
  public int partySide = 0;
  public int boardPos = 0;
  public GameObject body;
  public GameObject healthBar;

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

    CalculateDerivedStats();

    moves = new Move[4];
    moves[0] = new MoveTackle();

    if (Random.Range(0f, 1f) < 0.5f)
      moves[1] = new MoveScratch();

    if (Random.Range(0f, 1f) < 0.5f)
      moves[2] = new MoveRuin();

    if (Random.Range(0f, 1f) < 0.25f)
      moves[3] = new MoveSweep();

    currentHealth = (int)(maxHealth * Random.Range(0.5f, 1f));
  }

  private void CalculateDerivedStats() {
    maxHealth = 20 + (vit * 3) + str;
    initiative = 0 + rea;
    evasion = 0 + agi + rea;
    vitalResist = vit;
    mentalResist = wis;
    normalResist = wil;
  }

  public void DealDamage(float damage) {
    currentHealth -= (int)Mathf.Max(damage, 1f);
    currentHealth = Mathf.Max(currentHealth, 0);
  }
}
