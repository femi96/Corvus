using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public enum BattleState {
  Ongoing, Win, Lose
}

public class Battle : MonoBehaviour {

  public BattleState battleState;

  public Party[] parties;
  public List<Monster> monsters;
  public Monster currentMonster;

  public Color[] colors;

  [Header("Debug Toggles")]
  public bool debugControlEnemies = false;
  public bool debugIncrementHealth = false;
  public bool debugDecrementHealth = false;
  private float incrementHealthTick = 0;
  private float enemyAIDelay = 0;

  void Start() {
    BattleLoader.GenerateParties();
    parties = new Party[2];
    parties[0] = BattleLoader.AllyParty;
    parties[1] = BattleLoader.EnemyParty;

    battleState = BattleState.Ongoing;
    colors = new Color[100];

    for (int i = 0; i < 100; i++) {
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
    if (!debugControlEnemies && battleState != BattleState.Ongoing && currentMonster.partySide != 0) {
      enemyAIDelay += Time.deltaTime;

      if (enemyAIDelay >= 1.5f) {
        enemyAIDelay -= 1.5f;

        Move move = null;
        int moveIndex = 0;

        while (move == null) {
          moveIndex = Random.Range(0, 4);
          move = currentMonster.moves[moveIndex];
        }

        ActionUseMove(moveIndex);
      }
    }
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

  public void ActionSwap(int newMon) {
    SwapMonster(currentMonster, newMon);
    GetNextTurn();
  }

  public void ActionUseMove(int moveIndex) {
    Move mov = currentMonster.moves[moveIndex];

    if (mov != null) {
      TargetType[] targetTypes = mov.GetTargetTypes();

      int stage = 0;

      foreach (TargetType t in targetTypes) {
        switch (t) {

        case (TargetType.Single):
          Monster targetMonster = parties[1 - currentMonster.partySide].board[currentMonster.boardPos];

          if (targetMonster != null)
            mov.Act(currentMonster, targetMonster, stage);

          break;

        case (TargetType.Multi):
          List<Monster> targetMonsters = new List<Monster>();

          for (int i = 0; i < 3; i++) {
            Monster tMon = parties[1 - currentMonster.partySide].board[i];

            if (tMon != null)
              targetMonsters.Add(tMon);
          }

          mov.Act(currentMonster, targetMonsters.ToArray(), stage);
          break;

        case (TargetType.Self):
          mov.Act(currentMonster, currentMonster, stage);
          break;
        }

        stage += 1;
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

  private void SwapMonster(Monster oldMon, int newMon) {

    Party party = parties[oldMon.partySide];
    Monster swappingMon = party.members[newMon];

    if (swappingMon == null || party.board.Contains(swappingMon))
      return;

    // Old mon objects
    Destroy(oldMon.body);
    Destroy(oldMon.healthBar);

    // Swap in party
    party.board[oldMon.boardPos] = swappingMon;

    // New mon objects
    GameObject go;
    go = Instantiate(monPrefab, transform);
    swappingMon.body = go;
    go.GetComponent<Renderer>().material.SetColor("_Color", colors[swappingMon.monID]);
    go = Instantiate(hbPrefab, transform);
    swappingMon.healthBar = go;

    UpdateMonsterContext();
    UpdateMonsterRep();
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

  [Header("UI Pointers")]
  public GameObject allUI;
  public GameObject[] movesUI;
  public Text[] movesTextUI;
  public GameObject[] swapUI;
  public Text[] swapTextUI;

  public GameObject winUI;
  public GameObject loseUI;

  private void UpdateUI() {
    if (battleState != BattleState.Ongoing) {
      allUI.SetActive(false);
      winUI.SetActive(battleState == BattleState.Win);
      loseUI.SetActive(battleState == BattleState.Lose);
      return;
    }

    allUI.SetActive(debugControlEnemies || currentMonster.partySide == 0);

    for (int i = 0; i < 4; i++) {
      Move move = currentMonster.moves[i];
      movesUI[i].SetActive(move != null);

      if (move != null) {
        movesTextUI[i].text = move.GetName();
      }
    }

    int j = 0;
    Party party = parties[currentMonster.partySide];

    foreach (GameObject s in swapUI) { s.SetActive(false); }

    foreach (Monster m in party.members) {
      if (m != null && !party.board.Contains(m)) {
        swapUI[j].SetActive(true);
        swapTextUI[j].text = m.name;
        swapUI[j].GetComponent<Button>().onClick.RemoveAllListeners();
        swapUI[j].GetComponent<Button>().onClick.AddListener(() => ActionSwap(System.Array.IndexOf(party.members, m)));
        j += 1;
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
    // Check if battle ended
    bool allyLoss = true;
    bool enemyLoss = true;

    foreach (Monster m in parties[0].board) {
      if (m != null && m.currentHealth > 0)
        allyLoss = false;
    }

    foreach (Monster m in parties[1].board) {
      if (m != null && m.currentHealth > 0)
        enemyLoss = false;
    }

    if (allyLoss) {
      // End loss
      battleState = BattleState.Lose;
    }

    if (enemyLoss) {
      // End win
      battleState = BattleState.Win;
    }

    // Get next monster
    if (battleState == BattleState.Ongoing) {
      currentMonster = GetNextMonster();
      cursorGo.transform.position = ContextToWorld(currentMonster) + Vector3.up;
    }

    UpdateUI();
  }

  private Monster GetNextMonster() {
    UpdateMonsters();
    int bestInit = 0;
    List<Monster> bestMons = new List<Monster>();

    foreach (Monster m in monsters) {
      if (m.currentHealth <= 0)
        continue;

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
