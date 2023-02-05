﻿using Sandbox;
namespace OpenTycoon;

public partial class Player : AnimatedEntity
{
	[Net] public int Money { get; set; }
	[Net] public TycoonManagerEntity OwnedTycoon { get; set; }
	/// <summary>
	/// Called when the entity is first created 
	/// </summary>
	public override void Spawn()
	{
		base.Spawn();

		//
		// Use a watermelon model
		//
		Tags.Add( "player" );
		SetModel( "models/sbox_props/watermelon/watermelon.vmdl" );
		SetupPhysicsFromModel( PhysicsMotionType.Keyframed );
		Components.Add( new WalkController() );
		Components.Add( new FirstPersonCamera() );
		EnableDrawing = true;
		EnableHideInFirstPerson = true;
		EnableShadowInFirstPerson = true;
	}

	// An example BuildInput method within a player's Pawn class.
	[ClientInput] public Vector3 InputDirection { get; set; }
	[ClientInput] public Angles ViewAngles { get; set; }

	public override void BuildInput()
	{
		base.BuildInput();
		foreach ( var i in Components.GetAll<SimulatedComponent>() )
		{
			i.BuildInput();
		}
	}

	/// <summary>
	/// Called every tick, clientside and serverside.
	/// </summary>
	public override void Simulate( IClient cl )
	{
		base.Simulate( cl );

		foreach ( var i in Components.GetAll<SimulatedComponent>() )
		{
			i.Simulate( cl );
		}
	}

	/// <summary>
	/// Called every frame on the client
	/// </summary>
	public override void FrameSimulate( IClient cl )
	{
		base.FrameSimulate( cl );
		foreach ( var i in Components.GetAll<SimulatedComponent>() )
		{
			i.FrameSimulate( cl );
		}
	}
}
