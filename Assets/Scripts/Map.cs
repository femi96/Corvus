using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Map : MonoBehaviour {

  public Location currentLocation;
  public Player player;
  public Battle battle;

  void Start() {
    player = new Player();
    UpdatePartyUI();
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



  /* Party Management */

  public void PartyBoardAddMonster(int val) {
    int mon = val / 10;
    int pos = val % 10;

    for (int i = 0; i < 3; i++) {
      if (player.party.board[i] == player.party.members[mon])
        player.party.board[i] = null;
    }

    player.party.board[pos] = player.party.members[mon];
    UpdatePartyUI();
  }

  public void PartyBoardRemoveMonster(int pos) {
    player.party.board[pos] = null;
    UpdatePartyUI();
  }



  /* UI */

  [Header("UI Pointers")]
  public GameObject mapUI;
  public GameObject battleUI;
  public GameObject restUI;

  public Text locationUI;

  public GameObject[] partyMemberUI;

  private void UpdateLocationUI() {
    locationUI.text = currentLocation.GetName();
  }

  private void UpdatePartyUI() {
    for (int i = 0; i < 6; i++) {
      Transform t = partyMemberUI[i].transform;
      Monster mon = player.party.members[i];
      t.gameObject.SetActive(mon != null);

      if (mon == null)
        return;

      t.Find("NameText").GetComponent<Text>().text = mon.name;
      t.Find("Pos0Add").gameObject.SetActive(player.party.board[0] != mon);
      t.Find("Pos1Add").gameObject.SetActive(player.party.board[1] != mon);
      t.Find("Pos2Add").gameObject.SetActive(player.party.board[2] != mon);
      t.Find("Pos0Rmv").gameObject.SetActive(player.party.board[0] == mon);
      t.Find("Pos1Rmv").gameObject.SetActive(player.party.board[1] == mon);
      t.Find("Pos2Rmv").gameObject.SetActive(player.party.board[2] == mon);
    }
  }



  /* Battle Events */

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



  /* Rest Events */

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
