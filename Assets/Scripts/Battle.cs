﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Battle : MonoBehaviour {

  // 0 front, 1 middle, 2 back
  public Monster[] sideLeft;
  public Monster[] sideRight;
  public List<Monster> monsters;
  public Monster currentMonster;

  public Color[] colors;

  void Start() {
    sideLeft = new Monster[3];
    sideRight = new Monster[3];

    sideLeft[0] = new Monster();
    sideRight[1] = new Monster();
    sideRight[2] = new Monster();
    UpdateMonsterLocations();

    colors = new Color[10];

    for (int i = 0; i < 10; i++) {
      colors[i] = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
    }

    GenerateGameObjects();
    GetNextTurn();
  }

  void Update() {}

  public GameObject tilePrefab;
  public GameObject monPrefab;

  private void GenerateGameObjects() {
    foreach (Transform child in transform) {
      Destroy(child.gameObject);
    }

    Vector3 spacing = new Vector3(1.2f, 0, 0);
    Vector3 monHeight = Vector3.up;
    GameObject go;

    for (int i = 0; i < 3; i++) {
      go = Instantiate(tilePrefab, transform);
      go.transform.position = spacing * (i + 1);
      go = Instantiate(tilePrefab, transform);
      go.transform.position = spacing * (i + 1) * -1;

      if (sideRight[i] != null) {
        go = Instantiate(monPrefab, transform);
        go.transform.position = spacing * (i + 1) + monHeight;
        sideRight[i].body = go;
        go.GetComponent<Renderer>().material.SetColor("_Color", colors[sideRight[i].monID]);
      }

      if (sideLeft[i] != null) {
        go = Instantiate(monPrefab, transform);
        go.transform.position = spacing * (i + 1) * -1 + monHeight;
        sideLeft[i].body = go;
        go.GetComponent<Renderer>().material.SetColor("_Color", colors[sideLeft[i].monID]);
      }
    }
  }

  public void ActionWait() {
    GetNextTurn();
  }

  public void ActionMove(int dir) {
    MoveMonster(currentMonster.location, dir);
    GetNextTurn();
  }

  private void MoveMonster(int loc, int dir) {
    int side = loc / 3; // 0 is left, 1 is right
    int pos = loc % 3;

    Monster movingMon;

    while (dir != 0) {
      if ((dir > 0 && pos == 2) || (dir < 0 && pos == 0)) {
        dir = 0;
        continue;
      }

      int i = (int)Mathf.Sign(dir);

      if (side == 0) {
        movingMon = sideLeft[pos];
        sideLeft[pos] = sideLeft[pos + i];
        sideLeft[pos + i] = movingMon;
        pos += i;
        dir -= i;
      } else {
        movingMon = sideRight[pos];
        sideRight[pos] = sideRight[pos + i];
        sideRight[pos + i] = movingMon;
        pos += i;
        dir -= i;
      }
    }

    GenerateGameObjects();
    UpdateMonsterLocations();
  }

  private void UpdateMonsterLocations() {
    for (int i = 0; i < 3; i++) {
      if (sideLeft[i] != null) {
        sideLeft[i].location = i;
      }

      if (sideRight[i] != null) {
        sideRight[i].location = 3 + i;
      }
    }
  }

  private void ResetInitiative() {
    foreach (Monster m in monsters) {
      m.currentInitiative = Random.Range(1, 9) + m.initiative;
    }
  }

  public int monsterID;
  private void GetNextTurn() {
    currentMonster = GetNextMonster();
    monsterID = currentMonster.monID;
  }

  private Monster GetNextMonster() {
    UpdateMonsters();
    int bestInit = 0;
    List<Monster> bestMons = new List<Monster>();

    foreach (Monster m in monsters) {
      if (m.currentInitiative == bestInit) {
        bestMons.Add(m);
      }

      if (m.currentInitiative > bestInit) {
        bestMons = new List<Monster>();
        bestMons.Add(m);
        bestInit = m.currentInitiative;
      }
    }

    if (bestInit == 0) {
      ResetInitiative();
      return GetNextMonster();
    }

    Monster bestMon = bestMons[Random.Range(0, bestMons.Count)];
    bestMon.currentInitiative = 0;
    return bestMon;
  }

  private void UpdateMonsters() {
    monsters = new List<Monster>();

    foreach (Monster m in sideLeft) {
      if (m != null)
        monsters.Add(m);
    }

    foreach (Monster m in sideRight) {
      if (m != null)
        monsters.Add(m);
    }
  }
}
