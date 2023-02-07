using Editor;
using Sandbox;

namespace OpenTycoon;
[Library( "tycoon_brush" )]
[Solid, HammerEntity, RenderFields, VisGroup( VisGroup.Dynamic )]
[Title( "Tycoon Brush" ), Category( "Gameplay" ), Icon( "brush" )]
public class TycoonBrush : BaseTycoonEntity
{
	[Property]
	public bool Enabled { get; protected set; } = false;
	public override void Spawn()
	{
		base.Spawn();

		SetupPhysicsFromModel( PhysicsMotionType.Static );

		EnableDrawing = Enabled;
		EnableAllCollisions = Enabled;
	}
	public override void TycoonSpawned()
	{
		base.TycoonSpawned();
		EnableAllCollisions = true;
		EnableDrawing = true;
	}
}
