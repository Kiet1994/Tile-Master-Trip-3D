using UnityEngine;
using System.Collections;
using System.Collections.Generic;
[System.Serializable]
public class PiecePrefab
{
	public PieceType type;
	public GameObject prefab;
}
[System.Serializable]
public struct PiecePosition
{
	public PieceType type;
	public int x;
	public int y;
}
public enum PieceType
{
	EMPTY,
	NORMAL,
	BUBBLE,
	COUNT,
};
public class Grid : MonoBehaviour
{

	public int xDim;
	public int yDim;
	public float fillTime;
	//fill [] at inspector
	public PiecePrefab[] piecePrefabs;
	public GameObject backgroundPrefab;

	public PiecePosition[] initialPieces;	

	private Dictionary<PieceType, GameObject> piecePrefabDict;

	private GamePiece[,] pieces;

	private bool inverse = false;

	private GamePiece pressedPiece;
	private GamePiece enteredPiece;
	private void Start()
	{
		piecePrefabDict = new Dictionary<PieceType, GameObject>();
		for (int i = 0; i < piecePrefabs.Length; i++) //add [] PiecePrefabs to Dictionary
		{
			if (!piecePrefabDict.ContainsKey(piecePrefabs[i].type))
			{
				piecePrefabDict.Add(piecePrefabs[i].type, piecePrefabs[i].prefab);
			}
		}
		for (int x = 0; x < xDim; x++) // �ntantiate Backdround piece
		{
			for (int y = 0; y < xDim; y++)
			{
				GameObject background = Instantiate(backgroundPrefab, GetWorldPosition(x, y), Quaternion.identity);
				background.transform.parent = transform;
			}
		}

		pieces = new GamePiece[xDim, yDim];	

		for (int i = 0; i < initialPieces.Length; i++)
		{
			if (initialPieces[i].x >=0 && initialPieces[i].x < xDim
				&& initialPieces[i].y >= 0 && initialPieces[i].y < yDim)
			{
				SpawnNewPiece(initialPieces[i].x, initialPieces[i].y, initialPieces[i].type);
			}
		}

		// create pieces EMPTY
		for (int x = 0; x < xDim; x++)
		{
			for (int y = 0; y < xDim; y++)
			{
				if(pieces[x,y] == null) SpawnNewPiece(x, y, PieceType.EMPTY);
			}
		}

		StartCoroutine(Fill());

	}
	public IEnumerator Fill()
	{
		bool needsRefill = true;

		while (needsRefill)
		{
			yield return new WaitForSeconds(fillTime);

			while (FillStep())
			{
				inverse = !inverse;
				yield return new WaitForSeconds(fillTime);
			}

			needsRefill = ClearAllValidMatches();
		}
		while (CheckAllMalth() == false)
		{
			ClearAllPiece();
			yield return new WaitForSeconds(fillTime);
		}
	}
	public bool FillStep()
	{
		bool movedPiece = false;
		for (int y = yDim - 2; y >= 0; y--) // y = 3,2,1,0 check row from down to up // move down piece nomal  
		{
			for (int loopX = 0; loopX < xDim; loopX++) // 0, 1, 2, 3, 4
			{
				int x = loopX;

				if (inverse)
				{
					x = xDim - 1 - loopX; // 4, 3, 2, 1 ,0 
				}
				//Debug.Log(x + " x , y " + y);
				GamePiece piece = pieces[x, y];
				if (piece.IsMovable())// if have piece nomal
				{
					GamePiece pieceBelow = pieces[x, y + 1];
					//Debug.Log(pieceBelow.X + " below " + pieceBelow.Y);
					if (pieceBelow.Type == PieceType.EMPTY) // if piece empty below piece nomal
					{
						Destroy(pieceBelow.gameObject);
						piece.MovableComponent.Move(x, y + 1, fillTime); //move to piece empty
						pieces[x, y + 1] = piece; // set piece nomal to []
						//Debug.Log(piece.X + " xxxxxxxxxx " + piece.Y + "name " + piece.name);
						SpawnNewPiece(x, y, PieceType.EMPTY); // set piece empty to []
						movedPiece = true;
					}
					else // 
					{
						for (int diag = -1; diag <= 1; diag++) // -1 , 1
						{
							if (diag != 0) //
							{
								int diagX = x + diag; // -1 , 0 ,1 ,2 3 . 1 2 3 4 5

								if (inverse)
								{
									diagX = x - diag;
								}
								if (diagX >= 0 && diagX < xDim)
								{
									GamePiece diagonalPiece = pieces[diagX, y + 1];
									if (diagonalPiece.Type == PieceType.EMPTY)
									{
										bool hasPieceAbove = true;
										for (int aboveY = y; aboveY >= 0; aboveY--)
										{
											GamePiece pieceAbove = pieces[diagX, aboveY];
											if (pieceAbove.IsMovable())
											{
												break;
											}
											else if (!pieceAbove.IsMovable() && pieceAbove.Type != PieceType.EMPTY)
											{
												hasPieceAbove = false;
												break;
											}
										}
										if (!hasPieceAbove)
										{
											Destroy(diagonalPiece.gameObject);
											piece.MovableComponent.Move(diagX, y + 1, fillTime);
											pieces[diagX, y + 1] = piece;
											SpawnNewPiece(x, y, PieceType.EMPTY);
											movedPiece = true;
											break;
										}
									}
								}
							}
						}
					}
				}
			}
		} // stop movePiece = true when all prices row 3 2 1 0 is nomal

		for (int x = 0; x < xDim; x++) //instantiate 1 row piece nomal at top
		{
			GamePiece pieceBelow = pieces[x, 0];
			if (pieceBelow.Type == PieceType.EMPTY) //stop when all prices row 0 is nomal
			{
				Destroy(pieceBelow.gameObject);
				GameObject newPiece = Instantiate(piecePrefabDict[PieceType.NORMAL], GetWorldPosition(x, -1), Quaternion.identity); // 
				newPiece.transform.parent = transform;

				pieces[x, 0] = newPiece.GetComponent<GamePiece>();
				pieces[x, 0].Init(x, -1, this, PieceType.NORMAL); // 0,-1 ; 1, -1 ; 2 , -1; 
				pieces[x, 0].MovableComponent.Move(x, 0, fillTime); //
				pieces[x, 0].ColorComponent.SetColor((ColorType)Random.Range(0, pieces[x, 0].ColorComponent.NumColors));
				movedPiece = true;
			}
		}

		return movedPiece;

	}
	// set position all
	public Vector2 GetWorldPosition(int x, int y)
	{
		return new Vector2(transform.position.x - xDim / 2 + x,
			transform.position.y + yDim / 2 - y);
	}
	public GamePiece SpawnNewPiece(int x, int y, PieceType type)
	{
		GameObject newPiece = Instantiate(piecePrefabDict[type], GetWorldPosition(x, y), Quaternion.identity);
		newPiece.transform.parent = transform;

		pieces[x, y] = newPiece.GetComponent<GamePiece>();
		pieces[x, y].Init(x, y, this, type);

		return pieces[x, y];
	}
	public bool IsAdjacent(GamePiece piece1, GamePiece piece2) // near
	{
		var y = (piece1.X == piece2.X && (int)Mathf.Abs(piece1.Y - piece2.Y) == 1)
			|| (piece1.Y == piece2.Y && (int)Mathf.Abs(piece1.X - piece2.X) == 1);
		return y;
	}

