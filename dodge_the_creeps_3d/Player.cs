using Godot;

public partial class Player : CharacterBody3D
{
    [Signal]
    public delegate void hitEventHandler();

    // How fast the player moves in meters per second.
    [Export]
    public float speed = 14;
    
    // Vertical impulse applied to the character upon jumping in meters per second.
    [Export]
    public float jump_impulse = 20;
    
    // Vertical impulse applied to the character upon bouncing over a mob in meters per second.
    [Export]
    public float bounce_impulse = 16;
    
    // The downward acceleration when in the air, in meters per second per second.
    [Export]
    public float fall_acceleration = 75;

    private Vector3 velocity = Vector3.Zero;

    public override void _PhysicsProcess(double delta)
    {
        Vector3 direction = Vector3.Zero;
        
        if (Input.IsActionPressed("move_right"))
            direction.X += 1;
        if (Input.IsActionPressed("move_left"))
            direction.X -= 1;
        if (Input.IsActionPressed("move_back"))
            direction.Z += 1;
        if (Input.IsActionPressed("move_forward"))
            direction.Z -= 1;

        if (direction != Vector3.Zero)
        {
            // In the lines below, we turn the character when moving and make the animation play faster.
            direction = direction.Normalized();
            GetNode<Node3D>("Pivot").LookAt(Position + direction, Vector3.Up);
            GetNode<AnimationPlayer>("AnimationPlayer").SetSpeedScale(4);
        }
        else
        {
            GetNode<AnimationPlayer>("AnimationPlayer").SetSpeedScale(1);
        }

        velocity.X = direction.X * speed;
        velocity.Z = direction.Z * speed;

        // Jumping.
        if (IsOnFloor() && Input.IsActionJustPressed("jump"))
        {
            velocity.Y += jump_impulse;
        }

        // We apply gravity every frame so the character always collides with the ground when moving.
        // This is necessary for the IsOnFloor() function to work as a body can always detect
        // the floor, walls, etc. when a collision happens the same frame.
        velocity.Y -= (float)(fall_acceleration * delta);
        SetVelocity(velocity);
        SetUpDirection(Vector3.Up);
        MoveAndSlide();
        velocity = Velocity;
        
        // Here, we check if we landed on top of a mob and if so, we kill it and bounce.
        // With MoveAndSlide(), Godot makes the body move sometimes multiple times in a row to
        // smooth out the character's motion. So we have to loop over all collisions that may have
        // happened.
        // If there are no "slides" this frame, the loop below won't run.
        
        for (int index = 0; index < this.GetSlideCollisionCount(); index++)
        {
            var collision = GetSlideCollision(index);
            if (collision.GetCollider() is Mob)
            {
                var mob = collision.GetCollider() as Mob;
                if (Vector3.Up.Dot(collision.GetNormal()) > 0.1f)
                {
                    mob.squash();
                    velocity.Y = bounce_impulse;
                }
            }
        }

        // This makes the character follow a nice arc when jumping
        GetNode<Node3D>("Pivot").Rotation = new Vector3(Mathf.Pi / 6 * velocity.Y / jump_impulse, 0, 0);
    }

    public void die()
    {
        EmitSignal(nameof(hit));
        QueueFree();
    }

    public void _on_MobDetector_body_entered(Node body)
    {
        die();
    }
}
