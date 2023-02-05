using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;

namespace OpenTycoon;
class ButtonTextPanel : WorldPanel
{
	public Label Label;
	public ButtonTextPanel()
	{
		StyleSheet.Load( "/UI/ButtonText.scss" );
		Label = Add.Label( "null" );
	}
	[Event.Client.Frame]
	public void FrameUpdate()
	{
		Rotation = Camera.Rotation.Backward.EulerAngles.ToRotation();
	}
	protected override int BuildHash()
	{
		return HashCode.Combine( Label.Text );
	}
}