	public void SwapPieces(GamePiece piece1, GamePiece piece2)
	{
		if (piece1.IsMovable() && piece2.IsMovable())
		{
			pieces[piece1.X, piece1.Y] = piece2;
			pieces[piece2.X, piece2.Y] = piece1;

			if (GetMatch(piece1, piece2.X, piece2.Y) != null || GetMatch(piece2, piece1.X, piece1.Y) != null) // if if either piece has a Malth
			{
				int piece1X = piece1.X;
				int piece1Y = piece1.Y;

				piece1.MovableComponent.Move(piece2.X, piece2.Y, fillTime);
				piece2.MovableComponent.Move(piece1X, piece1Y, fillTime);

				ClearAllValidMatches();

				StartCoroutine(Fill());
			}
			else
			{
				pieces[piece1.X, piece1.Y] = piece1;
				pieces[piece2.X, piece2.Y] = piece2;
			}
		}
	}

	public void PressPiece(GamePiece piece)
	{
		pressedPiece = piece;
	}
	public void EnterPiece(GamePiece piece)
	{
		enteredPiece = piece;
	}
	public void ReleasePiece()
	{
		if (IsAdjacent(pressedPiece, enteredPiece))
		{
			SwapPieces(pressedPiece, enteredPiece);
		}
	}

