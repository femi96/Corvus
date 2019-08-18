using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum GoalState { None, BasicAttack, SpecialAttack };

public enum ActionState { Wait, Dead, Moving, Acting };

public enum DamageType { Physical, Magical, True };

public class Unit : MonoBehaviour {

  public Board board;

  public int team = 0;

  public Monster monster;
  private GameObject model;


  // Unit stats
  public int currentHealth;
  public int currentEnergy;


  /* Unit AI is broken into to layers:
    High level goal layer & low level action layer
    */

  [Header("AI")]
  // Goals
  public GoalState goalState;
  public Unit attackTarget;
  public int specialMoveCount;

  // Actions
  public ActionState actionState;
  private float actionTime = 0;

  // Moving
  private const float moveDuration = 0.5f;
  public Tile currentTile;
  private Tile nextTile;
  private Tile prevTile;

  // Acting
  private Move move;

  // UI
  [Header("UI")]
  private GameObject uiHover;
  private GameObject uiHealth;
  private GameObject uiEnergy;
  private List<GameObject> uiDamageText;
  private Vector3 uiHoverOffset = new Vector3(0, 1.25f, 0);

  void Start() {
    if (Random.Range(0f, 1f) > 0.5f)
      monster = new Ashire();
    else
      monster = new Shen();

    ResetModel();
    ResetUnit();

    uiHover = Instantiate(UIPrefabs.instance.hoverPrefab, UIPrefabs.instance.canvasTransform);
    uiHealth = uiHover.transform.Find("Health").gameObject;
    uiEnergy = uiHover.transform.Find("Energy").gameObject;
    uiDamageText = new List<GameObject>();
  }

  void Update() {

  }

  void LateUpdate() {
    UpdateUI();
  }

  private void UpdateUI() {
    uiHover.SetActive(IsAlive());

    float healthPercent = 100f * currentHealth / monster.MaxHealth();
    float energyPercent = 100f * currentEnergy / 100f;
    Vector3 pos = transform.position + uiHoverOffset;
    uiHealth.GetComponent<RectTransform>().sizeDelta = new Vector2(healthPercent, 20);
    uiEnergy.GetComponent<RectTransform>().sizeDelta = new Vector2(energyPercent, 20);
    uiHover.transform.position = RectTransformUtility.WorldToScreenPoint(Camera.main, pos);

    if (team == 0)
      uiHealth.GetComponent<Image>().color = UIColor.Ally();
    else
      uiHealth.GetComponent<Image>().color = UIColor.Enemy();

    // Remove nulls
    for (var i = uiDamageText.Count - 1; i > -1; i--)
      if (uiDamageText[i] == null)
        uiDamageText.RemoveAt(i);

    foreach (GameObject go in uiDamageText) {
      pos = go.GetComponent<TextEffectMover>().position;
      pos += transform.position;
      go.transform.position = RectTransformUtility.WorldToScreenPoint(Camera.main, pos);
    }
  }

  private void ResetModel() {
    foreach (Transform child in transform)
      Destroy(child.gameObject);

    model = monster.GetPrefab();
    model = Instantiate(model, transform.position, Quaternion.identity, transform);
  }

  void OnMouseOver() {
    if (Input.GetMouseButtonDown(0))
      board.clickSelection.OnClickUnit(this);
  }

  public void DealDamage(float damage, DamageType type = DamageType.True,
                         bool crit = false, bool miss = false) {

    // Damage effect on energy
    currentEnergy += Mathf.RoundToInt(damage / 200f * monster.EnergyMod());

    // Damage reduction from defense
    float reduction = 1f;

    if (type == DamageType.Physical)
      reduction = 1f - 0.05f * monster.Endurance() / (1 + 0.05f * Mathf.Abs(monster.Endurance()));

    if (type == DamageType.Magical)
      reduction = 1f - 0.05f * monster.Resistance() / (1 + 0.05f * Mathf.Abs(monster.Resistance()));

    damage *= reduction;

    // Round damage
    int roundedDamage = Mathf.RoundToInt(damage / 4);

    // Damage effect on health
    // int preHealth = currentHealth;
    currentHealth = Mathf.Max(currentHealth - roundedDamage, 0);

    // Visual Effects
    GameObject go = Instantiate(UIPrefabs.instance.textPrefab, UIPrefabs.instance.canvasTransform);
    Text txt = go.transform.Find("Text").gameObject.GetComponent<Text>();
    uiDamageText.Add(go);

    TextEffectMover em = go.GetComponent<TextEffectMover>();
    Vector3 rand = new Vector3(Random.Range(0.1f, 1f), 0f, 0f);
    rand = rand * (Random.Range(0, 2) * 2 - 1);
    em.velocity += rand;

    if (type == DamageType.Physical)
      txt.color = UIColor.DamagePhysical();

    if (type == DamageType.Magical)
      txt.color = UIColor.DamageMagical();

    if (type == DamageType.True)
      txt.color = UIColor.DamageTrue();

    if (miss) {
      txt.text = "Miss";
      txt.color = UIColor.DamageMiss();
      // Debug.Log("Miss!");
    } else if (crit) {
      txt.text = roundedDamage + "!";
      // Debug.Log("Crit! Unit takes " + roundedDamage + " " + type + " damage. " + preHealth + " -> " + currentHealth);
    } else {
      txt.text = roundedDamage + "";
      // Debug.Log("Unit takes " + roundedDamage + " " + type + " damage. " + preHealth + " -> " + currentHealth);
    }

    // Death check
    TryDead();
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
    specialMoveCount = -1;
    goalState = GoalState.None;
    actionState = ActionState.Wait;
  }

