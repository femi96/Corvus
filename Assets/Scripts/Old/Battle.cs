﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public enum BattleStateOld {
  Inactive, Ongoing, Win, Lose
}

public enum BattlePhase {
  Selection, Execution, End
}

public enum ActionType {
  Wait, Swap, Move, UseMove
}

public class Battle : MonoBehaviour {

  public Map map;
  public CameraMover camMover;

  public BattleStateOld battleStateOld = BattleStateOld.Inactive;
  public BattlePhase battlePhase = BattlePhase.Selection;

  public Party[] parties;

  public List<Monster> boardMonsters;
  public Monster currentMonster;

  [Header("Monster Visuals")]
  public Color[] colors;

  [Header("Debug Toggles")]
  public bool debugControlEnemies = false;





  /* Battle usage */

  public void StartBattle(Party allyParty, Party enemyParty) {
    battleStateOld = BattleStateOld.Ongoing;

    parties = new Party[2];
    parties[0] = allyParty;
    parties[1] = enemyParty;

    ResetMonstersForBattle();
    SetupColors();

    UpdateBoardMonsters();
    UpdateMonsterContext();
    GenerateInitialGameObjects();

    CloseMessage();
    StartSelectionPhase();
  }

  private void ResetMonstersForBattle() {
    for (int j = 0; j < 2; j++)
      for (int i = 0; i < 6; i++) {
        Monster m = parties[j].members[i];

        if (m != null)
          m.ResetForBattle();
      }
  }

  private void SetupColors() {
    if (!(colors == null || colors.Length != 100))
      return;

    colors = new Color[100];

    for (int i = 0; i < 100; i++) {
      colors[i] = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
    }
  }

