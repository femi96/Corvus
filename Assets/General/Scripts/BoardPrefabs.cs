using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardPrefabs : MonoBehaviour {

  public static BoardPrefabs instance;

  public GameObject board1Prefab;

  void Awake() {
    instance = this;
  }
}
