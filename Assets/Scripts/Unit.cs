﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ActionState { Wait, Dead, Moving, Acting };

public class Unit : MonoBehaviour {

  public Board board;

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
  private const float actDuration = 1f;
  private Move move;
  private Tile[] targetTiles;
  private List<Unit> targetsHit;
  private GameObject actGo;
  public GameObject actPrefab;

  void Start() {
    ResetUnit();
  }

  void Update() {

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
      else
        ChangeActionState(ActionState.Acting);

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
      float a = Mathf.Min(actionTime / actDuration, 1.0f);

      if (a >= 0.5f) {
        foreach (Tile actTile in targetTiles) {
          if (actTile.unit != null && !targetsHit.Contains(actTile.unit))
            actTile.unit.DealDamage(10);

          if (actGo == null) {
            actGo = Instantiate(actPrefab, transform);
            actGo.transform.position = actTile.transform.position;
          }
        }
      }

      if (a >= 1.0f) {
        Destroy(actGo);
        ChangeActionState(ActionState.Wait);
      }

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
      targetTiles = new Tile[] { currentTile.neighbors[Random.Range(0, currentTile.neighbors.Count)] };
      targetsHit = new List<Unit>();
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