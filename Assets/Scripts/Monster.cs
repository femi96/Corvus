using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EType {
  Normal, Fire
}

public enum Attr {
  Str, Agi, Wis, Rea, Vit, Wil
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
  private Dictionary<Attr, float> attrs;
  private Dictionary<Attr, int> attrsBase;

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
  public List<Status> statuses;

  // Context for knowing its board position & gameObject body
  /* Battle script may set these variables */
  public int partySide = 0;
  public int boardPos = 0;
  public GameObject body;
  public GameObject healthBar;

  public Monster(string name = "None", int species = 0) {
    this.name = name;
    this.speciesID = species;
    this.monID = Random.Range(0, 100);

    type = EType.Normal;
    attrs = new Dictionary<Attr, float>();
    attrsBase = new Dictionary<Attr, int>();
    statuses = new List<Status>();

    foreach (Attr a in System.Enum.GetValues(typeof(Attr)))
      attrsBase[a] = 5;

    CalculateCurrentStats();

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

  private void CalculateCurrentStats() {
    foreach (Attr a in System.Enum.GetValues(typeof(Attr))) {
      float val = attrsBase[a];
      int stage = 0;

      foreach (Status s in statuses) {
        if (s.GetStatus() == StatusType.StatMod && s.GetStatAttr() == a) {
          stage += s.GetStatMod();
        }
      }

      val *= (2f + Mathf.Max(0, stage)) / (2f + Mathf.Max(0, -stage));
      attrs[a] = val;
    }

    CalculateDerivedStats();
  }

  private void CalculateDerivedStats() {
    maxHealth = Mathf.RoundToInt(20 + (attrs[Attr.Vit] * 3) + attrs[Attr.Str]);
    initiative = Mathf.RoundToInt(0 + attrs[Attr.Rea]);
    evasion = Mathf.RoundToInt(0 + attrs[Attr.Agi] + attrs[Attr.Rea]);
    vitalResist = Mathf.RoundToInt(attrs[Attr.Vit]);
    mentalResist = Mathf.RoundToInt(attrs[Attr.Wis]);
    normalResist = Mathf.RoundToInt(attrs[Attr.Wil]);
  }

  public void ResetForBattle() {
    currentInitiative = 0;
    statuses = new List<Status>();
    CalculateCurrentStats();
  }

  public void DealDamage(float power) {
    float calc = power * Random.Range(0.80f, 1f);
    calc = Mathf.Max(calc, 1f);

    int damage = Mathf.RoundToInt(calc);
    currentHealth -= damage;
    currentHealth = Mathf.Max(currentHealth, 0);

    Debug.Log(name + " takes " + damage + " damage.");
  }

  public void Heal(float amount) {
    int delta = Mathf.RoundToInt(amount);
    currentHealth += delta;
    currentHealth = Mathf.Min(currentHealth, maxHealth);

    Debug.Log(name + " healed by " + delta + ".");
  }

  public float GetAttribute(Attr attr) {
    return attrs[attr];
  }

  public void ApplyStatus(Status status) {
    float temp1 = attrs[status.GetStatAttr()];
    statuses.Add(status);
    CalculateCurrentStats();
    float temp2 = attrs[status.GetStatAttr()];
    Debug.Log(name + " new status " + status.GetName() + ". " + status.GetStatAttr().ToString() + " change " + temp1 + " -> " + temp2 + ".");
  }
}
