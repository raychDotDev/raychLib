using System.Numerics;

namespace raychLib;

public partial class TerminalRenderer
{
	public void DrawLine(int startX, int startY, int endX, int endY, TerminalGlyph glyph)
	{
		int deltaX = endX - startX;
		int deltaY = endY - startY;

		int deltaMag = (int)(MathF.Sqrt(MathF.Pow(deltaX, 2) + MathF.Pow(deltaY, 2)));
		float dirX = (float)deltaX / (float)deltaMag;
		float dirY = (float)deltaY / (float)deltaMag;

		int length = (int)MathF.Sqrt(MathF.Pow(endX - startX, 2) + MathF.Pow(endY - startY, 2));

		for (int i = 0; i <= length; i++)
		{
			this.DrawGlyph((int)(startX + dirX * i), (int)(startY + dirY * i), glyph);
		}
	}

	public void DrawRectangleLines(int positionX, int positionY, int width, int height, TerminalGlyph glyph)
	{
		this.DrawLine(positionX, positionY, positionX, positionY + height, glyph);
		this.DrawLine(positionX, positionY + height, positionX + width, positionY + height, glyph);
		this.DrawLine(positionX + width, positionY + height, positionX + width, positionY, glyph);
		this.DrawLine(positionX + width, positionY, positionX, positionY, glyph);
	}

	public void DrawRectangleFilled(int positionX, int positionY, int width, int height, TerminalGlyph glyph)
	{
		for (int i = 0; i < height; i++)
		{
			for (int j = 0; j < width; j++)
			{
				DrawGlyph(positionX + j, positionY + i, glyph);
			}
		}
	}

	public void DrawFrame(int positionX, int positionY, int width, int height, BorderThickness thick, TerminalColor foregroundColor, TerminalColor backgroundColor)
	{
		var horGlyph = new TerminalGlyph(
				(char)(thick == BorderThickness.Light ? ExtraCharset.BorderLightHorizontal : ExtraCharset.BorderBoldHorizontal),
				foregroundColor,
				backgroundColor);
		var verGlyph = new TerminalGlyph(
				(char)(thick == BorderThickness.Light ? ExtraCharset.BorderLightVertical : ExtraCharset.BorderBoldVertical),
				foregroundColor,
				backgroundColor);
		var nwCorner = new TerminalGlyph(
				(char)(thick == BorderThickness.Light ? ExtraCharset.BorderLightNorthWestCorner : ExtraCharset.BorderBoldNorthWestCorner),
				foregroundColor,
				backgroundColor);
		var neCorner = new TerminalGlyph(
				(char)(thick == BorderThickness.Light ? ExtraCharset.BorderLightNorthEastCorner : ExtraCharset.BorderBoldNorthEastCorner),
				foregroundColor,
				backgroundColor);
		var swCorner = new TerminalGlyph(
				(char)(thick == BorderThickness.Light ? ExtraCharset.BorderLightSouthWestCorner : ExtraCharset.BorderBoldSouthWestCorner),
				foregroundColor,
				backgroundColor);
		var seCorner = new TerminalGlyph(
				(char)(thick == BorderThickness.Light ? ExtraCharset.BorderLightSouthEastCorner : ExtraCharset.BorderBoldSouthEastCorner),
				foregroundColor,
				backgroundColor);
		this.DrawLine(positionX, positionY, positionX, positionY + height, verGlyph);
		this.DrawLine(positionX, positionY + height, positionX + width, positionY + height, horGlyph);
		this.DrawLine(positionX + width, positionY + height, positionX + width, positionY, verGlyph);
		this.DrawLine(positionX + width, positionY, positionX, positionY, horGlyph);
		this.DrawGlyph(positionX, positionY, nwCorner);
		this.DrawGlyph(positionX + width, positionY, neCorner);
		this.DrawGlyph(positionX, positionY + height, swCorner);
		this.DrawGlyph(positionX + width, positionY + height, seCorner);
	}

	public void DrawFrameFilled(int positionX, int positionY, int width, int height, BorderThickness thick, TerminalColor foregroundColor, TerminalColor backgroundColor)
	{
		DrawRectangleFilled(positionX, positionY, width, height, new TerminalGlyph((char)ExtraCharset.BlockFull, backgroundColor, backgroundColor));
		DrawFrame(positionX, positionY, width, height, thick, foregroundColor, backgroundColor);
	}
}

public enum BorderThickness
{
	Light = 0,
	Bold = 1
}
