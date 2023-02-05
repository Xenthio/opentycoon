using Editor;
using Sandbox;

namespace OpenTycoon;

/// <summary>
/// A volume that pushes entities that are touching it.
/// </summary>
[Library( "tycoon_push_volume" )]
[HammerEntity, Solid, DrawAngles( "forcedirection" )]
[Title( "Push Volume" ), Category( "Triggers" ), Icon( "deblur" )]
public partial class TycoonPushVolumeEntity : BaseTrigger
{
	/// <summary>
	/// How strong should we be pushing other entities
	/// </summary>
	[Property]
	public float Force { get; set; } = 500;

	/// <summary>
	/// Direction of the force.
	/// </summary>
	[Property]
	public Angles ForceDirection { get; set; }

	/// <summary>
	/// If set, applies 1 second worth of force only when an entity enters the trigger
	/// </summary>
	[Property]
	public bool OnlyPushOnEnter { get; set; } = false;

	void PushObject( Entity entity, float time )
	{
		var force = (Rotation.From( ForceDirection ) * Rotation).Forward * Force * time;
		var isPhysics = false;
		if ( entity.PhysicsGroup != null && entity.PhysicsGroup.BodyCount > 0 )
		{
			foreach ( var body in entity.PhysicsGroup.Bodies )
			{
				if ( body.BodyType == PhysicsBodyType.Dynamic )
				{
					isPhysics = true;
					break;
				}
			}
		}

		if ( isPhysics )
		{
			foreach ( var body in entity.PhysicsGroup.Bodies )
			{
				body.ApplyImpulse( force * body.Mass );
			}
		}
		else
		{
			// Players...

			if ( force.z > 1 && entity.GroundEntity != null )
			{
				entity.GroundEntity = null;
			}

			entity.Velocity += force;
		}
	}

	[Event.Tick.Server]
	void PushObjectsAway()
	{
		if ( !Enabled ) return;

		if ( !OnlyPushOnEnter )
		{
			foreach ( var entity in TouchingEntities )
			{
				if ( !entity.IsValid() ) continue;
				PushObject( entity, Time.Delta );
			}
		}

		// Debug..
		if ( DebugFlags.HasFlag( EntityDebugFlags.Text ) )
		{
			DebugOverlay.Line( Position, Position + Rotation.From( ForceDirection ).Forward * 100 );
		}
	}

	public override void OnTouchStart( Entity toucher )
	{
		base.OnTouchStart( toucher );

		if ( OnlyPushOnEnter )
		{
			PushObject( toucher, 1 );
		}
	}

	/// <summary>
	/// Sets the force per second for this push volume
	/// </summary>
	[Input]
	protected void SetForce( float force )
	{
		Force = force;
	}
}
