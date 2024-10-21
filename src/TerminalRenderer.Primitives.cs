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
}
