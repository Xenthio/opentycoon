using Editor;
using Sandbox;

namespace OpenTycoon;
[Library( "tycoon_brush" )]
[Solid, HammerEntity, RenderFields, VisGroup( VisGroup.Dynamic )]
[Title( "Tycoon Brush" ), Category( "Gameplay" ), Icon( "brush" )]
public class TycoonBrush : BaseTycoonEntity
{
	[Property]
	public bool StartSpawned { get; protected set; } = false;
	public override void Spawn()
	{
		base.Spawn();

		SetupPhysicsFromModel( PhysicsMotionType.Static );

		EnableDrawing = StartSpawned;
		EnableAllCollisions = StartSpawned;
	}
	public override void TycoonSpawned()
	{
		base.TycoonSpawned();
		EnableAllCollisions = true;
		EnableDrawing = true;
	}
}
