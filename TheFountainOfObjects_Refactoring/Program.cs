//Fountain enable/disable
//Colors

Map map = new Map();
Player player = new Player();
FountainOfObjectsGame game = new FountainOfObjectsGame(map, player);
game.Run();

public class FountainOfObjectsGame
{
    public Map Map { get; }
    public Player Player { get; }
    public FountainRoom fountainRoom = new FountainRoom();

    public FountainOfObjectsGame(Map map, Player player)
    {
        Map = map;
        Player = player;
    }

    public void Run()
    {
        while (!(Player.IsAlive == true && fountainRoom.IsOn && Player.Coordinate.row == 0 && Player.Coordinate.column ==0))
        {
            Map.Render(Player);
            Surrounding();

            ICommand command = GetCommand();
            command.Execute(this);

            ICommand GetCommand()
            {
                while (true)
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine("What do you want to do? ");
                    string? input = Console.ReadLine();
                    if (input == "n") return new Move(Direction.North);
                    if (input == "s") return new Move(Direction.South);
                    if (input == "e") return new Move(Direction.East);
                    if (input == "w") return new Move(Direction.West);
                    if (input == "on") return new EnableFountain();
                    if (input == "off") return new DisableFountain();
                    Console.WriteLine($"I did not understand '{input}'.");
                }
            }
        }
        Map.Render(Player);
        Console.BackgroundColor = ConsoleColor.White;
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("Congratulations! You won!");
        Console.BackgroundColor= ConsoleColor.Black;
        Console.ForegroundColor = ConsoleColor.White;
    }
    public void Surrounding()
    {
        if (Player.Coordinate.row == 0 && Player.Coordinate.column == 0)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("You see light in this room coming from outside the cavern. This is the entrance.");
        }
        else if (Player.Coordinate.row == 0 && Player.Coordinate.column == 2 && fountainRoom.IsOff)
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("You hear water dripping in this room. The Fountain of Objects is here!");
        }
        else if (Player.Coordinate.row == 0 && Player.Coordinate.column == 2 && fountainRoom.IsOn)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("You hear the rushing waters from the Fountain of Objects. It has been reactivated!");
        }
    }
}


public interface ICommand
{
    void Execute(FountainOfObjectsGame game);
}

public class EnableFountain : ICommand
{
    public void Execute(FountainOfObjectsGame game)
    {
        Console.Clear();
        FountainRoom fountainRoom = game.fountainRoom;
        fountainRoom.IsOn = true;
    }
}

public class DisableFountain : ICommand
{
    public void Execute(FountainOfObjectsGame game)
    {
        Console.Clear();
        FountainRoom fountainRoom = game.fountainRoom;
        fountainRoom.IsOn = false;
    }
}

public class Move : ICommand
{
    public Direction Direction { get; }

    public Move(Direction direction)
    {
        Direction = direction;
    }

    public void Execute(FountainOfObjectsGame game)
    {
        Coordinate currentCoordinate = game.Player.Coordinate;
        Coordinate newCoordinate = Direction switch
        {
            Direction.North => new Coordinate(currentCoordinate.row - 1, currentCoordinate.column),
            Direction.South => new Coordinate(currentCoordinate.row + 1, currentCoordinate.column),
            Direction.East => new Coordinate(currentCoordinate.row, currentCoordinate.column + 1),
            Direction.West => new Coordinate(currentCoordinate.row, currentCoordinate.column - 1),
            _ => throw new NotImplementedException(),
        };
        Console.Clear();
        if (game.Map.OnMap(newCoordinate))
        game.Player.Coordinate = newCoordinate;
        else 
        { 
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("You cannot go there.");
        }
        
    }
}

public class Player
{
    public Coordinate Coordinate { get; set; }
    public bool IsAlive { get; set; }

    public Player()
    {
        Coordinate = new Coordinate(0, 0);
        IsAlive = true;
    }
}

public class Map
{
    private Room[,] _rooms = new Room[4, 4];
    private int Rows = 4;
    private int Columns = 4;

    public Map()
    {
        for (int row = 0; row < Rows; row++)
            for (int column = 0; column < Columns; column++)
                _rooms[row, column] = new EmptyRoom();

        _rooms[0, 0] = new EntranceRoom();
        _rooms[0, 2] = new FountainRoom();
    }

    public Room GetRoom(int row, int column) => _rooms[row, column];

    public bool OnMap(Coordinate coordinate)
    {
        if ((coordinate.row == -1) || (coordinate.column == -1)) return false;
        else if ((coordinate.row == 4) || (coordinate.column == 4)) return false;
        else return true;
    }

    public void Render(Player player)
    {
        char X = 'X';
        char Empty = ' ';
        char[,] room = new char[4, 4];
        for (int row = 0; row < 4; row++)
            for (int column = 0; column < 4; column++)
                if ((player.Coordinate.row == row) && (player.Coordinate.column == column))
                    room[row, column] = X;
                else room[row, column] = Empty;
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"┌───┬───┬───┬───┐" );
        Console.WriteLine($"│ {room[0, 0]} │ {room[0, 1]} │ {room[0, 2]} │ {room[0, 3]} │  {player.Coordinate}");
        Console.WriteLine($"├───┼───┼───┼───┤");
        Console.WriteLine($"│ {room[1, 0]} │ {room[1, 1]} │ {room[1, 2]} │ {room[1, 3]} │  Possible input:");
        Console.WriteLine($"├───┼───┼───┼───┤  n / s / e / w");
        Console.WriteLine($"│ {room[2, 0]} │ {room[2, 1]} │ {room[2, 2]} │ {room[2, 3]} │  When in fountain room:");
        Console.WriteLine($"├───┼───┼───┼───┤  on / off ");
        Console.WriteLine($"│ {room[3, 0]} │ {room[3, 1]} │ {room[3, 2]} │ {room[3, 3]} │");
        Console.WriteLine($"└───┴───┴───┴───┘");
    }
}

public abstract class Room { }
public class EmptyRoom : Room { }
public class EntranceRoom : Room { }
public class FountainRoom : Room
{
    public bool IsOn { get; set; }
    public bool IsOff => !IsOn;
}

public record Coordinate(int row, int column)
{
    public sealed override string ToString()
    {
        return $"You are at ({row}, {column})";
    }
}
public enum Direction { North, South, East, West }