  private void UpdateBoardMonsters() {
    boardMonsters = new List<Monster>();

    foreach (Party p in parties) {
      foreach (Monster m in p.board) {
        if (m != null)
          boardMonsters.Add(m);
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
    Vector3 depth = new Vector3(0, 0, 1f);
    Vector3 output = (spacing * (pos + 1) * sign) + (depth * (2 - pos));

    if (isMonster)
      output += 1.4f * Vector3.up;

    return output;
  }





  /* Manage Game Objects */

  [Header("Prefab Pointers")]
  public GameObject tilePrefab;
  public GameObject monPrefab;
  public GameObject cursorPrefab;
  public GameObject hbPrefab;
  private GameObject cursorGo;


  private void GenerateInitialGameObjects() {
    // Clear objects
    foreach (Transform child in transform)
      Destroy(child.gameObject);

    // Create tiles
    for (int j = 0; j < 2; j++)
      for (int i = 0; i < 3; i++) {
        GameObject go = Instantiate(tilePrefab, transform);
        go.transform.position = ContextToWorld(j, i);

      }

    cursorGo = Instantiate(cursorPrefab, transform);

    foreach (Monster m in boardMonsters)
      AddMonsterGameObject(m);

    UpdateMonsterRep();
  }

  private void AddMonsterGameObject(Monster m) {
    m.body = Instantiate(monPrefab, transform);
    m.body.GetComponent<Renderer>().material.SetColor("_Color", colors[m.monID]);
    m.healthBar = Instantiate(hbPrefab, transform);

    if (m.partySide == 1)
      m.body.GetComponent<SpriteRenderer>().flipX = true;
  }

  private void RemoveMonsterGameObject(Monster m) {
    Destroy(m.body);
    Destroy(m.healthBar);
  }

  private void UpdateMonsterRep() {
    /* Function for instant updates of monsters visual representations
      Eventually, these updates will be changed to act over time
      */

    Color g = new Color(0.25f, 1f, 0.25f);
    Color y = new Color(1f, 1f, 0.25f);
    Color r = new Color(1f, 0.25f, 0.25f);

    foreach (Monster m in boardMonsters) {
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
      m.healthBar.transform.position = ContextToWorld(m) + 1.5f * Vector3.up;
      m.healthBar.transform.localScale = new Vector3(hR, 0.2f, 1f);
      m.healthBar.GetComponent<Renderer>().material.SetColor("_Color", hbColor);
    }
  }





  /* Selection phase */

  private int selectionIndex = 0;

  private void StartSelectionPhase() {
    battlePhase = BattlePhase.Selection;
    selectionIndex = 0;
    GetNextAction();
  }

  private void GetNextAction() {
    if (selectionIndex >= boardMonsters.Count) {
      selectionIndex = 0;
      StartExecutionPhase();
      return;
    }

    currentMonster = parties[selectionIndex / 3].board[selectionIndex % 3];
    selectionIndex += 1;

    if (currentMonster == null || currentMonster.currentHealth <= 0) {
      GetNextAction();
      return;
    }

    cursorGo.transform.position = ContextToWorld(currentMonster) + 2.0f * Vector3.up;
    camMover.SetAnchor(cursorGo.transform.position);
    UpdateUI();

    // if AI, use AI and get next action
    if (!debugControlEnemies && currentMonster.partySide != 0) {
      AISelection();
      GetNextAction();
    }
  }

  private void AISelection() {
    currentMonster.actionType = ActionType.UseMove;

    Move move = null;
    int moveIndex = 0;

    while (move == null) {
      moveIndex = Random.Range(0, 4);
      move = currentMonster.moves[moveIndex];
    }

    currentMonster.actionParamInt = moveIndex;
  }

  public void ActionWait() {
    currentMonster.actionType = ActionType.Wait;
    GetNextAction();
  }

  public void ActionMove(int dir) {
    currentMonster.actionType = ActionType.Move;
    currentMonster.actionParamInt = dir;
    GetNextAction();
  }

  public void ActionSwap(int newMon) {
    currentMonster.actionType = ActionType.Swap;
    currentMonster.actionParamInt = newMon;
    GetNextAction();
  }

  public void ActionUseMove(int moveIndex) {
    currentMonster.actionType = ActionType.UseMove;
    currentMonster.actionParamInt = moveIndex;
    GetNextAction();
  }





  /* Execution phase */

  private void StartExecutionPhase() {
    battlePhase = BattlePhase.Execution;
    ResetInitiative();
    GetNextExecute();
    UpdateUI();
  }

  private void GetNextExecute() {
    CloseMessage();
    CheckEndCondition();

    if (battleStateOld == BattleStateOld.Ongoing) {
      if (GetNextCurrentMonster()) {
        cursorGo.transform.position = ContextToWorld(currentMonster) + 2.0f * Vector3.up;
        camMover.SetAnchor(cursorGo.transform.position);
      } else {
        StartEndPhase();
      }
    }
  }

  private void ExecuteWait() {
    switch (executionPhase) {
    case -1:
      SetMessage(currentMonster.name + " waits.");
      executionPhase = 0; // End
      executionDelay = 0f;
      return;
    }
  }

  private void ExecuteMove() {
    switch (executionPhase) {
    case -1:
      executionPhase = 0; // End
      executionDelay = 0f;
      MoveMonster(currentMonster, currentMonster.actionParamInt);
      return;
    }
  }

  private void ExecuteSwap() {
    switch (executionPhase) {
    case -1:
      executionPhase = 0; // End
      executionDelay = 0f;
      SwapMonster(currentMonster, currentMonster.actionParamInt);
      return;
    }
  }

  private void ExecuteUseMove() {
    switch (executionPhase) {
    case -1:
      executionPhase = 0; // End
      executionDelay = 0f;
      UseMove(currentMonster, currentMonster.actionParamInt);
      return;
    }
  }

  private void ResetInitiative() {
    foreach (Monster m in boardMonsters) {
      m.currentInitiative = Random.Range(1, 9) + m.initiative;
    }
  }

  private bool GetNextCurrentMonster() {
    int bestInit = 0;
    List<Monster> bestMons = new List<Monster>();

    foreach (Monster m in boardMonsters) {
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
      currentMonster = null;
      return false;
    } else {
      currentMonster = bestMons[Random.Range(0, bestMons.Count)];
      currentMonster.currentInitiative = 0;
      return true;
    }
  }

  private void CheckEndCondition() {
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
      battleStateOld = BattleStateOld.Lose;
    }

    if (enemyLoss) {
      // End win
      battleStateOld = BattleStateOld.Win;
    }

    UpdateUI();
  }





  /* End phase */

  private void StartEndPhase() {
    battlePhase = BattlePhase.End;
    CheckEndCondition();
    StartSelectionPhase();
  }





  /* Real time action */

  private float actionDelay = 1f;
  private float actionDelayTime = 0f;

  private float executionDelay = 0f;
  private float executionDelayTime = 0f;

  private int executionPhase = -1;

  void Update() {

    if (battleStateOld != BattleStateOld.Ongoing) {

      if (battleStateOld == BattleStateOld.Win || battleStateOld == BattleStateOld.Lose) {
        actionDelayTime += Time.deltaTime;

        if (actionDelayTime >= actionDelay) {
          map.EndBattle();
          battleStateOld = BattleStateOld.Inactive;
          actionDelayTime = 0;
        }
      }

      return;
    }

    // While battle is ongoing

    if (battlePhase == BattlePhase.Execution) {
      actionDelayTime += Time.deltaTime;

      if (actionDelayTime < actionDelay)
        return;

      executionDelayTime += Time.deltaTime;

      if (executionDelayTime < executionDelay || !(Inputs.NextMessage() || executionPhase == -1))
        return;

      if (executionPhase == 0) {
        // executionPhase == 0 is end
        actionDelayTime = 0;
        executionDelay = 0;
        executionPhase = -1;
        GetNextExecute();
        return;
      }

      switch (currentMonster.actionType) {
      case ActionType.Wait:
        ExecuteWait();
        break;

      case ActionType.Move:
        ExecuteMove();
        break;

      case ActionType.Swap:
        ExecuteSwap();
        break;

      case ActionType.UseMove:
        ExecuteUseMove();
        break;
      }

      executionDelayTime = 0f;
    }
  }

  private void MoveMonster(Monster mon, int dir) {
    MoveMonster(mon.partySide, mon.boardPos, dir);
  }

  private void MoveMonster(int side, int pos, int dir) {
    Monster movingMon;

    if (dir > 0)
      SetMessage(parties[side].board[pos].name + " moved backwards " + dir + ".");
    else
      SetMessage(parties[side].board[pos].name + " moved forwards " + -dir + ".");

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
      cursorGo.transform.position = ContextToWorld(currentMonster) + 2.0f * Vector3.up;
    }
  }

