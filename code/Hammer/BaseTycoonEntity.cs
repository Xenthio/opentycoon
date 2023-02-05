using Sandbox;

namespace OpenTycoon;
public class BaseTycoonEntity : ModelEntity
{
	[Input]
	public virtual void TycoonSpawned()
	{

	}
	[Input]
	public virtual void TycoonDeleted()
	{
		Delete();
	}
}
