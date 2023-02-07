using Editor;
using Sandbox;
using System.Collections.Generic;
using System.Linq;

namespace OpenTycoon
{
	[AutoApplyMaterial( "materials/tools/toolstrigger.vmat" )]
	[Solid, VisGroup( VisGroup.Trigger ), HideProperty( "enable_shadows" )]
	[Title( "Base Trigger" ), Icon( "select_all" )]
	public partial class BaseTycoonTrigger : BaseTycoonEntity
	{
		[ConCmd.Admin( "drawtriggers_toggle" )]
		internal static void ToggleDrawTriggers()
		{
			foreach ( var ent in All )
			{
				if ( ent is ModelEntity modelEnt )
				{
					if ( ent is BaseTrigger || ent.Tags.Has( "trigger" ) )
					{
						modelEnt.DebugFlags ^= EntityDebugFlags.TriggerBounds;
					}
				}
			}
		}

		/// <summary>
		/// Entities with these tags can activate this trigger.
		/// </summary>
		[Property(), Title( "Activation Tags" ), DefaultValue( "player" )]
		public TagList ActivationTags { get; set; } = "player";

		/// <summary>
		/// Whether this entity is enabled or not.
		/// </summary>
		[Property]
		public bool Enabled { get; protected set; } = false;

		/// <summary>
		/// List of entities currently within this trigger's bounds.
		/// </summary>
		public IEnumerable<Entity> TouchingEntities => touchingEntities;

		/// <summary>
		/// A convenience property containing number of entities currently within this trigger's bounds.
		/// </summary>
		public int TouchingEntityCount => touchingEntities.Count;

		readonly List<Entity> touchingEntities = new();

		// Used for when an entity enters the trigger while it is disabled, and then the trigger gets enabled
		readonly List<Entity> touchingEntitiesWhileDisabled = new();
		public override void TycoonSpawned()
		{
			base.TycoonSpawned();
			Enabled = true;
		}
		public override void Spawn()
		{
			base.Spawn();

			Tags.Add( "trigger" );

			SetupPhysicsFromModel( PhysicsMotionType.Static );
			EnableAllCollisions = false;
			EnableTouch = true;

			Transmit = TransmitType.Never;
		}

		public override void StartTouch( Entity other )
		{
			base.StartTouch( other );

			if ( other.IsWorld )
				return;

			AddToucher( other );
		}

		// This is to make sure we can add a toucher after they have entered the trigger but we were on a cooldown or something (trigger_multiple's wait param)
		public override void Touch( Entity other )
		{
			base.Touch( other );

			if ( other.IsWorld )
				return;

			AddToucher( other );
		}

		public override void EndTouch( Entity other )
		{
			base.EndTouch( other );

			if ( other.IsWorld )
				return;

			if ( touchingEntitiesWhileDisabled.Contains( other ) )
			{
				touchingEntitiesWhileDisabled.Remove( other );
			}

			if ( touchingEntities.Contains( other ) )
			{
				touchingEntities.Remove( other );
				OnTouchEnd( other );

				if ( touchingEntities.Count < 1 ) OnTouchEndAll( other );
			}
		}

		void AddToucher( Entity toucher )
		{
			if ( !toucher.IsValid() )
				return;

			if ( !Enabled )
			{
				// We don't care about the filter because we will pass these entities to StartTouch
				if ( !touchingEntitiesWhileDisabled.Contains( toucher ) )
				{
					touchingEntitiesWhileDisabled.Add( toucher );
				}

				return;
			}

			if ( touchingEntities.Contains( toucher ) )
				return;

			if ( !PassesTriggerFilters( toucher ) )
				return;

			bool anyoneTouching = touchingEntities.Count > 0;

			touchingEntities.Add( toucher );
			OnTouchStart( toucher );

			if ( !anyoneTouching ) OnTouchStartAll( toucher );
		}

		/// <summary>
		/// Fired when an entity starts touching this trigger. The touching entity must pass this trigger's filters to cause this output to fire.
		/// </summary>
		protected Output OnStartTouch { get; set; }

		/// <summary>
		/// Fired when an entity stops touching this trigger. Only entities that passed this trigger's filters will cause this output to fire.
		/// </summary>
		protected Output OnEndTouch { get; set; }

		/// <summary>
		/// Fired when an entity starts touching this trigger while no other passing entities are touching it.
		/// </summary>
		protected Output OnStartTouchAll { get; set; }

		/// <summary>
		/// Fired when all entities touching this trigger have stopped touching it.
		/// </summary>
		protected Output OnEndTouchAll { get; set; }

		/// <summary>
		/// Enables this trigger
		/// </summary>
		[Input]
		public void Enable()
		{
			Enabled = true;

			// Everything that is already inside the volume now just started touching us.
			foreach ( var entity in touchingEntitiesWhileDisabled.ToArray() )
			{
				if ( !entity.IsValid() ) continue;

				StartTouch( entity );
			}
			touchingEntitiesWhileDisabled.Clear();
		}

		/// <summary>
		/// Disables this trigger
		/// </summary>
		[Input]
		public void Disable()
		{
			Enabled = false;

			// We are disabled, pretend all our touching entities stopped touching us.
			foreach ( var entity in TouchingEntities.ToList() )
			{
				if ( !entity.IsValid() ) continue;

				EndTouch( entity );

				if ( !touchingEntitiesWhileDisabled.Contains( entity ) )
				{
					touchingEntitiesWhileDisabled.Add( entity );
				}
			}
		}

		/// <summary>
		/// Toggles this trigger between enabled and disabled states
		/// </summary>
		[Input]
		public void Toggle()
		{
			if ( Enabled )
			{
				Disable();
			}
			else
			{
				Enable();
			}
		}

		/// <summary>
		///	An entity that passes PassesTriggerFilters has started touching the trigger
		/// </summary>
		public virtual void OnTouchStart( Entity toucher )
		{
			if ( !Enabled ) return;

			OnStartTouch.Fire( toucher );
		}

		/// <summary>
		///	An entity that started touching this trigger has stopped touching
		/// </summary>
		public virtual void OnTouchEnd( Entity toucher )
		{
			if ( !Enabled ) return;

			OnEndTouch.Fire( toucher );
		}

		/// <summary>
		///	Called when an entity starts touching this trigger while no other passing entities are touching it
		/// </summary>
		public virtual void OnTouchStartAll( Entity toucher )
		{
			if ( !Enabled ) return;

			OnStartTouchAll.Fire( toucher );
		}

		/// <summary>
		///	Called when all entities touching this trigger have stopped touching it.
		/// </summary>
		public virtual void OnTouchEndAll( Entity toucher )
		{
			if ( !Enabled ) return;

			OnEndTouchAll.Fire( toucher );
		}

		/// <summary>
		///	Determine if an entity should be allowed to touch this trigger
		/// </summary>
		public virtual bool PassesTriggerFilters( Entity other )
		{
			if ( other is not ModelEntity )
			{
				return false;
			}

			if ( other.Tags.HasAny( ActivationTags ) || ActivationTags.Contains( "*" ) )
			{
				return true;
			}

			return false;
		}
	}
}
