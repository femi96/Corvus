using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterPrefabs : MonoBehaviour {

  public static MonsterPrefabs instance;

  public GameObject unitPrefab;
  public GameObject debugAnchorPrefab;

  public GameObject ashirePrefab;
  public GameObject shenPrefab;

  void Awake() {
    instance = this;
  }
}
