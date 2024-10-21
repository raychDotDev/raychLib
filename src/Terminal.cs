global using Raylib_CsLo;

namespace raychLib;

public class Terminal
{
	private static Terminal instance;

	public static Terminal Shared
	{
		get
		{
			if (Terminal.instance == null) Terminal.instance = new Terminal();
			return instance;
		}
	}

	protected TerminalRenderer Renderer { get; set; }

	internal TerminalRenderer getRenderer() => this.Renderer;

	public void Init(TerminalRenderer renderer)
	{
		this.Renderer = renderer;
		Terminal.instance = this;
	}

	public void SetScreen(TerminalScreen screen) => this.Renderer.SetScreen(screen);

	public void SetBufferSize(int width, int height) => this.Renderer.SetBufferSize(width, height);

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
