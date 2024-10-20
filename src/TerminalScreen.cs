namespace raychLib;

public abstract class TerminalScreen
{
	public virtual void Load() {}
	public virtual void Unload() {}

	public virtual void Update(float dt) {}
	public virtual void Draw(Terminal context) {}
}
