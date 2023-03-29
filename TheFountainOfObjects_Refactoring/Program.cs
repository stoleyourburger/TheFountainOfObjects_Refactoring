new FountainOfObjectsGame().Run();
//Console.Write("Choose map size (4, 6, 8): ");
//int input = Convert.ToInt32(Console.ReadLine());
//Map map = input switch
//{
//    4 => new Map,
//    6 => new Map(),
//    8 => new Map(),
//};
public class FountainOfObjectsGame
{
    public Map Map { get; }
    public Player Player { get; }
    public FountainRoom fountainRoom = new FountainRoom();



public FountainOfObjectsGame()
    {
        Map = new Map();
        Player = new Player();
    }

    public void Run()
    {



        while (!(Player.IsAlive == true && fountainRoom.IsOn && Map.GetRoom(Player.Coordinate) is EntranceRoom))
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
        Console.BackgroundColor = ConsoleColor.Black;
        Console.ForegroundColor = ConsoleColor.White;
    }
    public void Surrounding()
    {
        Room playersRoom = Map.GetRoom(Player.Coordinate);

        if (playersRoom is EntranceRoom)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("You see light in this room coming from outside the cavern. This is the entrance.");
        }
        else if (playersRoom is FountainRoom && fountainRoom.IsOff)
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("You hear water dripping in this room. The Fountain of Objects is here!");
        }
        else if (playersRoom is FountainRoom && fountainRoom.IsOn)
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
            Direction.North => new Coordinate(currentCoordinate.Row - 1, currentCoordinate.Column),
            Direction.South => new Coordinate(currentCoordinate.Row + 1, currentCoordinate.Column),
            Direction.East => new Coordinate(currentCoordinate.Row, currentCoordinate.Column + 1),
            Direction.West => new Coordinate(currentCoordinate.Row, currentCoordinate.Column - 1),
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
    public Coordinate Coordinate { get; set; } = new Coordinate(0, 0);
    public bool IsAlive { get; set; }

    public Player()
    {
        IsAlive = true;
    }
}
public class Map
{
    private readonly Room[,] _rooms;

    public int Rows;
    public int Columns;

    public Map()
    {
        Console.Write("Choose map size (4, 6, 8): ");
        int input = Convert.ToInt32(Console.ReadLine());
        switch (input)
        {
            case 4: Rows = 4; Columns = 4; break;
            case 6: Rows = 6; Columns = 6; break;
            case 8: Rows = 8; Columns = 8; break;
        }

        _rooms = new Room[Rows, Columns];
        for (int row = 0; row < Rows; row++)
            for (int column = 0; column < Columns; column++)
                _rooms[row, column] = new EmptyRoom();

        _rooms[0, 0] = new EntranceRoom();
        _rooms[0, 2] = new FountainRoom();
        _rooms[2, 1] = new PitRoom();

    }
    

    public Room GetRoom(int row, int column) => _rooms[row, column];
    public Room GetRoom(Coordinate coordinate) => _rooms[coordinate.Row, coordinate.Column];

    public bool OnMap(Coordinate coordinate)
    {
        if ((coordinate.Row == -1) || (coordinate.Column == -1)) return false;
        else if ((coordinate.Row == Rows) || (coordinate.Column == Columns)) return false;
        else return true;
    }

