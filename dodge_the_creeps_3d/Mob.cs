using Godot;

public partial class Mob : CharacterBody3D
{
    // Emitted when the player jumped on the mob.
    [Signal]
    public delegate void squashedEventHandler();

    // Minimum speed of the mob in meters per second.
    [Export]
    public float min_speed = 10;
    
    // Maximum speed of the mob in meters per second.
    [Export]
    public float max_speed = 18;

    private Vector3 velocity = Vector3.Zero;

    public override void _PhysicsProcess(double delta)
    {
        // warning-ignore:return_value_discarded
        SetVelocity(velocity);
        MoveAndSlide();
    }

    public void initialize(Vector3 start_position, Vector3 player_position)
    {
        LookAtFromPosition(start_position, player_position, Vector3.Up);
        RotateY((float)GD.RandRange(-Mathf.Pi / 4, Mathf.Pi / 4));

        float random_speed = (float)GD.RandRange(min_speed, max_speed);
        // We calculate a forward velocity first, which represents the speed.
        velocity = Vector3.Forward * random_speed;
        // We then rotate the vector based on the mob's Y rotation to move in the direction it's looking.
        velocity = velocity.Rotated(Vector3.Up, Rotation.Y);

        GetNode<AnimationPlayer>("AnimationPlayer").SetSpeedScale(random_speed / min_speed);
    }

    public void squash()
    {
        EmitSignal(nameof(squashed));
        QueueFree();
    }

    public void _on_VisibilityNotifier_screen_exited()
    {
        QueueFree();
    }
}