	public List<GamePiece> GetMatch(GamePiece piece, int newX, int newY)
	{
		if (piece.IsColored()) // if have color
		{
			ColorType color = piece.ColorComponent.Color;
			List<GamePiece> horizontalPieces = new List<GamePiece>();
			List<GamePiece> verticalPieces = new List<GamePiece>();
			List<GamePiece> matchingPieces = new List<GamePiece>();
			// First check horizontal
			horizontalPieces.Add(piece); // add in list ngang
			for (int dir = 0; dir <= 1; dir++) // 0 , 1
			{
				for (int xOffset = 1; xOffset < xDim; xOffset++) // 1, 2, 3, 4
				{
					int x; // 

					if (dir == 0) //left
					{
						x = newX - xOffset; // 
					}
					else // right
					{
						x = newX + xOffset;
					}
					if (x < 0 || x >= xDim)
					{
						break;
					}

					if (pieces[x, newY].IsColored() && pieces[x, newY].ColorComponent.Color == color)
					{
						horizontalPieces.Add(pieces[x, newY]);
					}
					else
					{
						break;
					}
				}
			}

			if (horizontalPieces.Count >= 3) // if malth 3 4 5 6
			{
				for (int i = 0; i < horizontalPieces.Count; i++)
				{
					matchingPieces.Add(horizontalPieces[i]); // add form list horizontal to list matchingPieces
				}
			}
			// Traverse vertically if we found a match (for L and T shape)
			if (horizontalPieces.Count >= 3)
			{
				for (int i = 0; i < horizontalPieces.Count; i++)
				{
					for (int dir = 0; dir <= 1; dir++)
					{
						for (int yOffset = 1; yOffset < yDim; yOffset++)
						{
							int y;

							if (dir == 0) // up
							{
								y = newY - yOffset;
							}
							else // down
							{
								y = newY + yOffset;
							}
							if (y < 0 || y >= yDim)
							{
								break;
							}

							if (pieces[horizontalPieces[i].X, y].IsColored() && pieces[horizontalPieces[i].X, y].ColorComponent.Color == color)
							{
								verticalPieces.Add(pieces[horizontalPieces[i].X, y]);
							}
							else
							{
								break;
							}
						}
					}
					if (verticalPieces.Count < 2)
					{
						verticalPieces.Clear();
					}
					else
					{
						for (int j = 0; j < verticalPieces.Count; j++)
						{
							matchingPieces.Add(verticalPieces[j]);
						}
						break;
					}
				}
			}

			if (matchingPieces.Count >= 3)
			{
				return matchingPieces;
			}

			// Didn't find anything going horizontally first,
			// so now check vertically
			horizontalPieces.Clear();
			verticalPieces.Clear();
			verticalPieces.Add(piece);

			for (int dir = 0; dir <= 1; dir++)
			{
				for (int yOffset = 1; yOffset < yDim; yOffset++) // 1, 2, 3, 4
				{
					int y;

					if (dir == 0) // Up
					{
						y = newY - yOffset;
					}
					else // Down
					{
						y = newY + yOffset;
					}
					if (y < 0 || y >= yDim)
					{
						break;
					}

					if (pieces[newX, y].IsColored() && pieces[newX, y].ColorComponent.Color == color)
					{
						verticalPieces.Add(pieces[newX, y]);
						//Debug.Log(pieces[newX, y].ColorComponent.Color + " x " + pieces[newX, y].x + "  y " + pieces[newX, y].y);
					}
					else
					{
						break;
					}
				}
			}
			if (verticalPieces.Count >= 3)
			{
				for (int i = 0; i < verticalPieces.Count; i++)
				{
					matchingPieces.Add(verticalPieces[i]);
				}
			}

			//Traverse horizontally if we found a match(for L and T shape)
			if (verticalPieces.Count >= 3)
			{
				for (int i = 0; i < verticalPieces.Count; i++)
				{
					for (int dir = 0; dir <= 1; dir++)
					{
						for (int xOffset = 1; xOffset < xDim; xOffset++)
						{
							int x;

							if (dir == 0) // Left
							{
								x = newX - xOffset;
							}
							else // Right
							{
								x = newX + xOffset;
							}
							if (x < 0 || x >= xDim)
							{
								break;
							}

							if (pieces[x, verticalPieces[i].Y].IsColored() && pieces[x, verticalPieces[i].Y].ColorComponent.Color == color)
							{
								horizontalPieces.Add(pieces[x, verticalPieces[i].Y]);
							}
							else
							{
								break;
							}
						}
					}
					if (horizontalPieces.Count < 2)
					{
						horizontalPieces.Clear();
					}
					else
					{
						for (int j = 0; j < horizontalPieces.Count; j++)
						{
							matchingPieces.Add(horizontalPieces[j]);
						}
						break;
					}
				}
			}

			foreach (GamePiece x in matchingPieces)
			{
				Debug.Log(x.ColorComponent.Color + " xxxx " + x.X + " yyyy " + x.Y);
			}
			Debug.Log(matchingPieces.Count);

			if (matchingPieces.Count >= 3)
			{
				return matchingPieces;
			}

		}
		return null;
	}
	public bool ClearAllValidMatches()
	{
		bool needsRefill = false;

		for (int y = 0; y < yDim; y++)
		{
			for (int x = 0; x < xDim; x++)
			{
				if (pieces[x, y].IsClearable())
				{
					List<GamePiece> match = GetMatch(pieces[x, y], x, y);

					if (match != null)
					{
						for (int i = 0; i < match.Count; i++)
						{
							if (ClearPiece(match[i].X, match[i].Y))
							{
								needsRefill = true;
							}
						}
					}
				}
			}
		}
		return needsRefill;
	}

