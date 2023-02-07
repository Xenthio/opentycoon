using Sandbox;
using static Sandbox.Event;

namespace OpenTycoon;

public class CitizenAnimationComponent : SimulatedComponent
{
	public override void Simulate( IClient cl )
	{
		base.Simulate( cl );
		var pl = Entity as Player;
		// where should we be rotated to
		var turnSpeed = 0.02f;

		Rotation rotation = pl.ViewAngles.ToRotation();

		var MovementMode = pl.Components.Get<MovementComponent>();

		var idealRotation = Rotation.LookAt( rotation.Forward.WithZ( 0 ), Vector3.Up );
		pl.Rotation = Rotation.Slerp( pl.Rotation, idealRotation, MovementMode.WishVelocity.Length * Time.Delta * turnSpeed );
		pl.Rotation = pl.Rotation.Clamp( idealRotation, 45.0f, out var shuffle ); // lock facing to within 45 degrees of look direction

		CitizenAnimationHelper animHelper = new CitizenAnimationHelper( pl );

		animHelper.WithWishVelocity( MovementMode.WishVelocity );
		animHelper.WithVelocity( pl.Velocity );
		animHelper.WithLookAt( pl.AimRay.Position + pl.AimRay.Forward * 100.0f, 1.0f, 1.0f, 0.5f );
		animHelper.AimAngle = rotation;
		animHelper.FootShuffle = shuffle;
		animHelper.DuckLevel = MathX.Lerp( animHelper.DuckLevel, Entity.Tags.Has( "ducked" ) ? 1 : 0, Time.Delta * 10.0f );
		animHelper.VoiceLevel = (Game.IsClient && pl.Client.IsValid()) ? pl.Client.Voice.LastHeard < 0.5f ? pl.Client.Voice.CurrentLevel : 0.0f : 0.0f;
		animHelper.IsGrounded = Entity.GroundEntity != null;
		animHelper.IsSitting = Entity.Tags.Has( "sitting" );
		animHelper.IsNoclipping = Entity.Tags.Has( "noclip" );
		animHelper.IsClimbing = Entity.Tags.Has( "climbing" );
		animHelper.IsSwimming = Entity.GetWaterLevel() >= 0.5f;
		animHelper.IsWeaponLowered = false;

		if ( Entity.Tags.Has( "jump" ) ) animHelper.TriggerJump();
		/*
		if ( ActiveChild != lastWeapon ) animHelper.TriggerDeploy();

		if ( ActiveChild is BaseCarriable carry )
		{
			carry.SimulateAnimator( animHelper );
		}
		else
		{*/
		animHelper.HoldType = CitizenAnimationHelper.HoldTypes.None;
		animHelper.AimBodyWeight = 0.5f;
		/*
		}

		lastWeapon = ActiveChild;
		*/
	}
}
