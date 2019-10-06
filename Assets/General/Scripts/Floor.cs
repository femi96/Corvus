using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floor : MonoBehaviour {

  public Player player;

  private GameObject boardObject;

  [Header("UI")]
  public GameObject nextEncounterUI;
  public GameObject startBattleUI;

  void Start() {
    startBattleUI.SetActive(false);
  }

  void Update() {}

  public void LoadEncounter() {
    LoadEncounter(new Encounter());
  }

  public void LoadEncounter(Encounter encounter) {
    Party playerParty = player.party;
    Party enemyParty = encounter.party;

    boardObject = Instantiate(encounter.boardPrefab, transform);
    ClickSelection.instance.Setup();

    Board board = FindObjectsOfType<Board>()[0];
    PartyTileHolder playerBench = board.parties[0];
    PartyTileHolder enemyBench = board.parties[1];
    playerBench.Setup(playerParty, 0);
    enemyBench.Setup(enemyParty, 1);

    nextEncounterUI.SetActive(false);
    startBattleUI.SetActive(true);
  }

  public void EndEncounter() {
    Destroy(boardObject);
    nextEncounterUI.SetActive(true);
    startBattleUI.SetActive(false);
  }
}
