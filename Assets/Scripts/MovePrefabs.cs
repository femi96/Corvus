﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePrefabs : MonoBehaviour {

  public static MovePrefabs instance;
  public static Transform container;

  public GameObject scratchPrefab;
  public GameObject bangPrefab;

  void Awake() {
    instance = this;
    container = transform;
  }
}