using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Battle : MonoBehaviour {

  public Party[] parties;
  public List<Monster> monsters;
  public Monster currentMonster;

  public Color[] colors;

  [Header("Debug Toggles")]
  public bool debugControlEnemies = false;
  public bool debugIncrementHealth = false;
  public bool debugDecrementHealth = false;
  private float incrementHealthTick = 0;

  void Start() {
    parties = new Party[2];

    Monster[] p0 = new Monster[] { new Monster(), new Monster(), new Monster(), new Monster(), new Monster() };
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

    /* Debug for testing health bar by varying over time */
    if (debugIncrementHealth || debugDecrementHealth) {
      incrementHealthTick += Time.deltaTime / 0.1f;

      if (incrementHealthTick > 1f) {
        incrementHealthTick -= 1f;
        int i = 1;

        if (debugDecrementHealth)
          i = -1;

        foreach (Monster m in monsters) {
          if (debugIncrementHealth && !debugDecrementHealth && m.currentHealth == m.maxHealth)
            m.currentHealth = 0;
          else if (debugDecrementHealth && m.currentHealth == 0)
            m.currentHealth = m.maxHealth;
          else
            m.currentHealth += i * Mathf.CeilToInt(0.01f * m.maxHealth);

          m.currentHealth = Mathf.Clamp(m.currentHealth, 0, m.maxHealth);
        }

        UpdateMonsterRep();
      }
    }

    /* Debug for disabling enemy AI */
    if (!debugControlEnemies && currentMonster.partySide != 0)
      ActionWait();
  }

  [Header("Prefab Pointers")]
  public GameObject tilePrefab;
  public GameObject monPrefab;
  public GameObject cursorPrefab;
  public GameObject hbPrefab;
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
          m.body = go;
          go.GetComponent<Renderer>().material.SetColor("_Color", colors[m.monID]);

          go = Instantiate(hbPrefab, transform);
          m.healthBar = go;
          // go.GetComponent<Renderer>().material.SetColor("_Color", colors[m.monID]);
        }

      }

    UpdateMonsterRep();
    cursorGo = Instantiate(cursorPrefab, transform);
  }

  public void ActionWait() {
    GetNextTurn();
  }

  public void ActionMove(int dir) {
    MoveMonster(currentMonster, dir);
    GetNextTurn();
  }

  public void ActionUseMove(int moveIndex) {
    Move mov = currentMonster.moves[moveIndex];

    if (mov != null) {
      if (mov.GetTargetType() == TargetType.Single) {
        Monster targetMonster = parties[1 - currentMonster.partySide].board[currentMonster.boardPos];
        mov.Act(currentMonster, targetMonster);
      }

      if (mov.GetTargetType() == TargetType.Multi) {
        Monster[] targetMonsters = new Monster[3];

        for (int i = 0; i < 3; i++) {
          targetMonsters[i] = parties[1 - currentMonster.partySide].board[i];
        }

        mov.Act(currentMonster, targetMonsters);
      }
    }

    UpdateMonsterRep();
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
      UpdateMonsterRep();
    }
  }

  private void UpdateMonsterRep() {
    /* Function for instant updates of monsters visual representations
      Eventually, these updates will be changed to act over time
      */

    UpdateMonsters();

    Color g = new Color(0.25f, 1f, 0.25f);
    Color y = new Color(1f, 1f, 0.25f);
    Color r = new Color(1f, 0.25f, 0.25f);

    foreach (Monster m in monsters) {
      m.body.transform.position = ContextToWorld(m);

      float hR = Mathf.Max(m.currentHealth * 1f / m.maxHealth, 0.001f);
      /* Health bar colors
        G 100% to 60%
        Y 50% to 20%
        R 10% to 0%
        */

      Color hbColor = Mathf.Clamp((hR - 0.5f) / 0.1f, 0f, 1f) * g +
                      Mathf.Clamp((0.25f - Mathf.Abs(0.35f - hR)) / 0.1f, 0f, 1f) * y +
                      Mathf.Clamp((0.2f - hR) / 0.1f, 0f, 1f) * r;
      m.healthBar.transform.position = ContextToWorld(m) + 0.8f * Vector3.up;
      m.healthBar.transform.localScale = new Vector3(hR, 0.2f, 0.2f);
      m.healthBar.GetComponent<Renderer>().material.SetColor("_Color", hbColor);
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
    Vector3 output = (spacing * (pos + 1) * sign) + (depth * (2 - pos));

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
