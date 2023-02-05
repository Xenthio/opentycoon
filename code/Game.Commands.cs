using Sandbox;
using System.Linq;

namespace OpenTycoon;
public partial class MyGame
{


	[ConCmd.Server( "ent_create" )]
	public static void SpawnEntity( string entName )
	{
		Log.Info( "creating " + entName );
		var owner = ConsoleSystem.Caller.Pawn as Player;

		if ( owner == null )
		{
			Log.Info( "Failed to create " + entName );
			return;
		}

		var entityType = TypeLibrary.GetType<Entity>( entName )?.TargetType;
		if ( entityType == null )
		{
			Log.Info( "Failed to create " + entName );
			return;
		}

		var tr = Trace.Ray( owner.AimRay, 500 )
			.UseHitboxes()
			.Ignore( owner )
			.Size( 2 )
			.Run();

		var ent = TypeLibrary.Create<Entity>( entityType );

		ent.Position = tr.EndPosition;
		ent.Rotation = Rotation.From( new Angles( 0, owner.AimRay.Forward.EulerAngles.yaw, 0 ) );

		//Log.Info( $"ent: {ent}" );
	}
	[ConCmd.Server( "reset_game" )]
	public static void ResetGame()
	{
		// Tell our game that all clients have just left.
		foreach ( IClient cl in Game.Clients )
		{
			cl.Components.RemoveAll();
			(MyGame.Current as MyGame).ClientDisconnect( cl, NetworkDisconnectionReason.DISCONNECT_BY_USER );
		}

		/*if ( !ConsoleSystem.Caller.HasPermission( "admin" ) )
		{
			Log.Info( "No permission: reset_game" );
			return;
		}*/
		CleanupClientEntities( To.Everyone );
		// Delete everything except the clients and the world
		var ents = Entity.All.ToList();
		ents.RemoveAll( e => e is IClient );
		ents.RemoveAll( e => e is WorldEntity );
		foreach ( Entity ent in ents )
		{
			ent.Delete();
		}

		// Reset the map
		//Map.Reset( DefaultCleanupFilter );
		Game.ResetMap( Entity.All.Where( x => x is Player ).ToArray() );


		// Create a brand new game
		MyGame.Current = new MyGame();

		// Tell our new game that all clients have just joined to set them all back up.
		foreach ( IClient cl in Game.Clients )
		{
			cl.Components.RemoveAll();
			(MyGame.Current as MyGame).ClientJoined( cl );
		}
	}
	[ClientRpc]
	public static void CleanupClientEntities()
	{
		var ents = Entity.All.ToList();
		foreach ( Entity ent in ents )
		{
			if ( ent.IsClientOnly )
			{
				ent.Delete();
			}
		}
		var objs = Game.SceneWorld.SceneObjects;


	}
}
