using Godot;

public partial class Main : Node
{
    [Export]
    public PackedScene mob_scene;

    public override void _Ready()
    {
        GD.Randomize();
        GetNode<Control>("UserInterface/Retry").Hide();
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event.IsActionPressed("ui_accept") && GetNode<Control>("UserInterface/Retry").Visible)
        {
            // warning-ignore:return_value_discarded
            GetTree().ReloadCurrentScene();
        }
    }

    public void _on_MobTimer_timeout()
    {
        // Create a new instance of the Mob scene.
        var mob = mob_scene.Instantiate() as Mob;

        // Choose a random location on the SpawnPath.
        var mob_spawn_location = GetNode<PathFollow3D>("SpawnPath/SpawnLocation");
        mob_spawn_location.ProgressRatio = (float)GD.Randf();

        // Communicate the spawn location and the player's location to the mob.
        Vector3 player_position = GetNode<Player>("Player").Transform.Origin;
        mob.initialize(mob_spawn_location.Position, player_position);

        // Spawn the mob by adding it to the Main scene.
        AddChild(mob);
        // We connect the mob to the score label to update the score upon squashing a mob.
        mob.Connect("squashed", new Callable(GetNode<ScoreLabel>("UserInterface/ScoreLabel"), "_on_Mob_squashed"));
    }

    public void _on_Player_hit()
    {
        GetNode<Timer>("MobTimer").Stop();
        GetNode<Control>("UserInterface/Retry").Show();
    }
}
