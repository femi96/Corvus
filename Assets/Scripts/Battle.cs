using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Battle : MonoBehaviour {

  // 0 front, 1 middle, 2 back
  public Monster[] sideLeft;
  public Monster[] sideRight;
  public List<Monster> monsters;

  void Start() {
    sideLeft = new Monster[3];
    sideRight = new Monster[3];

    sideLeft[0] = new Monster();
    sideRight[1] = new Monster();
    sideRight[2] = new Monster();

    GenerateGameObjects();
  }

  void Update() {}

  public GameObject tilePrefab;
  public GameObject monPrefab;

  public void GenerateGameObjects() {

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
      }

      if (sideLeft[i] != null) {
        go = Instantiate(monPrefab, transform);
        go.transform.position = spacing * (i + 1) * -1 + monHeight;
      }
    }
  }

  public void ActionWait() {
    GetNextTurn();
  }

  private void MoveMonster(int monster, int dir) {
    int side = monster / 3; // 0 is left, 1 is right
    int pos = monster % 3;

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
      } else {
        movingMon = sideRight[pos];
        sideRight[pos] = sideRight[pos + i];
        sideRight[pos + i] = movingMon;
        pos += i;
      }
    }
  }

  private void ResetInitiative() {
    foreach (Monster m in monsters) {
      m.currentInitiative = Random.Range(1, 9) + m.initiative;
    }
  }

  private void GetNextTurn() {
    GetNextMonster();
  }

  private Monster GetNextMonster() {
    int bestInit = 0;
    List<Monster> bestMons = new List<Monster>();

    foreach (Monster m in monsters) {
      if (m.currentInitiative == bestInit) {
        bestMons.Add(m);
      }

      if (m.currentInitiative > bestInit) {
        bestMons = new List<Monster>();
        bestMons.Add(m);
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
