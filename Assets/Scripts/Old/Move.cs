using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TargetType {
  Single, Multi, Self
}

public abstract class Move {

  public void Act(Monster user, Monster target, int stage) {
    Act(user, new Monster[] { target }, stage);
  }

  /* Abstract/virtual functions for specific overwrites */

  public abstract string GetName();

  public abstract void Act(Monster user, Monster[] targets, int stage);

  public abstract float GetPower();

  public abstract float GetScale(Monster user);

  public abstract TargetType[] GetTargetTypes();
}
