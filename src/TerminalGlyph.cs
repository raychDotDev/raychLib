namespace raychLib;

public struct TerminalGlyph
{
	public char Character;
	public TerminalColor ForegroundColor;
	public TerminalColor BackgroundColor;
	
	public TerminalGlyph()
	{
		this.Clear();
	}
	
	public TerminalGlyph(char character, TerminalColor foregroundColor, TerminalColor backgroundColor)
	{
		this.Character = character;
		this.ForegroundColor = foregroundColor;
		this.BackgroundColor = backgroundColor;
	}

	public void Clear()
	{
		this.Character = (char)0;
		this.ForegroundColor = new (255);
		this.BackgroundColor = new (0);
	}
}
