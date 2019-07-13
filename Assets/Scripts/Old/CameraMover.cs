using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMover : MonoBehaviour {

  private bool active = false;

  private Vector3 anchor;
  private Vector3 target;

  private Vector3 startPos;
  private Vector3 targetPos;

  private float moveTime = 0f;
  public float moveDuration = 1f;

  public Vector3 targetPullFactor;

  void Awake() {
    anchor = transform.position;
  }

  void Update() {

    if (!active) {
      transform.position = anchor;
      return;
    }

    moveTime += Time.deltaTime;

    float i = Mathf.Min(moveTime / moveDuration, 1f);
    i = 0.5f - 0.5f * Mathf.Cos(i * Mathf.PI);

    float j = 1f - i;
    transform.position = j * startPos + i * targetPos;


    /*
    Max then slow

    Vector3 i = targetPullFactor;
    Vector3 j = Vector3.one - i;
    Vector3 targetPos = Vector3.Scale(anchor, j) + Vector3.Scale(target, i);

    float moveSpeed = speed * (targetPos - transform.position).magnitude;
    moveSpeed = Mathf.Min(moveSpeed, maxSpeed);
    moveSpeed = Mathf.Max(moveSpeed, minSpeed);
    Debug.Log(moveSpeed);

    float moveStep = Time.deltaTime * moveSpeed;
    transform.position = Vector3.MoveTowards(transform.position, targetPos, moveStep);
    */

    // float targetStep = maxSpeed * Time.deltaTime;
    // transform.position = Vector3.MoveTowards(transform.position, targetPos, targetStep);
    // transform.LookAt(target);
  }

  public void SetAnchor(Vector3 v) {
    active = true;
    target = v;
    moveTime = 0;

    Vector3 i = targetPullFactor;
    Vector3 j = Vector3.one - i;

    startPos = transform.position;
    targetPos = Vector3.Scale(anchor, j) + Vector3.Scale(target, i);
  }
}
