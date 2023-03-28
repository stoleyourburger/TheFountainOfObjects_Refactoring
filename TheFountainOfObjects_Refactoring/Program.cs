//Player is in 0,0
//Fountain is in 0,2
//Fountain enable/disable
//Player can move
//Colors

Map map = new Map();
Player player = new Player();
FountainOfObjectsGame game = new FountainOfObjectsGame(map, player);
game.Run();

public class FountainOfObjectsGame
{
    public Map Map { get; }
    public Player Player { get; }

    public FountainOfObjectsGame(Map map, Player player)
    {
        Map = map;
        Player = player;
    }

    public void Run()
    {
        Renderer renderer = new Renderer();
        while (true)
        {
            renderer.Render(Map);
            Console.WriteLine(Player.Coordinate);

            ICommand command = GetCommand();
            command.Execute(this);

            ICommand GetCommand()
            {
                while (true)
                {
                    Console.WriteLine("What do you want to do? ");
                    string? input = Console.ReadLine();
                    if (input == "n") return new ActionCommand(Direction.North);
                    if (input == "s") return new ActionCommand(Direction.South);
                    if (input == "e") return new ActionCommand(Direction.East);
                    if (input == "w") return new ActionCommand(Direction.West);
                }
            }
        }
    }
}
//
//                // Check for a match with each known command.
//                //if (input == "enable fountain") return new EnableFountainCommand();
//                // More commands go here.
//
//                // If none of the above were a match, we have no clue what the command was. Try again.
//                Console.WriteLine($"I did not understand '{input}'.", ConsoleColor.Red);
//            }
//      }

public interface ICommand
{
    void Execute(FountainOfObjectsGame game);
}

public class ActionCommand : ICommand
{
    public Direction Direction { get; }

    public ActionCommand(Direction direction)
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
        };
        //
        //           //if (game.Map.IsOnMap(newCoordinate))
        game.Player.Coordinate = newCoordinate;
        //           //else
        //           //    Console.WriteLine("There is a wall there.");
    }
}

public class Player
{
    public Coordinate Coordinate { get; set; }

    public Player()
    {
        Coordinate = new Coordinate(0, 0);
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
}

public class Renderer
{
    public void Render(Map map)
    {
        char[,] room = new char[4, 4];
        for (int row = 0; row < 4; row++)
            for (int column = 0; column < 4; column++)
                room[row, column] = ToChar(map.GetRoom(row, column));

        Console.WriteLine($"┌───┬───┬───┬───┐");
        Console.WriteLine($"│ {room[0, 0]} │ {room[0, 1]} │ {room[0, 2]} │ {room[0, 3]} │");
        Console.WriteLine($"├───┼───┼───┼───┤");
        Console.WriteLine($"│ {room[1, 0]} │ {room[1, 1]} │ {room[1, 2]} │ {room[1, 3]} │");
        Console.WriteLine($"├───┼───┼───┼───┤");
        Console.WriteLine($"│ {room[2, 0]} │ {room[2, 1]} │ {room[2, 2]} │ {room[2, 3]} │");
        Console.WriteLine($"├───┼───┼───┼───┤");
        Console.WriteLine($"│ {room[3, 0]} │ {room[3, 1]} │ {room[3, 2]} │ {room[3, 3]} │");
        Console.WriteLine($"└───┴───┴───┴───┘");
    }

    private char ToChar(Room room)
    {
        return room switch
        {
            //EntranceRoom => 'O',
            //FountainRoom => 'X',
            //EmptyRoom => ' ',
            //
            Player.Coordinate => ' ',
        };
    }
}

public abstract class Room { }
public class EmptyRoom : Room { }
public class EntranceRoom : Room { }
public class FountainRoom : Room
{
    public bool IsOn { get; set; }
}

public record Coordinate(int row, int column);
public enum Direction { North, South, East, West }