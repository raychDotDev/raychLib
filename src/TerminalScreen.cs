namespace raychLib;

public abstract class TerminalScreen
{
	public virtual void Load() {}
	public virtual void Unload() {}

	public virtual void Update(float dt, TerminalInputController inputController) {}
	public virtual void Draw(TerminalRenderer context) {}
}
