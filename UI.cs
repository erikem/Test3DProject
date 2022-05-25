using Godot;
using System;

public class UI : Control
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.
    private TextureProgress HealthBar;
    private Label GoldText;
    public override void _Ready()
    {
        HealthBar = GetNode("HealthBar") as TextureProgress;
        GoldText = GetNode("GoldAmount") as Label;
    }

    public void UpdateHealthBar(float curr, float max)
    {
        HealthBar.Value = 100 * curr / max;
    }
    public void UpdateGoldText(float amount)
    {
        GoldText.Text = "Gold: " + Mathf.Round(amount).ToString();
    }

    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
    //  public override void _Process(float delta)
    //  {
    //      
    //  }
}
