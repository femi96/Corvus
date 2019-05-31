using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Map : MonoBehaviour {

  public Location currentLocation;
  public Player player;
  public Battle battle;

  [Header("UI Pointers")]
  public GameObject mapUI;
  public GameObject battleUI;

  public Text locationUI;

  void Start() {
    player = new Player();
  }

  private void UpdateLocationUI() {
    locationUI.text = currentLocation.GetName();
  }

  public void VisitLocation() {
    if (currentLocation != null)
      currentLocation.Visit(this);
  }

  public void DebugSetLocationBattle() {
    currentLocation = new LocationBattle();
    UpdateLocationUI();
  }

  public void StartBattle(Party allyParty, Party enemyParty) {
    mapUI.SetActive(false);
    battleUI.SetActive(true);

    battle.StartBattle(allyParty, enemyParty);
  }

  public void EndBattle() {
    mapUI.SetActive(true);
    battleUI.SetActive(false);

    if (battle.battleState == BattleState.Win)
      currentLocation.Clear(1);
    else
      currentLocation.Clear(0);
  }
}
