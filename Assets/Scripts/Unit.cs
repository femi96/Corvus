using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum ActionState { Wait, Dead, Moving, Acting };

public enum DamageType { Physical, Magical, True };

public class Unit : MonoBehaviour {

  public Board board;

  public int team = 0;

  public Monster monster;
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
      uiHealth.GetComponent<Image>().color = Color.green;
    else
      uiHealth.GetComponent<Image>().color = Color.red;

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

    // Evasion
    bool miss = Random.Range(0f, 1f) < monster.Evasion() / 100f;

    if (miss)
      damage = 0;

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
    int roundedDamage = Mathf.RoundToInt(damage);

    // Damage effect on health
    // int preHealth = currentHealth;
    currentHealth = Mathf.Max(currentHealth - roundedDamage, 0);

    // Effects
    GameObject go = Instantiate(UIPrefabs.instance.textPrefab, UIPrefabs.instance.canvasTransform);
    Text txt = go.transform.Find("Text").gameObject.GetComponent<Text>();
    uiDamageText.Add(go);

    TextEffectMover em = go.GetComponent<TextEffectMover>();
    Vector3 rand = new Vector3(Random.Range(0.1f, 1f), 0f, 0f);
    rand = rand * (Random.Range(0, 2) * 2 - 1);
    em.velocity += rand;

    if (miss) {
      txt.text = "Miss";
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
        if (currentEnergy > monster.specMoves[0].EnergyCost())
          move = monster.specMoves[0];
        else
          move = monster.baseMoves[Random.Range(0, monster.baseMoves.Count)];

        ChangeActionState(ActionState.Acting);
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
