using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClickSelection : MonoBehaviour {

  public Unit selectedUnit;
  private Board board;

  public void OnClickUnit(Unit unit) {
    if (selectedUnit == null || board.battleState != BattleState.Off)
      // Select
      selectedUnit = unit;
    else {
      // Swap
      if (!unit.currentTile.tileHolder.CanMoveTo(selectedUnit.team, unit.currentTile)) {
        selectedUnit = unit;
        return;
      }

      Tile tempTile = selectedUnit.currentTile;
      selectedUnit.MoveToTile(unit.currentTile, true);
      unit.MoveToTile(tempTile);
      selectedUnit = null;
    }
  }

  public void OnClickTile(Tile tile) {
    if (selectedUnit != null && board.battleState == BattleState.Off) {
      if (!tile.tileHolder.CanMoveTo(selectedUnit.team, tile)) {
        selectedUnit = null;
        return;
      }

      if (tile.unit != null) {
        // Swap if other
        tile.unit.MoveToTile(selectedUnit.currentTile, true);
      }

      // Move to tile
      selectedUnit.MoveToTile(tile);
      selectedUnit = null;
    }
  }

  void Awake() {
    board = GetComponent<Board>();

    nameText = selectionUI.transform.Find("Name").gameObject.GetComponent<Text>();
    teamImage = selectionUI.transform.Find("Team").gameObject.GetComponent<Image>();

    health = selectionUI.transform.Find("Health").gameObject;
    healthText = selectionUI.transform.Find("HealthTxt").gameObject.GetComponent<Text>();
    healthImage = health.GetComponent<Image>();
    energy = selectionUI.transform.Find("Energy").gameObject;
    energyText = selectionUI.transform.Find("EnergyTxt").gameObject.GetComponent<Text>();

    strText = selectionUI.transform.Find("Attribute/Str").gameObject.GetComponent<Text>();
    agiText = selectionUI.transform.Find("Attribute/Agi").gameObject.GetComponent<Text>();
    wisText = selectionUI.transform.Find("Attribute/Wis").gameObject.GetComponent<Text>();
    vitText = selectionUI.transform.Find("Attribute/Vit").gameObject.GetComponent<Text>();
    reaText = selectionUI.transform.Find("Attribute/Rea").gameObject.GetComponent<Text>();
    wilText = selectionUI.transform.Find("Attribute/Wil").gameObject.GetComponent<Text>();
  }

  void Update() {}

  void LateUpdate() {
    UpdateUI();
  }

  public GameObject selectionUI;
  private Text nameText;
  private Image teamImage;

  private GameObject health;
  private Text healthText;
  private Image healthImage;

  private GameObject energy;
  private Text energyText;

  private Text strText, agiText, wisText, vitText, reaText, wilText;

  private void UpdateUI() {
    selectionUI.SetActive(selectedUnit != null);

    if (!selectedUnit)
      return;

    // General
    nameText.text = selectedUnit.monster.GetName();
    int team = (selectedUnit.team == 0) ? 1 : 0;

    teamImage.color = team * UIColor.AllyLight() + (1 - team) * UIColor.EnemyLight();


    // Health
    float healthWidth = 360f * selectedUnit.currentHealth / selectedUnit.monster.MaxHealth();
    health.GetComponent<RectTransform>().sizeDelta = new Vector2(healthWidth, 40);
    healthText.text = selectedUnit.currentHealth + " / " + selectedUnit.monster.MaxHealth();
    healthImage.color = team * UIColor.Ally() + (1 - team) * UIColor.Enemy();

    // Energy
    float energyWidth = 360f * selectedUnit.currentEnergy / 100f;
    energy.GetComponent<RectTransform>().sizeDelta = new Vector2(energyWidth, 40);
    energyText.text = selectedUnit.currentEnergy + " / " + 100f;

    // Attributes
    strText.text = selectedUnit.monster.GetAttribute(Attribute.Str) + "";
    agiText.text = selectedUnit.monster.GetAttribute(Attribute.Agi) + "";
    wisText.text = selectedUnit.monster.GetAttribute(Attribute.Wis) + "";
    vitText.text = selectedUnit.monster.GetAttribute(Attribute.Vit) + "";
    reaText.text = selectedUnit.monster.GetAttribute(Attribute.Rea) + "";
    wilText.text = selectedUnit.monster.GetAttribute(Attribute.Wil) + "";
  }
}