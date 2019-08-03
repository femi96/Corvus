using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ActionState { Wait, Dead, Moving, Acting };

public enum DamageType { Physical, Magical, True };

public class Unit : MonoBehaviour {

  public Board board;

  public int team = 0;

  private Monster monster;
  private GameObject model;


  public ActionState actionState;
  private float actionTime = 0;

  // Unit stats
  public int currentHealth;
  public int currentEnergy;

  // Moving
  private const float moveDuration = 0.5f;
  public Tile currentTile;
  private Tile nextTile;
  private Tile prevTile;

  // Acting
  private Move move;

  void Start() {
    if (Random.Range(0f, 1f) > 0.5f)
      monster = new Ashire();
    else
      monster = new Shen();

    ResetModel();
    ResetUnit();
  }

  void Update() {

  }

  private void ResetModel() {
    foreach (Transform child in transform)
      Destroy(child.gameObject);

    model = monster.GetPrefab();
    model = Instantiate(model, transform);
  }

  void OnMouseOver() {
    if (Input.GetMouseButtonDown(0))
      board.clickSelection.OnClickUnit(this);
  }

  public void DealDamage(float damage, DamageType type = DamageType.True,
                         bool crit = false, float critDamage = 1f) {

    // Crit
    if (crit)
      damage *= critDamage;

    // Damage reduction from defense
    float reduction = 1f;

    if (type == DamageType.Physical)
      reduction = 1f - 0.05f * monster.Endurance() / (1 + 0.05f * Mathf.Abs(monster.Endurance()));

    if (type == DamageType.Magical)
      reduction = 1f - 0.05f * monster.Resistance() / (1 + 0.05f * Mathf.Abs(monster.Resistance()));

    damage *= reduction;

    // Evasion
    bool miss = Random.Range(0f, 1f) < Evasion();

    if (miss)
      damage = 0;

    // Round damage
    int roundedDamage = Mathf.RoundToInt(damage);

    // Damage effect on health
    int preHealth = currentHealth;
    currentHealth = Mathf.Max(currentHealth - roundedDamage, 0);

    if (miss)
      Debug.Log("Miss!");
    else if (crit)
      Debug.Log("Crit! Unit takes " + roundedDamage + " " + type + " damage. " + preHealth + " -> " + currentHealth);
    else
      Debug.Log("Unit takes " + roundedDamage + " " + type + " damage. " + preHealth + " -> " + currentHealth);

    TryDead();
  }


  public int GetAttribute(Attribute attr) {
    return monster.attributes[attr];
  }

  public float MoveSpeed() {
    return monster.MoveSpeed();
  }

  public float AttackSpeed() {
    return monster.AttackSpeed();
  }

  public float Endurance() {
    return monster.Endurance();
  }

  public float Resistance() {
    return monster.Resistance();
  }

  public float Evasion() {
    return monster.Evasion();
  }

  public float EnergyMod() {
    return monster.EnergyMod();
  }

  public float CritMod() {
    return monster.CritMod(); // TODO: Make this effected by status effects
  }


  private void TryDead() {
    if (currentHealth > 0)
      return;

    Debug.Log("Unit dies");
    ChangeActionState(ActionState.Dead);
  }

  public bool IsAlive() {
    return actionState != ActionState.Dead;
  }

  public void MoveToTile(Tile tile) {
    if (currentTile.unit == this)
      currentTile.unit = null;

    currentTile = tile;
    currentTile.unit = this;
    Vector3 tilePos = currentTile.transform.position;
    transform.position = tilePos + 0.75f * Vector3.up;
  }

  public void ResetUnit() {
    currentHealth = monster.MaxHealth();
    currentEnergy = 0;
    actionState = ActionState.Wait;
  }

  public void ActionStep() {
    // Called by board when battle is on

    switch (actionState) {
    case ActionState.Wait:
      actionTime += Time.deltaTime;

      // Add AI to decide action when waiting
      if (Random.Range(0f, 1f) > 0.5f)
        ChangeActionState(ActionState.Moving);
      else {
        move = monster.baseMoves[Random.Range(0, monster.baseMoves.Count)];
        ChangeActionState(ActionState.Acting);
      }

      break;

    case ActionState.Moving:
      actionTime += Time.deltaTime * MoveSpeed();
      float i = Mathf.Min(actionTime / moveDuration, 1.0f);
      Vector3 tilePos = Vector3.Lerp(prevTile.transform.position, nextTile.transform.position, i);
      transform.position = tilePos + 0.75f * Vector3.up;

      if (i >= 1.0f) {
        ChangeActionState(ActionState.Wait);
      }

      break;

    case ActionState.Acting:
      actionTime += Time.deltaTime * AttackSpeed();
      bool actingDone = move.Step(actionTime);

      if (actingDone)
        ChangeActionState(ActionState.Wait);

      break;

    case ActionState.Dead:
    default:
      actionTime += Time.deltaTime;
      return;
    }

  }

  private void ChangeActionState(ActionState state) {
    actionState = state;
    actionTime = 0;

    switch (actionState) {
    case ActionState.Moving:
      // find new destination
      nextTile = currentTile.neighbors[Random.Range(0, currentTile.neighbors.Count)];
      prevTile = currentTile;

      if (nextTile.unit == null) {
        nextTile.unit = this;
        prevTile.unit = null;
        currentTile = nextTile;
      } else {
        ChangeActionState(ActionState.Wait);
      }

      break;

    case ActionState.Acting:
      move.Setup(this);
      break;

    case ActionState.Dead:
      currentTile.unit = null;
      transform.position = transform.position + -0.75f * Vector3.up;
      break;

    case ActionState.Wait:
    default:
      return;
    }
  }
}
