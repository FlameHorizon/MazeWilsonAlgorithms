// See https://aka.ms/new-console-template for more information

using System.Numerics;

var maze = new char[4, 4];
FillArray(maze, '█');
PrintMaze(maze);

static void FillArray(char[,] array, char symbol)
{
    for (var i = 0; i < array.GetLength(0); i++)
    {
        for (var j = 0; j < array.GetLength(1); j++)
        {
            array[i, j] = symbol;
        }
    }
}

void PrintMaze(char[,] chars)
{
    for (var row = 0; row < chars.GetLength(0); row++)
    {
        for (var column = 0; column < chars.GetLength(1); column++)
        {
            Console.Write(chars[row, column]);
        }

        Console.WriteLine();
    }
}

// Pick random cell
int randomRow = Random.Shared.Next(0, (byte)maze.GetLength(0));
int randomColumn = Random.Shared.Next(0, maze.GetLength(1));

Console.WriteLine($"Picked random cell: {randomRow}, {randomColumn}");

// Randomly add cell to the maze
maze[randomRow, randomColumn] = ' ';
PrintMaze(maze);

// Randomly pick a starting point for our random walk
int randomWalkRow = Random.Shared.Next(0, maze.GetLength(0));
int randomWalkColumn = Random.Shared.Next(0, maze.GetLength(1));

// Helper variables
int randomWalkRowStart = randomWalkRow;
int randomWalkColumnStart = randomWalkColumn;

Console.WriteLine(
    $"Picked random walk start: {randomWalkRow}, {randomWalkColumn}");

// Keep track of where we are in the maze as we walk using
// direction arrows to show the path
const char up = '\u2191';
const char down = '\u2193';
const char left = '\u2190';
const char right = '\u2192';

while (true)
{
    // We want to also make sure that we are still inbound of the maze itself.
    // If we are out of bounds, we will pick a new random cell.
    // Make sure we haven't picked a cell which is already in the maze
    while (IsOutsideBounds(randomWalkRow, randomWalkColumn, maze) ||
           maze[randomWalkRow, randomWalkColumn] == ' ')
    {
        randomWalkRow = Random.Shared.Next(0, maze.GetLength(0));
        randomWalkColumn = Random.Shared.Next(0, maze.GetLength(1));
    }

    // Pick a random direction which will be our first step
    var randomDirection = (MoveDirection)Random.Shared.Next(0, 4);
    Console.WriteLine($"Picked random direction: {randomDirection}");

    Vector2 vector = randomDirection switch
    {
        MoveDirection.Up => new Vector2(0, -1),
        MoveDirection.Down => new Vector2(0, 1),
        MoveDirection.Left => new Vector2(-1, 0),
        MoveDirection.Right => new Vector2(1, 0),
        _ => Vector2.Zero
    };

    if (IsOutsideBounds(randomWalkRow + (int)vector.Y,
            randomWalkColumn + (int)vector.X, maze))
    {
        Console.WriteLine(
            "Will not update the maze, point is outside of the maze.");
        continue;
    }

    // Check if we are hitting the cell which is already part of a maze
    // If yes exit the loop
    if (maze[randomWalkRow + (int)vector.Y, randomWalkColumn + (int)vector.X] ==
        ' ')
    {
        Console.WriteLine("Hit a cell which is already part of a maze");
        break;
    }

    // Update random walk position and check if it is still in bounds,
    // if not pick a new random cell
    int previousRandomWalkRow = randomWalkRow;
    int previousRandomWalkColumn = randomWalkColumn;

    randomWalkRow += (int)vector.Y;
    randomWalkColumn += (int)vector.X;

    char directionSymbol = randomDirection switch
    {
        MoveDirection.Up => up,
        MoveDirection.Down => down,
        MoveDirection.Left => left,
        MoveDirection.Right => right,
        _ => ' '
    };

    maze[previousRandomWalkRow, previousRandomWalkColumn] = directionSymbol;
    PrintMaze(maze);

    //Thread.Sleep(2000);
}

// Mark the starting point of the random walk as a part of the maze
char direction = maze[randomWalkRowStart, randomWalkColumnStart];
Vector2 v = direction switch
{
    up => new Vector2(0, -1),
    down => new Vector2(0, 1),
    left => new Vector2(-1, 0),
    right => new Vector2(1, 0),
    _ => Vector2.Zero
};

// Walk from the start of the random walk to cell where we have a cell
// which is already part of a maze.
int row = randomWalkRowStart + (int)v.Y;
int column = randomWalkColumnStart + (int)v.X;

while (maze[row, column] != ' ')
{
    direction = maze[row, column];
    v = direction switch
    {
        up => new Vector2(0, -1),
        down => new Vector2(0, 1),
        left => new Vector2(-1, 0),
        right => new Vector2(1, 0),
        _ => Vector2.Zero
    };

    maze[row, column] = ' ';

    row += (int)v.Y;
    column += (int)v.X;
    
}

Console.WriteLine("Updated the maze with a new path");
PrintMaze(maze);

bool IsOutsideBounds(int row, int column, char[,] maze)
{
    return row < 0 || row >= maze.GetLength(0) ||
           column < 0 || column >= maze.GetLength(1);
}

public enum MoveDirection
{
    Up,
    Down,
    Left,
    Right
}