using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectDelegate : MonoBehaviour {

  public delegate void MoveDelegate(Unit unit);
  public MoveDelegate methodToCall;

  void OnTriggerEnter(Collider other) {
    Unit unit = other.gameObject.GetComponent<Unit>();

    if (methodToCall != null
        && unit != null
        && unit.actionState != ActionState.Dead)
      methodToCall(unit);
  }
}
