using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePiece : MonoBehaviour
{
	private int x;
	private int y;

	public int X
	{
		get { return x; }
		set { if (IsMovable()) x = value; }
	}
	public int Y
	{
		get { return y; }
		set { if (IsMovable()) y = value; }
	}
	private PieceType type;
	public PieceType Type
	{
		get { return type; }
	}
	private Grid grid;
	public Grid GridRef
	{
		get { return grid; }
	}
	private MovablePiece movableComponent;
	public MovablePiece MovableComponent
	{
		get { return movableComponent; }
	}
	private ColorPiece colorComponent;
	public ColorPiece ColorComponent
	{
		get { return colorComponent; }
	}
	private ClearablePiece clearableComponent;
	public ClearablePiece ClearableComponent
	{
		get { return clearableComponent; }
	}
	private void Awake()
	{
		movableComponent = GetComponent<MovablePiece>();
		colorComponent = GetComponent<ColorPiece>();
		clearableComponent = GetComponent<ClearablePiece>();
	}
	public void Init(int _x, int _y, Grid _grid, PieceType _type)
	{
		x = _x;
		y = _y;
		grid = _grid;
		type = _type;
	}
	private void OnMouseEnter()
	{
		grid.EnterPiece(this);
	}
	private void OnMouseDown()
	{
		grid.PressPiece(this);
	}
	private void OnMouseUp()
	{
		grid.ReleasePiece();
	}
	public bool IsMovable()
	{
		return movableComponent != null;
	}	
	public bool IsColored()
	{
		return colorComponent != null;
	}	public bool IsClearable()
	{
		return clearableComponent != null;
	}
}
