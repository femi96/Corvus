using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Map : MonoBehaviour {

  public Location currentLocation;
  public Player player;

  [Header("UI Pointers")]
  public Text locationUI;

  void Start() {
    player = new Player();
  }

  private void UpdateLocationUI() {
    locationUI.text = currentLocation.GetName();
  }

  public void VisitLocation() {
    if (currentLocation != null)
      currentLocation.Visit(player);
  }

  public void DebugSetLocationBattle() {
    currentLocation = new LocationBattle();
    UpdateLocationUI();
  }
}
