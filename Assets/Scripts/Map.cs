using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Map : MonoBehaviour {

  public Location currentLocation;
  public Player player;
  public Battle battle;

  public List<Location> locations;

  void Start() {
    player = new Player();
    UpdatePartyUI();
    GenerateMap();
  }

  void Update() {
    if (debugReGenMap) {
      debugReGenMap = false;
      GenerateMap();
    }

    if (debugPushMap) {
      debugPushMap = false;
      GenerateStepPushNodes(2f);
    }

    if (debugPullMap) {
      debugPullMap = false;
      GenerateStepPullNodes(2.5f);
    }
  }



  /* Location managements */

  public void VisitLocation() {
    if (currentLocation != null)
      currentLocation.Visit(this);
  }

  private void GenerateMap() {

    float mapWidth = 20f;
    float mapHeight = 10f;
    int mapLayers = 6;

    List<Location> ls = new List<Location>();

    // Start & end node
    Location start = new Location(0f, Random.Range(0f, mapHeight));
    ls.Add(start);
    Location end = new Location(mapWidth, Random.Range(0f, mapHeight));
    ls.Add(end);


    // Generate location layers
    List<Location>[] lls = new List<Location>[mapLayers];
    int layerCount = 3;
    float layerWidth = mapWidth / (mapLayers + 1);

    for (int i = 0; i < mapLayers; i++) {
      lls[i] = new List<Location>();
      layerCount = Mathf.RoundToInt(Random.Range(3f, 5f));
      float widthOffset = 0;
      float heightOffset = 0;

      for (int j = 0; j < layerCount; j++) {

        widthOffset = Mathf.Clamp(widthOffset + Random.Range(-0.5f, 0.5f), -0.5f, 0.5f);
        heightOffset += Random.Range(0.5f, 1.5f) / (layerCount - 1f);

        Location loc = new Location(
          layerWidth * (i + 1 + widthOffset),
          // layerWidth * (i + 1),
          mapHeight * heightOffset);
        // Random.Range(0f, mapHeight));
        // mapHeight * j / (layerCount - 1f));
        lls[i].Add(loc);
      }

      lls[i].Sort(delegate(Location a, Location b) {
        return (a.GetY()).CompareTo(b.GetY());
      });
    }


    // Connect location layers
    for (int i = 0; i < mapLayers - 1; i++) {
      int a = lls[i].Count;
      int b = lls[i + 1].Count;
      int minEdgeCount = Mathf.Max(a, b);
      int maxEdgeCount = a + b - 1;
      int edgeCount = Mathf.RoundToInt(Random.Range(minEdgeCount * 1f, maxEdgeCount - 1f));
      int edgeCountSave = edgeCount;

      int[] edgeCounts = new int[a];

      for (int j = 0; j < a; j++) {
        edgeCounts[j] = 1;
        edgeCount -= 1;
      }

      while (edgeCount > 0) {
        edgeCounts[Random.Range(0, a)] += 1;
        edgeCount -= 1;
      }

      int left = 0;
      int count = 0;

      for (int j = 0; j < a; j++) {
        int edgesRemaining = edgeCountSave - count;
        int leftRemaining = b - left;

        bool skipLeftMust = (edgesRemaining < leftRemaining);
        bool skipLeftMustNot = (left == 0 && count == 0);
        bool skipLeftChance = Random.Range(0f, 1f) > 0.5f;

        bool skipLeft = (skipLeftMust || skipLeftChance) && !skipLeftMustNot;

        if (skipLeft)
          left = Mathf.Min(left + 1, b - 1);

        while (edgeCounts[j] > 0) {
          lls[i][j].AddNeighbor(lls[i + 1][left]);
          edgeCounts[j] -= 1;
          count += 1;

          if (edgeCounts[j] > 0)
            left = Mathf.Min(left + 1, b - 1);
        }
      }
    }


    // Connect start
    for (int j = 0; j < lls[0].Count; j++) {
      start.AddNeighbor(lls[0][j]);
    }


    // Connect end
    for (int j = 0; j < lls[mapLayers - 1].Count; j++) {
      lls[mapLayers - 1][j].AddNeighbor(end);
    }


    // Add location layers to locations
    for (int i = 0; i < mapLayers; i++) {
      for (int j = 0; j < lls[i].Count; j++) {
        ls.Add(lls[i][j]);
      }
    }


    // Color locations by lands
    // Land A layer of nodes
    // Land A + B layer of nodes
    // Land B layer of nodes
    // Maybe use voronni for lands
    locations = ls;

    for (int i = 0; i < 4; i++) {
      GenerateStepPullNodes(2.5f);
      GenerateStepPushNodes(2f);
    }

    DebugDisplayLocations();
  }

  private void GenerateStepPullNodes(float pullRadius) {
    foreach (Location l in locations) {
      foreach (Location n in l.neighbors) {
        float d = l.GetDistance(n);

        if (d > pullRadius) {
          float f = 1f - (pullRadius / d);
          float dx = l.GetX() - n.GetX();
          float dy = l.GetY() - n.GetY();
          float nx = dx * f / 2f;
          float ny = dy * f / 2f;

          if (l != locations[0] && l != locations[1])
            l.ShiftCoords(-nx, -ny);

          if (n != locations[0] && n != locations[1])
            n.ShiftCoords(nx, ny);
        }
      }
    }

    DebugDisplayLocations();
  }

  private void GenerateStepPushNodes(float pushRadius) {
    foreach (Location l in locations) {
      foreach (Location n in locations) {
        if (l == n)
          continue;

        float d = l.GetDistance(n);

        if (d < pushRadius) {
          float f = (pushRadius / d) - 1f;
          float dx = l.GetX() - n.GetX();
          float dy = l.GetY() - n.GetY();
          float nx = dx * f / 2f;
          float ny = dy * f / 2f;
          l.ShiftCoords(nx, ny);
          n.ShiftCoords(-nx, -ny);
        }
      }
    }

    DebugDisplayLocations();
  }



  /* Debug Set Functions */

  [Header("Debug Variables")]
  public bool debugReGenMap = false;
  public bool debugPushMap = false;
  public bool debugPullMap = false;
  public GameObject debugLocPrefab;
  public GameObject debugPathPrefab;

  private void DebugDisplayLocations() {
    foreach (Transform child in transform)
      Destroy(child.gameObject);

    foreach (Location loc in locations) {
      GameObject go = Instantiate(debugLocPrefab, transform);
      go.transform.position = new Vector3(loc.GetX(), loc.GetY(), 4);

      foreach (Location n in loc.neighbors) {
        go = Instantiate(debugPathPrefab, transform);
        go.GetComponent<LineRenderer>().SetPositions(new Vector3[] {
          new Vector3(loc.GetX(), loc.GetY(), 4), new Vector3(n.GetX(), n.GetY(), 4)
        });
      }
    }
  }

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
