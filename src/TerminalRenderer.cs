using System.Reflection;
using System.Numerics;
using Raylib_CSharp;
using Raylib_CSharp.Windowing;
using Raylib_CSharp.Geometry;
using Raylib_CSharp.Transformations;
using Raylib_CSharp.Rendering;
using Raylib_CSharp.Colors;
using Raylib_CSharp.Textures;
using Raylib_CSharp.Fonts;


namespace raychLib;

public partial class TerminalRenderer
{
	public const int MIN_BUFFER_WIDTH = 40;
	public const int MIN_BUFFER_HEIGHT = 20;

	private TerminalScreen? Screen;

	public TerminalGlyph[,] Buffer { get; private set; }

	private RenderTexture2D RenderZone;

	protected TerminalInputController InputController { get; set; }

	private Font font;

	public readonly string Charset;

	internal Vector2 glyphOffset;

	private TerminalColor ClearColor = new(0);

	public unsafe TerminalRenderer(int windowWidth, int windowHeight, int bufferWidth = 50, int bufferHeight = 30, string title = "untitled")
	{
		Raylib.SetConfigFlags(ConfigFlags.VSyncHint);
		Raylib.SetConfigFlags(ConfigFlags.ResizableWindow);
		Window.Init(windowWidth, windowHeight, title);
		// Time.SetTargetFPS(60);
		Window.SetMinSize(400, 300);
		byte[] fontData;
		var assembly = Assembly.GetExecutingAssembly();
		using (var stream = assembly.GetManifestResourceStream("raychLib.res.fonts.default_font.ttf"))
		{
			using (var memSream = new MemoryStream())
			{
				if (stream == null) throw new Exception("Cannot load default font");
				stream.CopyTo(memSream);
				fontData = memSream.ToArray();
			}
		}

		string charset = "";
		using (var stream = assembly.GetManifestResourceStream("raychLib.res.fonts.charset.txt"))
		{
			if (stream == null) throw new Exception("Cannot load charset");
			using (var reader = new StreamReader(stream))
			{
				charset = reader.ReadToEnd();
				this.Charset = charset;
			}
		}

		ReadOnlySpan<int> codepoints = TextManager.LoadCodepoints(charset);

		int fontSize = 40;
		this.font = Font.LoadFromMemory(".ttf", new ReadOnlySpan<byte>(fontData), fontSize, codepoints);
		this.Buffer = new TerminalGlyph[bufferHeight, bufferWidth];

		var glyphInfo = this.font.GetGlyphInfo((int)'█');

		int xoff = glyphInfo.AdvanceX - this.font.BaseSize;
		xoff = xoff > this.font.BaseSize ? 0 : xoff;
		this.glyphOffset = new(this.font.BaseSize + xoff, this.font.BaseSize);

		this.RenderZone = RenderTexture2D.Load(bufferWidth * (int)this.glyphOffset.X, bufferHeight * (int)this.glyphOffset.Y);

		this.RenderZone.Texture.SetFilter(TextureFilter.Point);
		this.InputController = new TerminalInputController();

		Window.EnableEventWaiting();
	}

	public void SetScreen(TerminalScreen screen)
	{
		this.Screen?.OnUnload?.Invoke(this.Screen);
		this.Screen = screen;
		this.Screen?.OnLoad?.Invoke(this.Screen);
	}

	public void SetBufferSize(int width, int height)
	{
		this.Buffer = new TerminalGlyph[height, width];
		this.RenderZone.Unload();
		this.RenderZone = RenderTexture2D.Load(width * (int)this.glyphOffset.X, height * (int)this.glyphOffset.Y);
	}

	public void Render()
	{
		ref Texture2D texture = ref this.RenderZone.Texture;
		
		Graphics.BeginTextureMode(this.RenderZone);
		Graphics.ClearBackground(Color.Black);
		{
			this.ClearBuffer();
			this.Screen?.OnDraw?.Invoke(this.Screen, new TerminalDrawEventArgs(this));
			this.DrawBuffer();
		}
		Graphics.EndTextureMode();
	
		Graphics.BeginDrawing();
		Graphics.ClearBackground(Color.Black);
		{
			Graphics.DrawTexturePro(
					texture,
					new Rectangle(0, 0, texture.Width, -texture.Height),
					new Rectangle(0, 0, Window.GetScreenWidth(), Window.GetScreenHeight()),
					new Vector2(0, 0),
					0.0f,
					Color.White);
		}
		Graphics.EndDrawing();
	}

