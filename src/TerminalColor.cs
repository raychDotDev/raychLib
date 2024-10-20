namespace raychLib;

public struct TerminalColor
{
	public byte R;
	public byte G;
	public byte B;

	public TerminalColor()
	{
		this.R = 0; this.G = 0; this.B = 0;
	}

	public TerminalColor(byte grayscale)
	{
		this.R = grayscale; this.G = grayscale; this.B = grayscale;
	}

	public TerminalColor(byte r, byte g, byte b)
	{
		this.R = r; this.G = g; this.B = b;
	}

	public static explicit operator Raylib_cs.Color(TerminalColor c)
	{ return new Color(c.R, c.G, c.B, (byte)255); }

	public static explicit operator TerminalColor(Color c)
	{ return new TerminalColor(c.R, c.G, c.B); }
}
