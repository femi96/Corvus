using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectMover : MonoBehaviour {
  public Vector3 velocity = Vector3.zero;

  void Update() {
    transform.position += velocity * Time.deltaTime;
  }
}
