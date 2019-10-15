using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floor : MonoBehaviour {

  public Player player;

  public GameObject encounterObjectPrefab;
  public Transform floorMapContainer;

  private GameObject boardObject;
  private List<EncounterObject> encounterObjects;
  private EncounterObject currentEncounterObject;

  [Header("UI")]
  public GameObject nextEncounterUI;
  public GameObject startBattleUI;

  void Start() {
    startBattleUI.SetActive(false);
    GenerateEncounterGraph();
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

    floorMapContainer.gameObject.SetActive(false);
  }

  public void EndEncounter() {
    Destroy(boardObject);
    nextEncounterUI.SetActive(true);
    startBattleUI.SetActive(false);

    foreach (EncounterObject e in encounterObjects) {
      if (!e.Unlocked())
        continue;

      if (e != currentEncounterObject)
        e.Skip();
    }

    currentEncounterObject.FinishEncounter();
    floorMapContainer.gameObject.SetActive(true);
  }

  public void SetEncounterObject(EncounterObject e) {
    currentEncounterObject = e;
  }

  public void GenerateEncounterGraph() {
    foreach (Transform child in floorMapContainer)
      Destroy(child.gameObject);

    const float xPadding = 1f;
    const float xStart = -3.5f;
    const float yPadding = 1f;

    // Generate an encounter graph with 6 layers
    List<int> layerWidths = new List<int>() { 2, 2, 3, 3 };
    layerWidths.Add(Random.Range(2, 4));
    layerWidths.Add(Random.Range(2, 4));
    List<int> layerWidthsInOrder = new List<int>();

    while (layerWidths.Count > 0) {
      int i = UnityEngine.Random.Range(0, layerWidths.Count);
      layerWidthsInOrder.Add(layerWidths[i]);
      layerWidths.RemoveAt(i);
    }

    encounterObjects = new List<EncounterObject>();

    List<EncounterObject> prevEncs;
    List<EncounterObject> currEncs;

    prevEncs = new List<EncounterObject>();
    EncounterObject startEnc = Instantiate(encounterObjectPrefab, floorMapContainer).GetComponent<EncounterObject>();
    startEnc.SetStart(this);
    startEnc.transform.position = Vector3.right * (xPadding * (xStart + 0));
    prevEncs.Add(startEnc);
    encounterObjects.Add(startEnc);

    while (layerWidthsInOrder.Count > 0) {
      currEncs = new List<EncounterObject>();
      int count = layerWidthsInOrder[0];
      int x = 7 - layerWidthsInOrder.Count;
      float yStart = -(count - 1) / 2f;

      for (int i = 0; i < count; i++) {
        EncounterObject newEnc = Instantiate(encounterObjectPrefab, floorMapContainer).GetComponent<EncounterObject>();
        newEnc.SetEncounter(new Encounter(), this);
        newEnc.transform.position = Vector3.right * (xPadding * (xStart + x))
                                    + Vector3.forward * (yPadding * (yStart + i));
        currEncs.Add(newEnc);
        encounterObjects.Add(newEnc);
      }

      // Connect prev to current (adjacent only)
      for (int i = 0; i < prevEncs.Count; i++) {
        EncounterObject pe = prevEncs[i];
        float iv = i * 1f  / prevEncs.Count;
        float id = 1 * 1f  / prevEncs.Count;

        for (int j = 0; j < currEncs.Count; j++) {
          EncounterObject ce = currEncs[j];
          float jv = j * 1f / currEncs.Count;
          float jd = 1 * 1f  / currEncs.Count;

          if ((jv <= iv && jv + jd > iv) || (jv >= iv && jv - jd < iv)
              || (iv <= jv && iv + id > jv) || (iv >= jv && iv - id < jv))
            pe.AddNext(ce);
        }
      }

      layerWidthsInOrder.RemoveAt(0);
      prevEncs = currEncs;
    }

    currEncs = new List<EncounterObject>();
    EncounterObject endEnc = Instantiate(encounterObjectPrefab, floorMapContainer).GetComponent<EncounterObject>();
    endEnc.SetEnd(this);
    endEnc.transform.position = Vector3.right * (xPadding * (xStart + 7));
    currEncs.Add(endEnc);
    encounterObjects.Add(endEnc);

    // Connect prev to end
    foreach (EncounterObject pe in prevEncs) {
      pe.AddNext(endEnc);
    }

    // Update visual
    foreach (EncounterObject e in encounterObjects) {
      e.UpdateVisual();
    }
  }
}
