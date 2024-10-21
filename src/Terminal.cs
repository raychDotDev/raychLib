global using Raylib_CSharp;
global using Raylib_CSharp.Windowing;

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
	
	public int GetBufferWidth() => this.Renderer.Buffer.GetLength(1);
	public int GetBufferHeight() => this.Renderer.Buffer.GetLength(0);

	public void Run()
	{
		while (!Window.ShouldClose())
		{
			this.Renderer.Render();
			this.Renderer.Update(Time.GetFrameTime());
		}
	}

	public void Stop()
	{
		this.Renderer.Unload();
		Window.Close();
	}

	public int GetFPS() => Time.GetFPS();

	public float GetTime() => (float)Time.GetTime();
}
