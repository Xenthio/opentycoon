using Editor;
using Sandbox;

namespace OpenTycoon;
[HammerEntity(), Title( "Tycoon Creator" ), Library( "tycoon_creator" ), Model]
public class PointCreationEntity : BaseTycoonEntity
{
	public bool Enabled = false;
	public float TimeBetweenDrops = 2;
	TimeSince LastPointMade;

	[Property( Title = "Money Value" )]
	public float MoneyValue { get; set; } = 1;

	[Property( Title = "Parent Tycoon" )]
	[FGDType( "target_destination" )]
	public string ParentTycoon { get; set; }

	public override void Spawn()
	{
		base.Spawn();
		EnableDrawing = false;
		EnableAllCollisions = false;
	}

	public override void TycoonSpawned()
	{
		base.TycoonSpawned();
		Enabled = true;
	}
	[Event.Tick.Server]
	public void Tick()
	{
		if ( !Enabled ) return;
		if ( LastPointMade > TimeBetweenDrops )
		{
			LastPointMade = 0;
			var a = new PointEntity();
			a.Position = Position;
			a.Model = Model;
			a.SetupPhysicsFromModel( PhysicsMotionType.Dynamic );
			a.Scale = Scale;
			a.Tags.Add( "solid" );
			a.Tags.Add( "point" );
			a.PhysicsBody.Mass = 10f;
		}
	}
}
