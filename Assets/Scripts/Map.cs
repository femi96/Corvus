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
    visitableLocoations = new List<Location> { locations[0] };
    GoToLocation(locations[0]);
  }

  void Update() {
    if (debugReGenMap) {
      debugReGenMap = false;
      GenerateMap();
    }
  }



  /* Location managements */

  private int mapLayers = 6;
  private List<Location>[] lls;
  private List<Location> visitableLocoations;

  public void VisitLocation() {
    if (currentLocation == null)
      return;

    switch (currentLocation.GetLocationType()) {
    case LocationType.Start:
      UnlockLocations();
      break;

    case LocationType.Battle:
      StartBattle(player.party, currentLocation.enemyParty);
      break;

    case LocationType.Rest:
      StartRest();
      break;
    }
  }

  public void GoToLocation(Location loc) {
    if (!visitableLocoations.Contains(loc))
      return;

    currentLocation = loc;
    UpdateLocationUI();
    visitableLocoations = new List<Location>();
  }

  public void UnlockLocations() {
    visitableLocoations = currentLocation.neighbors;
  }



  /* Location generation */

  private void GenerateMap() {

    float mapWidth = 20f;
    float mapHeight = 10f;

    List<Location> ls = new List<Location>();

    // Start & end node
    Location start = new Location(0f, Random.Range(0f, mapHeight));
    ls.Add(start);
    Location end = new Location(mapWidth, Random.Range(0f, mapHeight));
    ls.Add(end);


    // Generate location layers
    lls = new List<Location>[mapLayers];
    int layerCount = 3;
    float layerWidth = mapWidth / (mapLayers + 1);

    for (int i = 0; i < mapLayers; i++) {
      lls[i] = new List<Location>();
      layerCount = Mathf.RoundToInt(Random.Range(3f, 5f));
      float widthOffset = 0;
      float heightOffset = -Random.Range(0.75f, 1.5f) / (layerCount - 1f);
      float widthOffsetNew = 0;
      float heightOffsetNew = 0;

      for (int j = 0; j < layerCount; j++) {
        bool toClose = true;
        Location loc = new Location(-10f, -10f);

        while (toClose) {
          widthOffsetNew = Mathf.Clamp(widthOffset + Random.Range(-0.5f, 0.5f), -0.5f, 0.5f);
          heightOffsetNew = heightOffset + Random.Range(0.75f, 1.5f) / (layerCount - 1f);

          loc = new Location(
            layerWidth * (i + 1 + widthOffsetNew),
            mapHeight * heightOffsetNew);

          toClose = false;

          foreach (Location oldLoc in lls[i])
            toClose = toClose || loc.GetDistance(oldLoc) < 2f;
        }

        widthOffset = widthOffsetNew;
        heightOffset = heightOffsetNew;
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
        lls[i][j].cost = 3;

        if (i == 2 || i == 4 || i == 5)
          lls[i][j].cost = 4;

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
      GenerateStepPushNodes(2f);
    }

    GenerateLocationTypes();
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
  }

  private void GenerateLocationTypes() {
    locations[0].CreateStart();
    locations[1].CreateEnd();

    int riskBudget = 18;
    int rewardBudget = 6;
    LocationAssignment(new int[] { riskBudget, rewardBudget }, locations[0]);
  }

  private int[] LocationAssignment(int[] budget, Location loc) {
    // Recursive location assignment that takes available budget and returns budget used
    // Debug.Log(loc.GetLocationType());

    int[] usableBudget = new int[2];
    int[] usedBudget = new int[2];
    int[] childUsedBudget = new int[2];
    int[] childAvailableBudget = new int[2];

    for (int i = 0; i < budget.Length; i++)
      childAvailableBudget[i] = budget[i];

    // Skip if already reached
    if (loc.GetLocationType() != LocationType.None && loc.GetLocationType() != LocationType.Start && loc.GetLocationType() != LocationType.End)
      return loc.budgetTotal;

    // Skip end node
    if (loc.GetLocationType() == LocationType.End) {
      loc.budget = new int[] { 0, 0 };
      loc.budgetTotal = loc.budget;
      return loc.budgetTotal;
    }


    // For neighbors
    foreach (Location n in loc.neighbors) {
      childUsedBudget = LocationAssignment(childAvailableBudget, n);
      childAvailableBudget = childUsedBudget;
    }

    // Skip start node
    if (loc.GetLocationType() == LocationType.Start) {
      loc.budget = new int[] { 0, 0 };
      loc.budgetTotal = childUsedBudget;
      return loc.budgetTotal;
    }


    // Set current location
    for (int i = 0; i < budget.Length; i++) {
      usedBudget[i] = childUsedBudget[i];
      usableBudget[i] = budget[i] - usedBudget[i];
    }

    int total = loc.cost;
    int risk = 0;
    int reward = 0;
    Random.Range(Mathf.Max(3 - usableBudget[1], 0), Mathf.Min(usableBudget[0] + 1, 4));
    Mathf.Max(3 - risk, 0);

    for (int i = 0; i < total; i++) {
      float rewardChance = usableBudget[1] / (0f + usableBudget[0] + usableBudget[1]);

      if (Random.Range(0f, 1f) < rewardChance) {
        if (reward < usableBudget[1])
          reward += 1;
        else if (risk < usableBudget[0])
          risk += 1;
      } else {
        if (risk < usableBudget[0])
          risk += 1;
        else if (reward < usableBudget[1])
          reward += 1;
      }
    }

    usedBudget[0] += risk;
    usedBudget[1] += reward;

    if (risk > reward)
      loc.CreateBattle();
    else
      loc.CreateRest();

    loc.budget = new int[] { risk, reward };
    loc.budgetTotal = usedBudget;
    return loc.budgetTotal;
  }



  /* Debug Functions */

  [Header("Debug Variables")]
  public bool debugReGenMap = false;
  public GameObject debugLocPrefab;
  public GameObject debugLocRiskPrefab;
  public GameObject debugLocRewPrefab;
  public GameObject debugPathPrefab;

  private void DebugDisplayLocations() {
    foreach (Transform child in transform)
      Destroy(child.gameObject);

    foreach (Location loc in locations) {
      GameObject go;
      GameObject locGo = Instantiate(debugLocPrefab, transform);
      locGo.transform.position = new Vector3(loc.GetX(), loc.GetY(), 4);
      locGo.GetComponent<GoOnClick>().location = loc;
      locGo.GetComponent<GoOnClick>().map = this;

      foreach (Location n in loc.neighbors) {
        go = Instantiate(debugPathPrefab, transform);
        go.GetComponent<LineRenderer>().SetPositions(new Vector3[] {
          new Vector3(loc.GetX(), loc.GetY(), 4), new Vector3(n.GetX(), n.GetY(), 4)
        });
      }

      for (int i = 0; i < loc.budget[0]; i++) {
        go = Instantiate(debugLocRiskPrefab, locGo.transform);
        go.transform.position = new Vector3(loc.GetX() + (0.25f * (i - (loc.budget[0] / 2f))), loc.GetY(), 3.75f);
      }

      for (int i = 0; i < loc.budget[1]; i++) {
        go = Instantiate(debugLocRewPrefab, locGo.transform);
        go.transform.position = new Vector3(loc.GetX() + (0.25f * (i - (loc.budget[1] / 2f))), loc.GetY() - 0.25f, 3.75f);
      }
    }
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

    //if (battle.battleState == BattleState.Win)
    // Win
    //else
    // Lose
    UnlockLocations();
  }



  /* Rest Events */

  public void StartRest() {
    mapUI.SetActive(false);
    restUI.SetActive(true);
  }

  public void EndRest() {
    mapUI.SetActive(true);
    restUI.SetActive(false);
    UnlockLocations();
  }

  public void RestParty() {
    foreach (Monster m in player.party.members) {
      if (m == null)
        continue;

      float f = 5.0f + m.GetAttribute(Attr.Vit);
      m.Heal(f);
    }

    EndRest();
  }
}
