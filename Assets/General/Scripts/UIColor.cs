using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct AffinityColor {
  public Affinity a;
  public Color c;
}

public class UIColor : MonoBehaviour {

  public static UIColor instance;

  public Color ally;
  public Color enemy;

  public Color damPhy;
  public Color damMag;
  public Color damTrue;
  public Color damMiss;

  public AffinityColor[] affinityColors;

  void Awake() {
    UIColor.instance = this;
  }

  public static Color Ally() { return instance.ally; }
  public static Color AllyLight() { return 0.5f * UIColor.Ally() + 0.5f * Color.white; }
  public static Color Enemy() { return instance.enemy; }
  public static Color EnemyLight() { return 0.5f * UIColor.Enemy() + 0.5f * Color.white; }

  public static Color DamagePhysical() { return instance.damPhy; }
  public static Color DamageMagical() { return instance.damMag; }
  public static Color DamageTrue() { return instance.damTrue; }
  public static Color DamageMiss() { return instance.damMiss; }

  public static Color AffinityColor(Affinity a) {
    foreach (AffinityColor ac in instance.affinityColors)
      if (a == ac.a)
        return ac.c;

    return Color.white;
  }
}
