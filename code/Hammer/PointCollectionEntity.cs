using Editor;
using Sandbox;

namespace OpenTycoon;
[HammerEntity(), Title( "Tycoon Collector" ), Library( "tycoon_collector" )]
public class PointCollectionEntity : BrushEntity
{


	[Property( Title = "Parent Tycoon" )]
	[FGDType( "target_destination" )]
	public EntityTarget ParentTycoon { get; set; }

	public override void Touch( Entity other )
	{
		base.Touch( other );
		if ( !Game.IsServer ) return;
		if ( !Enabled ) return;
		if ( other is PointEntity pnt )
		{
			ParentTycoon.GetTarget<TycoonManagerEntity>().TycoonOwner.Money += pnt.Value;
			pnt.Delete();
		}
	}

}
