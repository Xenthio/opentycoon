using Sandbox.UI;

namespace OpenTycoon;
public partial class HudRootPanel : RootPanel
{
	public static HudRootPanel Current;
	public HudRootPanel()
	{
		Current = this;
		AddChild<Money>();
		Style.PointerEvents = PointerEvents.None;
	}
}
