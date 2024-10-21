using System.Reflection;
using System.Numerics;

namespace raychLib;

public class TerminalRenderer
{
	private TerminalScreen? Screen;

	public TerminalGlyph[,] Buffer { get; private set; }

	private RenderTexture RenderZone;

	protected TerminalInputController InputController { get; set; }

	private Font font;

	public readonly string Charset;

	internal Vector2 glyphOffset;

	public unsafe TerminalRenderer(int windowWidth, int windowHeight, int bufferWidth = 50, int bufferHeight = 30, string title = "untitled")
	{
		// Raylib.SetConfigFlags(ConfigFlags.FLAG_VSYNC_HINT);
		Raylib.SetConfigFlags(ConfigFlags.FLAG_WINDOW_RESIZABLE);
		Raylib.InitWindow(windowWidth, windowHeight, title);
		Raylib.SetTargetFPS(60);
		Raylib.SetWindowMinSize(400, 300);

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
		int codepointsCount = 0;
		int* codepoints = Raylib.LoadCodepoints(charset, &codepointsCount);
		int fontSize = 40;
		fixed (byte* fontDataPtr = fontData)
		{
			this.font = Raylib.LoadFontFromMemory(".ttf", fontDataPtr, fontData.Length, fontSize, codepoints, codepointsCount);
		}
		this.Buffer = new TerminalGlyph[bufferHeight, bufferWidth];

		var glyphInfo = Raylib.GetGlyphInfo(this.font, Raylib.GetGlyphIndex(this.font, (int)'█'));

		int xoff = glyphInfo.advanceX - this.font.baseSize;
		xoff = xoff > this.font.baseSize ? 0 : xoff;
		this.glyphOffset = new(this.font.baseSize + xoff, this.font.baseSize);

		this.RenderZone = Raylib.LoadRenderTexture(bufferWidth * (int)this.glyphOffset.X, bufferHeight * (int)this.glyphOffset.Y);

		Raylib.SetTextureFilter(this.RenderZone.texture, TextureFilter.TEXTURE_FILTER_POINT);
		this.InputController = new TerminalInputController();
	}

	public void SetScreen(TerminalScreen screen)
	{
		this.Screen?.Unload();
		this.Screen = screen;
		this.Screen?.Load();
	}

	public void Render()
	{
		Raylib.EnableEventWaiting();
		ref Texture texture = ref this.RenderZone.texture;

		Raylib.BeginTextureMode(this.RenderZone);
		Raylib.ClearBackground(Raylib.BLACK);
		{
			this.ClearBuffer();
			this.Screen?.Draw(this);
			this.DrawBuffer();
		}
		Raylib.EndTextureMode();

		Raylib.BeginDrawing();
		Raylib.ClearBackground(Raylib.BLACK);
		{
			Raylib.DrawTexturePro(
					texture,
					new Rectangle(0, 0, texture.width, -texture.height),
					new Rectangle(0, 0, Raylib.GetScreenWidth(), Raylib.GetScreenHeight()),
					new Vector2(0, 0),
					0.0f,
					Raylib.WHITE);
		}
		Raylib.EndDrawing();
		Raylib.PollInputEvents();
	}

	private void RenderGlyph(ref TerminalGlyph glyph, int x, int y)
	{
		Raylib.DrawTextEx(
				this.font,
				$"{glyph.Character}",
				new Vector2(x * this.glyphOffset.X,
					y * this.glyphOffset.Y),
				this.font.baseSize,
				0.0f,
				(Color)glyph.ForegroundColor);
	}

	public void Update(float deltaTime)
	{
		this.Screen?.Update(Raylib.GetFrameTime(), this.InputController);
	}

	private void ClearBuffer()
	{
		for (int i = 0; i < this.Buffer.GetLength(0); i++)
		{
			for (int j = 0; j < this.Buffer.GetLength(1); j++)
			{
				ref var g = ref this.Buffer[i, j]; g.Clear();
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

	public void Unload()
	{
		this.Screen?.Unload();
	}
}
