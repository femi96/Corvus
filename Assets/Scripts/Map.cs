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
  public GameObject restUI;

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

  // Debug Set Locations

  public void DebugSetLocationBattle() {
    currentLocation = new LocationBattle();
    UpdateLocationUI();
  }

  public void DebugSetLocationRest() {
    currentLocation = new LocationRest();
    UpdateLocationUI();
  }

  // Battle Events

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

  // Rest Events

  public void StartRest() {
    mapUI.SetActive(false);
    restUI.SetActive(true);
  }

  public void EndRest() {
    mapUI.SetActive(true);
    restUI.SetActive(false);
  }

  public void RestParty() {
    foreach (Monster m in player.party.members) {
      float f = 5.0f + m.GetAttribute(Attr.Vit);
      m.Heal(f);
    }

    EndRest();
  }
}
