using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPointers : MonoBehaviour {

  public static UIPointers instance;

  public GameObject startBattleButton;
  public Text unitCountText;
  public Text winCountText;
  public Text roundText;

  void Awake() {
    instance = this;
  }
}
