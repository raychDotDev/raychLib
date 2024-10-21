namespace raychLib;

public abstract class TerminalScreen
{
	public OnLoadEvent? OnLoad;
	public OnUnloadEvent? OnUnload;
	public OnUpdateEvent? OnUpdate;
	public OnDrawEvent? OnDraw;

	public delegate void OnLoadEvent(object sender);
	public delegate void OnUnloadEvent(object sender);
	public delegate void OnUpdateEvent(object sender, TerminalUpdateEventArgs eventArgs);
	public delegate void OnDrawEvent(object sender, TerminalDrawEventArgs eventArgs);
}

public class TerminalUpdateEventArgs : EventArgs
{
	public readonly float FrameTime;
	public readonly TerminalInputController InputController;

	public TerminalUpdateEventArgs(float frameTime, TerminalInputController controller)
	{
		this.FrameTime = frameTime;
		this.InputController = controller;
	}
}

public class TerminalDrawEventArgs
{
	public readonly TerminalRenderer RenderContext;
	public TerminalDrawEventArgs(TerminalRenderer context)
	{
		this.RenderContext = context;
	}
}
