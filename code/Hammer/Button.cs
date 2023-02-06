using Editor;
using Sandbox;

namespace OpenTycoon;
[HammerEntity(), Title( "Tycoon Button" ), Library( "tycoon_button" ), EditorModel( "models/button.vmdl" )]
public partial class Button : BaseTycoonEntity
{
	[Property( Title = "Button Text" )]
	[Net] public string ButtonText { get; set; } = "null";
	[Property( Title = "Price" )]
	[Net] public int Price { get; set; } = 0;

	[Property( Title = "Parent Tycoon" )]
	[FGDType( "target_destination" )]
	public EntityTarget ParentTycoon { get; set; }
	ButtonLabelEntity Label { get; set; }
	public override void Spawn()
	{
		base.Spawn();
		SetModel( "models/button.vmdl" );
		SetupPhysicsFromModel( PhysicsMotionType.Keyframed );

		EnableTouch = false;
		Tags.Add( "trigger" );
		EnableAllCollisions = false;
		EnableDrawing = false;
	}
	public bool Enabled = false;
	public override void TycoonSpawned()
	{
		Enabled = true;
		EnableDrawing = true;
		EnableAllCollisions = true;
		Label = new( $"{ButtonText} - ${Price}" );
		Label.Transform = Transform;
		base.TycoonSpawned();
	}
	public override void TycoonDeleted()
	{
		base.TycoonDeleted();
		Label?.Delete();
	}
	[Event.Tick.Server]
	public void Tick()
	{

		if ( ParentTycoon.GetTarget<TycoonManagerEntity>() is TycoonManagerEntity mng )
		{
			if ( mng.TycoonOwner is Player ply )
			{
				if ( ply.Money < Price )
				{
					SetMaterialOverride( "materials/dev/primary_red.vmat" );
				}
				else
				{
					SetMaterialOverride( "materials/dev/primary_green.vmat" );
				}
			}
		}
	}
	public override void Touch( Entity other )
	{
		base.Touch( other );
		if ( !Enabled ) return;
		if ( other is Player ply )
		{
			if ( ply.Money >= Price )
			{
				Log.Info( "touched by ply" );
				OnPurchased.Fire( this );
				ply.Money -= Price;
			}
		}
	}
	protected Output OnPurchased { get; set; }
}
