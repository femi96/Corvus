using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Battle : MonoBehaviour {

  public Party[] parties;
  public List<Monster> monsters;
  public Monster currentMonster;

  public Color[] colors;

  [Header("Debug Toggles")]
  public bool controlEnemies = false;

  void Start() {
    parties = new Party[2];

    Monster[] p0 = new Monster[] { new Monster() };
    Monster[] p1 = new Monster[] { new Monster(), new Monster() };
    parties[0] = new Party(p0);
    parties[1] = new Party(p1);


    colors = new Color[10];

    for (int i = 0; i < 10; i++) {
      colors[i] = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
    }

    UpdateMonsterContext();
    GenerateGameObjects();
    GetNextTurn();
  }

  void Update() {
    if (!controlEnemies && currentMonster.partySide != 0)
      ActionWait();
  }

  [Header("Prefab Pointers")]
  public GameObject tilePrefab;
  public GameObject monPrefab;
  public GameObject cursorPrefab;
  private GameObject cursorGo;

  private void GenerateGameObjects() {
    GameObject go;

    for (int j = 0; j < 2; j++)
      for (int i = 0; i < 3; i++) {
        go = Instantiate(tilePrefab, transform);
        go.transform.position = ContextToWorld(j, i);

        Monster m = parties[j].board[i];

        if (m != null) {
          go = Instantiate(monPrefab, transform);
          go.transform.position = ContextToWorld(m);
          m.body = go;
          go.GetComponent<Renderer>().material.SetColor("_Color", colors[m.monID]);
        }
      }

    cursorGo = Instantiate(cursorPrefab, transform);
  }

  public void ActionWait() {
    GetNextTurn();
  }

  public void ActionMove(int dir) {
    MoveMonster(currentMonster, dir);
    GetNextTurn();
  }

  private void MoveMonster(Monster mon, int dir) {
    MoveMonster(mon.partySide, mon.boardPos, dir);
  }

  private void MoveMonster(int side, int pos, int dir) {
    Monster movingMon;

    while (dir != 0) {
      if ((dir > 0 && pos == 2) || (dir < 0 && pos == 0)) {
        dir = 0;
        continue;
      }

      int i = (int)Mathf.Sign(dir);

      movingMon = parties[side].board[pos];
      parties[side].board[pos] = parties[side].board[pos + i];
      parties[side].board[pos + i] = movingMon;
      pos += i;
      dir -= i;

      UpdateMonsterContext();

      foreach (Monster m in monsters) {
        m.body.transform.position = ContextToWorld(m);
      }
    }
  }

  private void UpdateMonsterContext() {
    for (int j = 0; j < 2; j++)
      for (int i = 0; i < 3; i++) {
        if (parties[j].board[i] != null) {
          parties[j].board[i].partySide = j;
          parties[j].board[i].boardPos = i;
        }
      }
  }

  private Vector3 ContextToWorld(Monster mon) {
    return ContextToWorld(mon.partySide, mon.boardPos, true);
  }

  private Vector3 ContextToWorld(int side, int pos, bool isMonster = false) {
    int sign = 2 * side - 1;
    Vector3 spacing = new Vector3(1.2f, 0, 0);
    Vector3 depth = new Vector3(0, 0.15f, 1f);
    Vector3 output = (spacing * (pos + 1) * sign) + (depth * (pos - 0));

    if (isMonster)
      output += 0.5f * Vector3.up;

    return output;
  }

  private void ResetInitiative() {
    foreach (Monster m in monsters) {
      m.currentInitiative = Random.Range(1, 9) + m.initiative;
    }
  }

  private void GetNextTurn() {
    currentMonster = GetNextMonster();
    cursorGo.transform.position = ContextToWorld(currentMonster) + Vector3.up;
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

    foreach (Party p in parties) {
      foreach (Monster m in p.board) {
        if (m != null)
          monsters.Add(m);
      }
    }
  }
}
