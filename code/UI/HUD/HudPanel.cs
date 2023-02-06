using Sandbox;

namespace OpenTycoon;
public class HudPanel : HudEntity<HudRootPanel>
{
	public static HudPanel Current;

	public HudPanel()
	{
		Current = this;

		if ( Game.IsClient )
		{

		}
	}
}
