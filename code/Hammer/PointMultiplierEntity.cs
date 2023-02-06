using Editor;
using Sandbox;

namespace OpenTycoon;
[HammerEntity(), Title( "Tycoon Multiplier" ), Library( "tycoon_multiplier" )]
public class PointMultiplierEntity : BaseTycoonEntity
{

	public bool Enabled = false;
	[Property( Title = "Multiplier" )]
	public float Multiplier { get; set; } = 2;

	[Property( Title = "Parent Tycoon" )]
	[FGDType( "target_destination" )]
	public EntityTarget ParentTycoon { get; set; }
	public override void Spawn()
	{
		base.Spawn();
		SetupPhysicsFromModel( PhysicsMotionType.Static );
		Tags.Add( "trigger" );
		EnableDrawing = false;
		EnableAllCollisions = false;
	}
	public override void TycoonSpawned()
	{
		base.TycoonSpawned();
		EnableAllCollisions = false;
		EnableDrawing = true;
		EnableTouch = true;
		Enabled = true;
	}
	public override void Touch( Entity other )
	{
		base.Touch( other );
		if ( !Game.IsServer ) return;
		if ( !Enabled ) return;
		if ( other is PointEntity pnt )
		{
			pnt.Value *= Multiplier;
		}
	}

}
