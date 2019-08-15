using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIColor : MonoBehaviour {

  public static UIColor instance;

  public Color ally;
  public Color enemy;

  void Awake() {
    UIColor.instance = this;
  }

  public static Color Ally() { return instance.ally; }
  public static Color AllyLight() { return 0.5f * UIColor.Ally() + 0.5f * Color.white; }
  public static Color Enemy() { return instance.enemy; }
  public static Color EnemyLight() { return 0.5f * UIColor.Enemy() + 0.5f * Color.white; }
}
