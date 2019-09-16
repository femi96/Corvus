using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterPrefabs : MonoBehaviour {

  public static MonsterPrefabs instance;

  public GameObject ashirePrefab;
  public GameObject shenPrefab;

  void Awake() {
    instance = this;
  }
}