    public void Render(Player player)
    {
        char X = 'X';
        char Empty = ' ';
        char[,] room = new char[Rows, Columns];
        for (int row = 0; row < Rows; row++)
            for (int column = 0; column < Columns; column++)
                if ((player.Coordinate.Row == row) && (player.Coordinate.Column == column))
                    room[row, column] = X;
                else room[row, column] = Empty;

        if (_rooms.Length == 16)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"┌───┬───┬───┬───┐");
            Console.WriteLine($"│ {room[0, 0]} │ {room[0, 1]} │ {room[0, 2]} │ {room[0, 3]} │  {player.Coordinate}");
            Console.WriteLine($"├───┼───┼───┼───┤");
            Console.WriteLine($"│ {room[1, 0]} │ {room[1, 1]} │ {room[1, 2]} │ {room[1, 3]} │  Possible input:");
            Console.WriteLine($"├───┼───┼───┼───┤  n / s / e / w");
            Console.WriteLine($"│ {room[2, 0]} │ {room[2, 1]} │ {room[2, 2]} │ {room[2, 3]} │  When in fountain room:");
            Console.WriteLine($"├───┼───┼───┼───┤  on / off ");
            Console.WriteLine($"│ {room[3, 0]} │ {room[3, 1]} │ {room[3, 2]} │ {room[3, 3]} │");
            Console.WriteLine($"└───┴───┴───┴───┘");
        }
        else if (_rooms.Length == 36)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"┌───┬───┬───┬───┬───┬───┐");
            Console.WriteLine($"│ {room[0, 0]} │ {room[0, 1]} │ {room[0, 2]} │ {room[0, 3]} │ {room[0, 4]} │ {room[0, 5]} │  {player.Coordinate}");
            Console.WriteLine($"├───┼───┼───┼───┼───┼───┤");
            Console.WriteLine($"│ {room[1, 0]} │ {room[1, 1]} │ {room[1, 2]} │ {room[1, 3]} │ {room[1, 4]} │ {room[1, 5]} │  Possible input:");
            Console.WriteLine($"├───┼───┼───┼───┼───┼───┤  n / s / e / w");
            Console.WriteLine($"│ {room[2, 0]} │ {room[2, 1]} │ {room[2, 2]} │ {room[2, 3]} │ {room[2, 4]} │ {room[2, 5]} │  When in fountain room:");
            Console.WriteLine($"├───┼───┼───┼───┼───┼───┤  on / off ");
            Console.WriteLine($"│ {room[3, 0]} │ {room[3, 1]} │ {room[3, 2]} │ {room[3, 3]} │ {room[3, 4]} │ {room[3, 5]} │");
            Console.WriteLine($"├───┼───┼───┼───┼───┼───┤");
            Console.WriteLine($"│ {room[4, 0]} │ {room[4, 1]} │ {room[4, 2]} │ {room[4, 3]} │ {room[4, 4]} │ {room[4, 5]} │");
            Console.WriteLine($"├───┼───┼───┼───┼───┼───┤");
            Console.WriteLine($"│ {room[5, 0]} │ {room[5, 1]} │ {room[5, 2]} │ {room[5, 3]} │ {room[5, 4]} │ {room[5, 5]} │");
            Console.WriteLine($"└───┴───┴───┴───┴───┴───┘");
        }
        else if (_rooms.Length == 64)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"┌───┬───┬───┬───┬───┬───┬───┬───┐");
            Console.WriteLine($"│ {room[0, 0]} │ {room[0, 1]} │ {room[0, 2]} │ {room[0, 3]} │ {room[0, 4]} │ {room[0, 5]} │ {room[0, 6]} │ {room[0, 7]} │  {player.Coordinate}");
            Console.WriteLine($"├───┼───┼───┼───┼───┼───┼───┼───┤");
            Console.WriteLine($"│ {room[1, 0]} │ {room[1, 1]} │ {room[1, 2]} │ {room[1, 3]} │ {room[1, 4]} │ {room[1, 5]} │ {room[1, 6]} │ {room[1, 7]} │  Possible input:");
            Console.WriteLine($"├───┼───┼───┼───┼───┼───┼───┼───┤  n / s / e / w");
            Console.WriteLine($"│ {room[2, 0]} │ {room[2, 1]} │ {room[2, 2]} │ {room[2, 3]} │ {room[2, 4]} │ {room[2, 5]} │ {room[2, 6]} │ {room[2, 7]} │  When in fountain room:");
            Console.WriteLine($"├───┼───┼───┼───┼───┼───┼───┼───┤  on / off ");
            Console.WriteLine($"│ {room[3, 0]} │ {room[3, 1]} │ {room[3, 2]} │ {room[3, 3]} │ {room[3, 4]} │ {room[3, 5]} │ {room[3, 6]} │ {room[3, 7]} │");
            Console.WriteLine($"├───┼───┼───┼───┼───┼───┼───┼───┤");
            Console.WriteLine($"│ {room[4, 0]} │ {room[4, 1]} │ {room[4, 2]} │ {room[4, 3]} │ {room[4, 4]} │ {room[4, 5]} │ {room[4, 6]} │ {room[4, 7]} │");
            Console.WriteLine($"├───┼───┼───┼───┼───┼───┼───┼───┤");
            Console.WriteLine($"│ {room[5, 0]} │ {room[5, 1]} │ {room[5, 2]} │ {room[5, 3]} │ {room[5, 4]} │ {room[5, 5]} │ {room[5, 6]} │ {room[5, 7]} │");
            Console.WriteLine($"├───┼───┼───┼───┼───┼───┼───┼───┤");
            Console.WriteLine($"│ {room[6, 0]} │ {room[6, 1]} │ {room[6, 2]} │ {room[6, 3]} │ {room[6, 4]} │ {room[6, 5]} │ {room[6, 6]} │ {room[6, 7]} │");
            Console.WriteLine($"├───┼───┼───┼───┼───┼───┼───┼───┤");
            Console.WriteLine($"│ {room[7, 0]} │ {room[7, 1]} │ {room[7, 2]} │ {room[7, 3]} │ {room[7, 4]} │ {room[7, 5]} │ {room[7, 6]} │ {room[7, 7]} │");
            Console.WriteLine($"└───┴───┴───┴───┴───┴───┴───┴───┘");
        }
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
public class PitRoom : Room
{

}

public record Coordinate(int Row, int Column)
{
    public sealed override string ToString()
    {
        return $"You are at ({Row}, {Column})";
    }
}
public enum Direction { North, South, East, West }