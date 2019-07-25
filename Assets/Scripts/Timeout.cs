using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timeout : MonoBehaviour {

  public float duration = 5f;
  private float time = 0f;

  void Update() {
    time += Time.deltaTime;

    if (time > duration)
      Destroy(gameObject);
  }
}
