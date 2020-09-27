using Godot;
using System;

public class player : Node
{
    bulletBrain bulletBrain;
    public bool canShoot = true;

    public bool gameOver = false;
    [Export] public int health = 3;

    [Export] public int bonus = 5;

    public int score = 0;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        bulletBrain = (bulletBrain)GetNode("/root/game/bullets/bulletBrain");
        loadHighScore();
        updateUI();
        

    }

    public override void _Input(InputEvent _inputEvent)
    {
        if((_inputEvent.IsActionPressed("click")) && (gameOver == true))
        {
            GetTree().ReloadCurrentScene();
        }
    }
    public void _on_playerHitZone_area_entered(Area2D bullet)
    {
        var bulletType =(AnimatedSprite)bullet.GetNodeOrNull("AnimatedSprite");

        if((bulletType != null) && (bulletType.Animation == "enemy") && (bullet is bullet))
        {
            //bulletBrain.spawnExplosion(bullet.GlobalPosition, "enemy");
            bulletBrain.CallDeferred("spawnExplosion", bullet.GlobalPosition, "enemy");
            bullet.QueueFree();
            hitPlayer();
        }
    }

    public void hitPlayer(int damageAmount = 1)
    {
        health = Math.Max(health-damageAmount, 0);
        updateUI();

        if((health <= 0) && (gameOver != true))
        {
            gameOver = true;
            canShoot = false;

            var gameOverScreen = (Node2D)GetNode("/root/game/hud/gameOverScreen");
            gameOverScreen.Visible = true;
            // remove the player cannon
            var cannon = (Node2D)GetNode("/root/game/foreground/cannon");
            //bulletBrain.spawnExplosion(cannon.GlobalPosition, "enemy");
            bulletBrain.CallDeferred("spawnExplosion", cannon.GlobalPosition, "enemy");
            cannon.QueueFree();
            
        }

    }

    public void addScore(int scoreAmount = 1)
    {
        score += scoreAmount;
        if((score % bonus) == 0) // Check modulo with bonus
        {
            var healthAnimation = (AnimationPlayer)GetNode("/root/game/hud/healthBonusAnimation");
            healthAnimation.Play("BonusAnim");
            health += 1;
        }
        updateUI();
        bulletBrain.increaseDifficulty();
    }

    public void updateUI()
    {
        var healthAndScore = (Label)GetNode("/root/game/hud/healthAndScore");
        var newHudText = "HEALTH: " + health + "     " + "SCORE: " + score;
        healthAndScore.Text = newHudText;
    }

    public void updateHighScoreUI(string name, string highScore)
    {
        var highScoreDisplay = (Label)GetNode("/root/game/hud/highScoreDisplay");
        var highScoreText = "HIGH SCORE:   " + highScore + " - " + name;
        highScoreDisplay.Text = highScoreText;
    }

    public void loadHighScore()
    {
        string nameHigh = "xxx";
        string score = "0";
        var highScoreSaved = new File();
        if(highScoreSaved.FileExists("res://savedData/highscore.save"))
        {
            var nodeData = new Godot.Collections.Dictionary<string, string>((Godot.Collections.Dictionary)JSON.Parse(highScoreSaved.GetLine()).Result);
            score = nodeData.Values.ToString();
            nameHigh = nodeData.Keys.ToString();
        }
        updateHighScoreUI(nameHigh, score);
    }


//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