  private void SwapMonster(Monster oldMon, int newMon) {

    Party party = parties[oldMon.partySide];
    Monster swappingMon = party.members[newMon];

    if (swappingMon == null || party.board.Contains(swappingMon)) {
      SetMessage(oldMon.name + " swap with " + swappingMon.name + " failed.");
      return;
    }

    SetMessage(oldMon.name + " swaps with " + swappingMon.name + ".");

    // Old mon objects
    RemoveMonsterGameObject(oldMon);

    // Swap in party
    party.board[oldMon.boardPos] = swappingMon;

    // New mon objects
    AddMonsterGameObject(swappingMon);

    UpdateBoardMonsters();
    UpdateMonsterContext();
    UpdateMonsterRep();
  }

  private void UseMove(Monster mon, int moveIndex) {
    Move mov = mon.moves[moveIndex];

    SetMessage(mon.name + " uses move " + mov.GetName() + ".");

    if (mov != null) {
      TargetType[] targetTypes = mov.GetTargetTypes();

      int stage = 0;

      foreach (TargetType t in targetTypes) {
        switch (t) {

        case (TargetType.Single):
          Monster targetMonster = parties[1 - mon.partySide].board[mon.boardPos];

          if (targetMonster != null)
            mov.Act(mon, targetMonster, stage);

          break;

        case (TargetType.Multi):
          List<Monster> targetMonsters = new List<Monster>();

          for (int i = 0; i < 3; i++) {
            Monster tMon = parties[1 - mon.partySide].board[i];

            if (tMon != null)
              targetMonsters.Add(tMon);
          }

          mov.Act(mon, targetMonsters.ToArray(), stage);
          break;

        case (TargetType.Self):
          mov.Act(mon, mon, stage);
          break;
        }

        stage += 1;
      }
    }

    UpdateMonsterRep();
  }





  /* UI */

  [Header("UI Pointers")]
  public GameObject allUI;
  public GameObject[] movesUI;
  public Text[] movesTextUI;
  public GameObject[] swapUI;
  public Text[] swapTextUI;

  public GameObject messageUI;
  public Text messageTextUI;

  public GameObject winUI;
  public GameObject loseUI;

  private void UpdateUI() {
    winUI.SetActive(battleStateOld == BattleStateOld.Win);
    loseUI.SetActive(battleStateOld == BattleStateOld.Lose);

    if (battleStateOld != BattleStateOld.Ongoing || battlePhase != BattlePhase.Selection) {
      allUI.SetActive(false);
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

  private void SetMessage(string str) {
    messageTextUI.text = str;
    messageUI.SetActive(true);
  }

  private void CloseMessage() {
    messageUI.SetActive(false);
    messageTextUI.text = "";
  }
}