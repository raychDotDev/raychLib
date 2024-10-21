using System.Numerics;
using Raylib_CSharp.Interact;

namespace raychLib;

public class TerminalInputController
{
	public TerminalInputController()
	{
		Input.SetExitKey(KeyboardKey.Null);
	}

	public bool IsKeyPressed(TerminalKey key)
	{
		return Input.IsKeyPressed((KeyboardKey)key);
	}

	public bool IsKeyDown(TerminalKey key)
	{
		return Input.IsKeyDown((KeyboardKey)key);
	}

	public bool IsKeyReleased(TerminalKey key)
	{
		return Input.IsKeyReleased((KeyboardKey)key);
	}

	public bool IsMouseButtonPressed(TerminalMouseButton button)
	{
		return Input.IsMouseButtonPressed((MouseButton)button);
	}

	public bool IsMouseButtonDown(TerminalMouseButton button)
	{
		return Input.IsMouseButtonDown((MouseButton)button);
	}

	public bool IsMouseButtonReleased(TerminalMouseButton button)
	{
		return Input.IsMouseButtonReleased((MouseButton)button);
	}

	public void SetMouseCursor(MouseCursor cursor)
	{
		Input.SetMouseCursor((Raylib_CSharp.Interact.MouseCursor)cursor);
	}

	public Vector2 GetMousePosition()
	{
		Vector2 mouseRawPos = Input.GetMousePosition();
		Vector2 windowSize = new(Window.GetScreenWidth(), Window.GetScreenHeight());
		var buffer = Terminal.Shared.getRenderer().Buffer;
		Vector2 bufferSize = new(buffer.GetLength(1), buffer.GetLength(0));
		Vector2 windowSizeScaled = RayMath.Vector2Divide(windowSize, bufferSize);
		return RayMath.Vector2Divide(mouseRawPos, windowSizeScaled);
	}

	public void HideCursor() => Input.HideCursor();

	public void ShowCursor() => Input.ShowCursor();
}

public enum TerminalKey
{
	/// <summary>
	/// NULL, used for no key pressed
	/// </summary>
	Null = 0,

	// Alphanumeric keys
	Apostrophe = 39,
	Comma = 44,
	Minus = 45,
	Period = 46,
	Slash = 47,
	Zero = 48,
	One = 49,
	Two = 50,
	Three = 51,
	Four = 52,
	Five = 53,
	Six = 54,
	Seven = 55,
	Eight = 56,
	Nine = 57,
	Semicolon = 59,
	Equal = 61,
	A = 65,
	B = 66,
	C = 67,
	D = 68,
	E = 69,
	F = 70,
	G = 71,
	H = 72,
	I = 73,
	J = 74,
	K = 75,
	L = 76,
	M = 77,
	N = 78,
	O = 79,
	P = 80,
	Q = 81,
	R = 82,
	S = 83,
	T = 84,
	U = 85,
	V = 86,
	W = 87,
	X = 88,
	Y = 89,
	Z = 90,

	// Function keys
	Space = 32,
	Escape = 256,
	Enter = 257,
	Tab = 258,
	Backspace = 259,
	Insert = 260,
	Delete = 261,
	Right = 262,
	Left = 263,
	Down = 264,
	Up = 265,
	PageUp = 266,
	PageDown = 267,
	Home = 268,
	End = 269,
	CapsLock = 280,
	ScrollLock = 281,
	NumLock = 282,
	PrintScreen = 283,
	Pause = 284,
	F1 = 290,
	F2 = 291,
	F3 = 292,
	F4 = 293,
	F5 = 294,
	F6 = 295,
	F7 = 296,
	F8 = 297,
	F9 = 298,
	F10 = 299,
	F11 = 300,
	F12 = 301,
	LeftShift = 340,
	LeftControl = 341,
	LeftAlt = 342,
	LeftSuper = 343,
	RightShift = 344,
	RightControl = 345,
	RightAlt = 346,
	RightSuper = 347,
	KeyboardMenu = 348,
	LeftBracket = 91,
	Backslash = 92,
	RightBracket = 93,
	Grave = 96,

	// Keypad keys
	Kp0 = 320,
	Kp1 = 321,
	Kp2 = 322,
	Kp3 = 323,
	Kp4 = 324,
	Kp5 = 325,
	Kp6 = 326,
	Kp7 = 327,
	Kp8 = 328,
	Kp9 = 329,
	KpDecimal = 330,
	KpDivide = 331,
	KpMultiply = 332,
	KpSubtract = 333,
	KpAdd = 334,
	KpEnter = 335,
	KpEqual = 336,

}

public enum TerminalMouseButton
{
	/// <summary>
	/// Mouse button left
	/// </summary>
	Left = 0,

	/// <summary>
	/// Mouse button right
	/// </summary>
	Right = 1,

	/// <summary>
	/// Mouse button middle (pressed wheel)
	/// </summary>
	Middle = 2,

	/// <summary>
	/// Mouse button side (advanced mouse device)
	/// </summary>
	Side = 3,

	/// <summary>
	/// Mouse button extra (advanced mouse device)
	/// </summary>
	Extra = 4,

	/// <summary>
	/// Mouse button forward (advanced mouse device)
	/// </summary>
	Forward = 5,

	/// <summary>
	/// Mouse button back (advanced mouse device)
	/// </summary>
	Back = 6
}

public enum MouseCursor
{
	/// <summary>
	/// Default pointer shape
	/// </summary>
	Default = 0,

	/// <summary>
	/// Arrow shape
	/// </summary>
	Arrow = 1,

	/// <summary>
	/// Text writing cursor shape
	/// </summary>
	IBeam = 2,

	/// <summary>
	/// Cross shape
	/// </summary>
	Crosshair = 3,

	/// <summary>
	/// Pointing hand cursor
	/// </summary>
	PointingHand = 4,

	/// <summary>
	/// Horizontal resize/move arrow shape
	/// </summary>
	ResizeEw = 5,

	/// <summary>
	/// Vertical resize/move arrow shape
	/// </summary>
	ResizeNs = 6,

	/// <summary>
	/// Top-left to bottom-right diagonal resize/move arrow shape
	/// </summary>
	ResizeNwse = 7,

	/// <summary>
	/// The top-right to bottom-left diagonal resize/move arrow shape
	/// </summary>
	ResizeNesw = 8,

	/// <summary>
	/// The omnidirectional resize/move cursor shape
	/// </summary>
	ResizeAll = 9,

	/// <summary>
	/// The operation-not-allowed shape
	/// </summary>
	NotAllowed = 10
}
