using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Location {

  public HashSet<Location> neighbors = new HashSet<Location>();
  private float x;
  private float y;

  public Location() : this(0f, 0f) {
  }

  public Location(float x, float y) {
    this.x = x;
    this.y = y;
  }

  public virtual string GetName() { return "None"; }

  public virtual void Visit(Map map) {}

  public virtual void Clear(int result) {}

  public void AddNeighbor(Location loc) {
    if (loc != this)
      neighbors.Add(loc);
  }

  public void ClearNeighbors() {
    neighbors = new HashSet<Location>();
  }

  public void SetCoords(float x, float y) {
    this.x = x;
    this.y = y;
  }

  public void ShiftCoords(float dx, float dy) {
    this.x += dx;
    this.y += dy;
  }

  public float GetX() {
    return x;
  }

  public float GetY() {
    return y;
  }

  public float GetDistance(Location other) {
    float dx = GetX() - other.GetX();
    float dy = GetY() - other.GetY();
    return Mathf.Sqrt((dx * dx) + (dy * dy));
  }
}