	private void RenderGlyph(ref TerminalGlyph glyph, int x, int y)
	{
		Graphics.DrawTextEx(
				this.font,
				$"{glyph.Character}",
				new Vector2(x * this.glyphOffset.X,
					y * this.glyphOffset.Y),
				this.font.BaseSize,
				0.0f,
				(Color)glyph.ForegroundColor);
	}

	public void Update(float deltaTime)
	{
		this.Screen?.OnUpdate?.Invoke(this.Screen, new TerminalUpdateEventArgs(Time.GetFrameTime(), this.InputController));
	}

	private void ClearBuffer()
	{
		for (int i = 0; i < this.Buffer.GetLength(0); i++)
		{
			for (int j = 0; j < this.Buffer.GetLength(1); j++)
			{
				ref var g = ref this.Buffer[i, j]; g.Clear();
				g.BackgroundColor = this.ClearColor;
			}
		}
	}

	private void DrawBuffer()
	{
		for (int i = 0; i < this.Buffer.GetLength(0); i++)
		{
			for (int j = 0; j < this.Buffer.GetLength(1); j++)
			{
				ref TerminalGlyph glyph = ref this.Buffer[i, j];

				var bgglyph = new TerminalGlyph('█', glyph.BackgroundColor, new(0));

				this.RenderGlyph(ref bgglyph, j, i);

				this.RenderGlyph(ref glyph, j, i);
			}
		}
	}

	public void DrawGlyph(int x, int y, TerminalGlyph glyph)
	{
		if (x >= this.Buffer.GetLength(1) || x < 0 || y >= this.Buffer.GetLength(0) || y < 0)
			return;

		ref TerminalGlyph bufferGlyph = ref this.Buffer[y, x];

		bufferGlyph.Character = glyph.Character;
		bufferGlyph.ForegroundColor = glyph.ForegroundColor;
		bufferGlyph.BackgroundColor = glyph.BackgroundColor;
	}


	public void DrawText(int x, int y, string text, TerminalColor foregroundColor, TerminalColor backgroundColor)
	{
		for (int i = 0; i < text.Length; i++) this.DrawGlyph(x + i, y, new TerminalGlyph(text[i], foregroundColor, backgroundColor));
	}

	public void DrawText(int x, int y, TerminalGlyph[] text)
	{
		for (int i = 0; i < text.Length; i++) this.DrawGlyph(x + i, y, text[i]);
	}

	public void HighlightGlyph(int x, int y, float foregroundValue, float backgroundValue)
	{
		if (x >= this.Buffer.GetLength(1) || x < 0 || y >= this.Buffer.GetLength(0) || y < 0)
			return;

		ref var g = ref this.Buffer[y, x];
		//TODO: make lerp an TerminalColor class method
		g.ForegroundColor.R = (byte)RayMath.Lerp(g.ForegroundColor.R, 255, foregroundValue);
		g.ForegroundColor.G = (byte)RayMath.Lerp(g.ForegroundColor.G, 255, foregroundValue);
		g.ForegroundColor.B = (byte)RayMath.Lerp(g.ForegroundColor.B, 255, foregroundValue);

		g.BackgroundColor.R = (byte)RayMath.Lerp(g.BackgroundColor.R, 255, backgroundValue);
		g.BackgroundColor.G = (byte)RayMath.Lerp(g.BackgroundColor.G, 255, backgroundValue);
		g.BackgroundColor.B = (byte)RayMath.Lerp(g.BackgroundColor.B, 255, backgroundValue);
	}

	public void SetClearColor(TerminalColor color)
	{
		this.ClearColor = color;
	}

	public void Unload()
	{
		this.Screen?.OnUnload?.Invoke(this.Screen);
	}
}
