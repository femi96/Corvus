using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ActionState { Wait, Dead, Moving, Acting };

public class Unit : MonoBehaviour {

  public Board board;

  public int team = 0;

  private Monster monster;
  private GameObject model;

  public float health;
  public float energy;

  public ActionState actionState;
  private float actionTime = 0;

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

  public void DealDamage(float damage) {
    float prehealth = health;
    health = Mathf.Max(health - damage, 0);

    Debug.Log("Unit takes " + damage + " damage. " + prehealth + " -> " + health);
    TryDead();
  }

  private void TryDead() {
    if (health > 0)
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
    health = 40;
    energy = 0;
    actionState = ActionState.Wait;
  }

  public void ActionStep() {
    // Called by board when battle is on

    actionTime += Time.deltaTime;

    switch (actionState) {
    case ActionState.Wait:

      // Add AI to decide action when waiting
      if (Random.Range(0f, 1f) > 0.5f)
        ChangeActionState(ActionState.Moving);
      else {
        move = monster.baseMoves[Random.Range(0, monster.baseMoves.Count)];
        ChangeActionState(ActionState.Acting);
      }

      break;

    case ActionState.Moving:
      float i = Mathf.Min(actionTime / moveDuration, 1.0f);
      Vector3 tilePos = Vector3.Lerp(prevTile.transform.position, nextTile.transform.position, i);
      transform.position = tilePos + 0.75f * Vector3.up;

      if (i >= 1.0f) {
        ChangeActionState(ActionState.Wait);
      }

      break;

    case ActionState.Acting:
      bool actingDone = move.Step(actionTime);

      if (actingDone)
        ChangeActionState(ActionState.Wait);

      break;

    case ActionState.Dead:
    default:
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
