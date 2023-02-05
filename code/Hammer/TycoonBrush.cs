using Editor;
using Sandbox;

namespace OpenTycoon;
[Library( "tycoon_brush" )]
[Solid, HammerEntity, RenderFields, VisGroup( VisGroup.Dynamic )]
[Title( "Tycoon Brush" ), Category( "Gameplay" ), Icon( "brush" )]
public class TycoonBrush : BaseTycoonEntity
{
	public override void Spawn()
	{
		base.Spawn();
		SetupPhysicsFromModel( PhysicsMotionType.Static );
		EnableDrawing = false;
		EnableAllCollisions = false;
	}
	public override void TycoonSpawned()
	{
		base.TycoonSpawned();
		EnableAllCollisions = true;
		EnableDrawing = true;
	}
}
