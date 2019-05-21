using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StatusType {
  StatMod
}

public class Status {

  // Stat mod variables
  private Attr attr;
  private int mod;

  public Status(Attr attr, int mod) {
    this.attr = attr;
    this.mod = mod;
  }

  public StatusType GetStatus() {
    return StatusType.StatMod;
  }

  public float GetStatMod() {
    return Mathf.Pow(2, mod);
  }

  public Attr GetStatAttr() {
    return attr;
  }
}