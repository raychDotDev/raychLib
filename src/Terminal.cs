global using Raylib_CsLo;

namespace raychLib;

public class Terminal
{
	private static Terminal? instance;

	public static Terminal Shared
	{
		get
		{
			if (Terminal.instance == null) throw new Exception("There is no Terminal instance!");
			return instance;
		}
	}

	protected TerminalRenderer Renderer { get; set; }

	internal TerminalRenderer getRenderer() => this.Renderer;

	public unsafe Terminal(TerminalRenderer renderer)
	{
		this.Renderer = renderer;
		Terminal.instance = this;
	}

	public void SetScreen(TerminalScreen screen) => this.Renderer.SetScreen(screen);

	public void Run()
	{
		while (!Raylib.WindowShouldClose())
		{
			this.Renderer.Render();
			this.Renderer.Update(Raylib.GetFrameTime());
		}
	}

	public void Stop()
	{
		this.Renderer.Unload();
		Raylib.CloseWindow();
	}

	public int GetFPS() => Raylib.GetFPS();

	public float GetTime() => (float)Raylib.GetTime();
}
