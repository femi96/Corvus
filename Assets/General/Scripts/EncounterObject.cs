using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EncounterObject : MonoBehaviour {

  private Encounter encounter;
  private Floor floor;

  private List<EncounterObject> nextEncs = new List<EncounterObject>();

  private bool unlocked = false;
  private bool done = false;
  private bool skipped = false;

  private bool isStart = false;
  private bool isEnd = false;

  public GameObject visualPrefab;
  public GameObject linePrefab;
  public Material startMat;
  public Material endMat;
  public Material defaultMat;

  void Start() {}

  void Update() {}

  void OnMouseDown() {
    ActivateEncounter();
  }

  public void SetEncounter(Encounter e, Floor f) {
    encounter = e;
    floor = f;
  }

  public void SetStart(Floor f) {
    isStart = true;
    floor = f;
    unlocked = true;
  }

  public void SetEnd(Floor f) {
    isEnd = true;
    floor = f;
  }

  public void Skip() {
    skipped = true;
    UpdateVisual();
  }

  public void ActivateEncounter() {
    if (!unlocked || done || skipped)
      return;

    if (isStart) {
      // Nothing
      FinishEncounter();
      return;
    }

    if (isEnd) {
      // New floor
      floor.GenerateEncounterGraph();
      return;
    }

    floor.SetEncounterObject(this);
    floor.LoadEncounter(encounter);
  }

  public void AddNext(EncounterObject e) {
    nextEncs.Add(e);
  }

  public void Unlock() {
    unlocked = true;
    UpdateVisual();
  }

  public bool Unlocked() {
    return unlocked;
  }

  public void FinishEncounter() {
    done = true;

    foreach (EncounterObject e in nextEncs)
      e.Unlock();

    UpdateVisual();
  }

  public void UpdateVisual() {
    // Clear current visual
    foreach (Transform child in transform)
      Destroy(child.gameObject);

    GameObject vgo = Instantiate(visualPrefab, transform);

    if (!unlocked || done || skipped)
      vgo.transform.localScale = Vector3.one * 0.5f;

    MeshRenderer mr = vgo.GetComponent<MeshRenderer>();

    if (isStart) {
      // Set as start node
      mr.material = startMat;
    } else if (isEnd) {
      // Set as end node
      mr.material = endMat;
    } else {
      // Set based on encounter
      mr.material = defaultMat;
    }

    // Lines to neighbors
    foreach (EncounterObject e in nextEncs) {
      LineRenderer lr = Instantiate(linePrefab, transform).GetComponent<LineRenderer>();
      lr.SetPositions(new Vector3[2] {
        transform.position, e.transform.position
      });
    }
  }
}
