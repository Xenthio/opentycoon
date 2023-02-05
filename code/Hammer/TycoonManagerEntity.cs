using Editor;
using Sandbox;

namespace OpenTycoon;
[HammerEntity(), Title( "Tycoon Plot" ), Library( "tycoon_plot" ), EditorModel( "models/button.vmdl" )]
public partial class TycoonManagerEntity : ModelEntity
{

	[Net] public Player TycoonOwner { get; set; }


	ButtonLabelEntity Label { get; set; }
	public override void Spawn()
	{
		base.Spawn();
		SetModel( "models/button.vmdl" );
		SetupPhysicsFromModel( PhysicsMotionType.Keyframed );
		Tags.Add( "trigger" );
		EnableTouch = true;

		Label = new( "Claim this tycoon" );
		Label.Transform = Transform;
	}
	public override void Touch( Entity other )
	{
		if ( other is Player ply && Game.IsServer )
		{
			Log.Info( "touched by ply" );
			if ( ply.OwnedTycoon == null )
			{
				TycoonOwner = ply;
				ply.OwnedTycoon = this;
				Label.Delete();
				EnableTouch = false;
				EnableAllCollisions = false;
				EnableDrawing = false;
				OnPurchased.Fire( this );
			}
		}
		base.Touch( other );
	}
	protected Output OnPurchased { get; set; }
}
