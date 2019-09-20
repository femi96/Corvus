using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPrefabs : MonoBehaviour {

  public static UIPrefabs instance;

  public Transform canvasTransform;

  public GameObject hoverPrefab;
  public GameObject playerHoverPrefab;
  public GameObject textPrefab;

  void Awake() {
    instance = this;
  }
}