  public void BattleTimeStep() {
    // Called by board when battle is on

    // If goal blocked, remove goal
    // If no goal, set new goal
    //  Special attack if mana for next special
    //  Basic attack
    // If no action, set action based on goal
    //  If action cannot be set, mark goal as blocked

    switch (actionState) {
    case ActionState.Wait:
      actionTime += Time.deltaTime;

      goalState = GoalState.None;

      if (TryNewGoalSpecial() || TryNewGoalAttack()) {
      } else {
        return;
      }

      switch (goalState) {
      case GoalState.BasicAttack:
        NewActionForGoalAttack();
        break;

      case GoalState.SpecialAttack:
        NewActionForGoalSpecial();
        break;

      default:
        return;
      }

      break;

    case ActionState.Moving:
      actionTime += Time.deltaTime * monster.MoveSpeed();
      float i = Mathf.Min(actionTime / moveDuration, 1.0f);
      Vector3 tilePos = Vector3.Lerp(prevTile.transform.position, nextTile.transform.position, i);
      transform.position = tilePos + 0.75f * Vector3.up;

      if (i >= 1.0f) {
        ChangeActionState(ActionState.Wait);
      }

      break;

    case ActionState.Acting:
      actionTime += Time.deltaTime * monster.AttackSpeed();
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
      prevTile = currentTile;

      if (nextTile.unit == null) {
        model.transform.rotation = Quaternion.LookRotation(nextTile.transform.position - prevTile.transform.position);
        nextTile.unit = this;
        prevTile.unit = null;
        currentTile = nextTile;
      } else {
        ChangeActionState(ActionState.Wait);
      }

      break;

    case ActionState.Acting:
      model.transform.rotation = Quaternion.LookRotation(attackTarget.transform.position - transform.position);
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

  private bool TryNewGoalSpecial() {
    int newCount = (specialMoveCount + 1) % monster.specMoves.Count;

    if (monster.specMoves.Count == 0 || currentEnergy < monster.specMoves[newCount].EnergyCost())
      return false;

    specialMoveCount = newCount;

    goalState = GoalState.SpecialAttack;

    float minHp = 1000f;

    foreach (Unit unit in board.units) {
      float hp = unit.currentHealth / (float)unit.monster.MaxHealth();

      if (unit.team != team && unit.IsAlive() && hp <= minHp) {
        attackTarget = unit;
        minHp = hp;
      }
    }

    return true;
  }

  private bool TryNewGoalAttack() {
    if (monster.baseMoves.Count == 0)
      return false;

    goalState = GoalState.BasicAttack;

    float minDist = 1000f;

    foreach (Unit unit in board.units) {
      float dist = currentTile.DistanceTo(unit.currentTile);

      if (unit.team != team && unit.IsAlive() && dist <= minDist) {
        attackTarget = unit;
        minDist = dist;
      }
    }

    return true;
  }

  private void NewActionForGoalAttack() {
    // If in range, attack, otherwise get in range

    // get base moves
    // filter moves by if distance is within range
    float targetDistance = currentTile.DistanceTo(attackTarget.currentTile);

    List<Move> movesInRange = new List<Move>();
    float maxRange = 0f;

    foreach (Move m in monster.baseMoves) {
      if (m.Range() >= targetDistance)
        movesInRange.Add(m);

      maxRange = Mathf.Max(maxRange, m.Range());
    }

    // if moves remaining (in range), use a random move
    if (movesInRange.Count > 0) {
      move = movesInRange[Random.Range(0, movesInRange.Count)];
      Debug.Log(move);
      ChangeActionState(ActionState.Acting);
      return;
    }

    // if no moves, get all tiles in range from target
    // Route to nearest tile in range
    PathToAttackTarget(maxRange);
  }

  private void NewActionForGoalSpecial() {
    // If in range, attack, otherwise get in range

    float targetDistance = currentTile.DistanceTo(attackTarget.currentTile);
    float range = monster.specMoves[specialMoveCount].Range();

    // if move (in range), use move
    if (range >= targetDistance) {
      move = monster.specMoves[specialMoveCount];
      ChangeActionState(ActionState.Acting);
      return;
    }

    // if no moves, get all tiles in range from target
    // Route to nearest tile in range
    PathToAttackTarget(range);
  }

  private void PathToAttackTarget(float range) {
    // Helper function that paths and moves unit to within range of attackTaget
    List<Tile> tilesInRange = board.GetTilesInRange(attackTarget.currentTile, range);
    List<Tile> pathToTile = board.GetShortestPathTo(currentTile, tilesInRange);

    if (pathToTile.Count > 0) {
      nextTile = pathToTile[0];
    } else {
      nextTile = currentTile.neighbors[Random.Range(0, currentTile.neighbors.Count)];
    }

    ChangeActionState(ActionState.Moving);
  }
}