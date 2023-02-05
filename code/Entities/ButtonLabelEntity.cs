using Sandbox;

namespace OpenTycoon;
public partial class ButtonLabelEntity : Entity
{
	public ButtonLabelEntity( string text )
	{
		Text = text;
	}
	public ButtonLabelEntity()
	{

	}
	[Net] public string Text { get; set; }
	ButtonTextPanel NamePanel;
	public override void Spawn()
	{
		Transmit = TransmitType.Always;
		base.Spawn();
	}
	public override void ClientSpawn()
	{
		base.ClientSpawn();
		NamePanel = new();
		NamePanel.Position = Position + (Vector3.Up * 16);
		NamePanel.Label.Text = Text;

		if ( !EnableDrawing )
			NamePanel.Style.Opacity = 0;
	}
	[Event.Tick.Client]
	public void Tick()
	{
		NamePanel.Label.Text = Text;
	}
	protected override void OnDestroy()
	{
		base.OnDestroy();
		if ( Game.IsClient )
		{
			NamePanel.Delete( true );
		}
	}
}