	public bool ClearPiece(int x, int y)
	{
		if (pieces[x, y].IsClearable() && !pieces[x, y].ClearableComponent.IsBeingCleared)
		{
			pieces[x, y].ClearableComponent.Clear();
			SpawnNewPiece(x, y, PieceType.EMPTY);

			return true;
		}
		return false;
	}

	public void ClearAllPiece()
	{
		for (int x = 0; x < xDim; x++)
		{
			for (int y = 0; y < xDim; y++)
			{
				ClearPiece(x,y);
			}
		}
		StartCoroutine(Fill());
	}

	public bool CheckAllMalth()
	{
		bool haveMalth = false;
		for (int y = 0; y < xDim - 1; y++)
		{
			for (int x = 0; x < xDim - 1; x++)
			{
				var piece1 = pieces[x, y];
				var piece2 = pieces[x + 1, y];
				var piece3 = pieces[x, y + 1];
				if (piece1.IsMovable() && piece2.IsMovable())
				{
					if (piece1.ColorComponent.Color != piece2.ColorComponent.Color)
					{
						pieces[x, y] = piece2;
						pieces[x + 1, y] = piece1;
						if (GetMatch(piece1, piece2.X, piece2.Y) != null || GetMatch(piece2, piece1.X, piece1.Y) != null)
						{
							haveMalth = true;
						}
						pieces[x, y] = piece1;
						pieces[x + 1, y] = piece2;
						if (haveMalth) break;
					}
				}

				if (piece1.IsMovable() && piece3.IsMovable())
				{
					if (piece1.ColorComponent.Color != piece3.ColorComponent.Color)
					{
						pieces[x, y] = piece3;
						pieces[x, y + 1] = piece1;
						if (GetMatch(piece1, piece3.X, piece3.Y) != null || GetMatch(piece3, piece1.X, piece1.Y) != null)
						{
							haveMalth = true;
						}
						pieces[x, y] = piece1;
						pieces[x , y + 1] = piece3;
						if (haveMalth) break;
					}
				}
			}
		}
		Debug.Log(haveMalth);
		return haveMalth;
	}
}