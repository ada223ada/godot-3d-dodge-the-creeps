using Godot;

public partial class ScoreLabel : Label
{
    private int score = 0;

    public void _on_Mob_squashed()
    {
        GD.Print("_on_Mob_squashed");
        score += 1;
        Text = $"Score: {score}";
    }
}
