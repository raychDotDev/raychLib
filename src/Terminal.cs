global using Raylib_cs;
using System.Reflection;

namespace raychLib;

public class Terminal
{
	public TerminalGlyph[,] Buffer { get; private set; }

	private RenderTexture2D RenderZone;

	private Font font;

	private TerminalScreen? Screen;
	public readonly string Charset;

	public unsafe Terminal(int windowWidth, int windowHeight, int bufferWidth = 50, int bufferHeight = 30, string title = "untitled")
	{
		Raylib.SetConfigFlags(ConfigFlags.ResizableWindow);
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
		int[] codepoints = Raylib.LoadCodepoints(charset, ref codepointsCount);
		int fontSize = 40;
		this.font = Raylib.LoadFontFromMemory(".ttf", fontData, fontSize, codepoints, codepointsCount);
		this.Buffer = new TerminalGlyph[bufferHeight, bufferWidth];

		var glyphInfo = Raylib.GetGlyphInfo(this.font, Raylib.GetGlyphIndex(this.font, (int)'█'));
		int xoff = glyphInfo.AdvanceX - this.font.BaseSize;
		xoff = xoff > this.font.BaseSize ? 0 : xoff;
		this.RenderZone = Raylib.LoadRenderTexture(bufferWidth * (this.font.BaseSize + (xoff)), bufferHeight * this.font.BaseSize);

		Raylib.SetExitKey(KeyboardKey.Null);
	}

	public void SetScreen(TerminalScreen screen)
	{
		this.Screen?.Unload();
		this.Screen = screen;
		this.Screen?.Load();
	}

	private void ClearBuffer()
	{
		foreach (var g in this.Buffer) g.Clear();
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

	private void RenderGlyph(ref TerminalGlyph glyph, int x, int y)
	{
		var glyphInfo = Raylib.GetGlyphInfo(this.font, Raylib.GetGlyphIndex(this.font, (int)glyph.Character));

		int xoff = glyphInfo.AdvanceX - this.font.BaseSize;
		xoff = xoff > this.font.BaseSize ? 0 : xoff;
		Raylib.DrawTextEx(
				this.font,
				$"{glyph.Character}",
				new(x * (this.font.BaseSize + xoff ),
					y * this.font.BaseSize),
				this.font.BaseSize,
				0.0f,
				(Color)glyph.ForegroundColor);
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

	public void Run()
	{
		while (!Raylib.WindowShouldClose())
		{
			ref Texture2D texture = ref this.RenderZone.Texture;
			Raylib.BeginTextureMode(this.RenderZone);
			Raylib.SetTextureFilter(texture, TextureFilter.Point);
			Raylib.ClearBackground(Color.Black);
			{
				this.ClearBuffer();
				this.Screen?.Draw(this);
				this.DrawBuffer();
			}
			Raylib.EndTextureMode();

			Raylib.BeginDrawing();
			Raylib.ClearBackground(Color.Black);
			{
				Raylib.DrawTexturePro(
						texture,
						new(0, 0, texture.Width, -texture.Height),
						new(0, 0, Raylib.GetScreenWidth(), Raylib.GetScreenHeight()),
						new(0, 0),
						0.0f,
						Color.White);
			}
			Raylib.EndDrawing();
			this.Screen?.Update(Raylib.GetFrameTime());
		}
	}

	public void Stop()
	{
		//TODO: add something important 
		Raylib.CloseWindow();
	}
}
