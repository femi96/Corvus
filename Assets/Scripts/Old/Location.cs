using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LocationType {
  None, Start, End, Battle, Rest
}

public class Location {

  public List<Location> neighbors = new List<Location>();
  private string name = "None";
  private float x;
  private float y;
  private LocationType type = LocationType.None;
  public int cost = 0;
  public int[] budgetTotal = new int[] { 0, 0 };
  public int[] budget = new int[] { 0, 0 };

  // Battle starts
  public Party enemyParty;


  public Location() : this(0f, 0f) {}

  public Location(float x, float y) {
    this.x = x;
    this.y = y;
  }



  /* Location functionality */

  public LocationType GetLocationType() { return type; }

  public string GetName() { return name; }



  /* Location type setting */

  public void CreateStart() {
    type = LocationType.Start;
    name = "Start";
  }

  public void CreateEnd() {
    type = LocationType.End;
    name = "End";
  }

  public void CreateBattle() {
    type = LocationType.Battle;
    name = "Battle " + Random.Range(0, 1000);
    GenerateEnemy();
  }

  private void GenerateEnemy() {
    Monster[] p = new Monster[6];

    for (int i = 0; i < 6; i += 1) {
      p[i] = new Monster("Enemy" + i);
    }

    enemyParty = new Party(p);
  }

  public void CreateRest() {
    type = LocationType.Rest;
    name = "Rest " + Random.Range(0, 1000);
  }



  /* Map connectons */

  public void AddNeighbor(Location loc) {
    if (loc != this)
      neighbors.Add(loc);
  }

  public void ClearNeighbors() {
    neighbors = new List<Location>();
  }

  public void CreateCoords(float x, float y) {
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
