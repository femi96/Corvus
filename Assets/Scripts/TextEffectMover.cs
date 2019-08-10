using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextEffectMover : MonoBehaviour {
  public Vector3 position = Vector3.zero;
  public Vector3 velocity = Vector3.zero;
  public Vector3 acceleration = Vector3.zero;

  void Update() {
    velocity += acceleration * Time.deltaTime;
    position += velocity * Time.deltaTime;
  }
}
