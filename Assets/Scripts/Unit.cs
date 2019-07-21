﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ActionState { Wait, Dead, Moving };

public class Unit : MonoBehaviour {

  public Board board;

  public float health;
  public float energy;

  public ActionState actionState;
  private float actionTime = 0;

  // Moving
  private const float moveDuration = 0.2f;
  public Tile currentTile;
  private Tile nextTile;
  private Tile prevTile;

  void Start() {
    ResetUnit();
  }

  void Update() {

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
      ChangeActionState(ActionState.Moving);
      break;

    case ActionState.Moving:
      float i = Mathf.Min(actionTime / moveDuration, 1.0f);
      transform.position = Vector3.Lerp(prevTile.transform.position, nextTile.transform.position, i);

      if (i >= 0.5f) {
        currentTile = nextTile;
      }

      if (i >= 1.0f) {
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
      } else {
        ChangeActionState(ActionState.Wait);
      }

      break;

    case ActionState.Wait:
    case ActionState.Dead:
    default:
      return;
    }
  }
}
