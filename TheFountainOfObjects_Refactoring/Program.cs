new FountainOfObjectsGame().Run();

public class FountainOfObjectsGame
{
    public Map Map { get; }
    public Player Player { get; }
    public Sense Sense { get; }
    public FountainRoom fountainRoom = new FountainRoom();
    

    public FountainOfObjectsGame()
    {
        Map = new Map();
        Player = new Player();
        Sense = new Sense();
    }

    public void Run()
    {
        Console.WriteLine("You enter the Cavern of Objects, a maze of rooms filled with dangerous pits in search");
        Console.WriteLine("of the Fountain of Objects.");
        Console.WriteLine("Light is visible only in the entrance, and no other light is seen anywhere in the caverns.");
        Console.WriteLine("You must navigate the Caverns with your other senses.");
        Console.WriteLine("Find the Fountain of Objects, activate it, and return to the entrance.");
        Console.WriteLine("");
        Console.WriteLine("Look out for pits. You will feel a breeze if a pit is in an adjacent room. If you enter a room with a pit, you will die.");
        Console.WriteLine("Maelstroms are violent forces of sentient wind. Entering a room with one could transport you to any other location in the caverns. You will be able to hear their growling and groaning in nearby rooms.");
        Console.WriteLine("Amaroks roam the caverns. Encountering one is certain death, but you can smell their rotten stench in nearby rooms.");
        Console.WriteLine("");
        Console.WriteLine("You carry with you a bow and a quiver of arrows.");
        Console.WriteLine("You can use them to shoot monsters in the caverns but be warned: you have a limited supply");

        while (true)
        {
            Coordinate currentCoordinate = Player.Coordinate;
            
            Map.Render(Player);
            if (Player.IsAlive == false)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("You died.");
                Console.ForegroundColor = ConsoleColor.White;
                break;
            }

            else if (Player.IsAlive == true && fountainRoom.IsOn && Map.GetRoom(Player.Coordinate) is EntranceRoom)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Congratulations! You won!");
                Console.ForegroundColor = ConsoleColor.White;
                break;
            }
            ICommand command = GetCommand();
            command.Execute(this);
            ICommand GetCommand()
            {
                while (true)
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine("What do you want to do? ");
                    string? input = Console.ReadLine();
                    if (input == "north") return new Move(Direction.North);
                    if (input == "south") return new Move(Direction.South);
                    if (input == "east") return new Move(Direction.East);
                    if (input == "west") return new Move(Direction.West);
                    if (input == "shoot north" && Map.Arrows > 0)
                    {
                        Map.Arrows--;
                        if (currentCoordinate.Row < 1) { Console.Clear(); Console.WriteLine("Trying to shoot a wall, wise guy?"); Map.Render(Player); continue; }
                        else if ((Map.GetRoom(Player.Coordinate.Row - 1, Player.Coordinate.Column) is AmarokRoom)) return new Shoot(Direction.North);
                        else { Console.Clear(); Console.WriteLine("You missed!"); Map.Render(Player); continue; }

                    }
                    if (input == "shoot south" && Map.Arrows > 0)
                    {
                        Map.Arrows--;
                        if (currentCoordinate.Row == Map.Rows - 1) { Console.Clear(); Console.WriteLine("Trying to shoot a wall, wise guy?"); Map.Render(Player); continue; }
                        else if ((Map.GetRoom(Player.Coordinate.Row + 1, Player.Coordinate.Column) is AmarokRoom)) return new Shoot(Direction.South);
                        else { Console.Clear(); Console.WriteLine("You missed!"); Map.Render(Player); continue; }
                    }
                    if (input == "shoot east" && Map.Arrows > 0)
                    {
                        Map.Arrows--;
                        if (currentCoordinate.Column == Map.Columns - 1) { Console.Clear(); Console.WriteLine("Trying to shoot a wall, wise guy?"); Map.Render(Player); continue; }
                        else if ((Map.GetRoom(Player.Coordinate.Row, Player.Coordinate.Column + 1) is AmarokRoom)) return new Shoot(Direction.East);
                        else { Console.Clear(); Console.WriteLine("You missed!"); Map.Render(Player); continue; }
                    }
                    if (input == "shoot west" && Map.Arrows > 0)
                    {
                        Map.Arrows--;
                        if (currentCoordinate.Column < 1) { Console.Clear(); Console.WriteLine("Trying to shoot a wall, wise guy?"); Map.Render(Player); continue; }
                        else if (Map.GetRoom(Player.Coordinate.Row, Player.Coordinate.Column - 1) is AmarokRoom) return new Shoot(Direction.West);
                        else { Console.Clear(); Console.WriteLine("You missed!"); Map.Render(Player); continue; }
                    }
                    if ((input == "shoot north" && Map.Arrows == 0) || (input == "shoot south" && Map.Arrows == 0) || (input == "shoot east" && Map.Arrows == 0) || (input == "shoot west" && Map.Arrows == 0))
                    {
                        Console.Clear();
                        Console.WriteLine("You are out of arrows!");
                        Map.Render(Player);
                        continue;
                    }
                    if (input == "on") return new EnableFountain();
                    if (input == "off") return new DisableFountain();
                    if (input == "help") { Console.Clear(); Help(); Map.Render(Player); continue; }
                    Console.WriteLine($"I did not understand '{input}'.");
                }
            }
            Sense.SenseCheck(this, Player);
        }
    }
    public void Help()
    {
        Console.WriteLine("Possible directions: north / south / east / west");
        Console.WriteLine("To shoot type: shoot north / south / east / west");
        Console.WriteLine("When in fountain room: on / off");
    }
}
public interface ISense
{
    void SenseCheck(FountainOfObjectsGame game, Player player);
}
public class Sense : ISense
{
    public void SenseCheck(FountainOfObjectsGame game, Player player)
    {
        Room playersRoom = game.Map.GetRoom(player.Coordinate);

        if (playersRoom is EntranceRoom)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("You see light in this room coming from outside the cavern. This is the entrance.");
        }
        else if (playersRoom is FountainRoom && game.fountainRoom.IsOff)
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("You hear water dripping in this room. The Fountain of Objects is here!");
        }
        else if (playersRoom is FountainRoom && game.fountainRoom.IsOn)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("You hear the rushing waters from the Fountain of Objects. It has been reactivated!");
        }

        if (playersRoom is PitRoom)
        {
            player.IsAlive = false;
        }

        if (playersRoom is AmarokRoom)
        {
            player.IsAlive = false;
        }

        if (game.Map.PitInVicinity(player, playersRoom) == true)
        {
            Console.WriteLine("You feel a draft. There is a pit in a nearby room.");
        }

        if (game.Map.MaelstromInVicinity(player, playersRoom) == true)
        {
            Console.WriteLine("You hear the growling and groaning of a maelstrom nearby.");
        }

        if (game.Map.AmarokInVicinity(player, playersRoom) == true)
        {
            Console.WriteLine("You can smell the rotten stench of an amarok in a nearby room.");
        }


        if (playersRoom is MaelstromRoom)
        {
            game.Map.MaelstromMove(player);
            Coordinate currentCoordinate = game.Player.Coordinate;
            Coordinate newCoordinate = new Coordinate(currentCoordinate.Row - 1, currentCoordinate.Column + 2);

            if (game.Map.OnMap(newCoordinate))
                game.Player.Coordinate = newCoordinate;
            else
            {
                newCoordinate = new Coordinate(currentCoordinate.Row - 1, currentCoordinate.Column + 1);
                if (game.Map.OnMap(newCoordinate))
                    game.Player.Coordinate = newCoordinate;
                else
                {
                    newCoordinate = new Coordinate(currentCoordinate.Row - 1, currentCoordinate.Column);
                    if (game.Map.OnMap(newCoordinate))
                        game.Player.Coordinate = newCoordinate;
                    else
                    {
                        game.Player.Coordinate = new Coordinate(0, 0);
                    }
                }
            }
            Console.WriteLine("You got into maelstrom! Check where you happen to be now...");
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

public class Shoot : ICommand
{
    public Direction Direction { get; }

    public Shoot(Direction direction)
    {
        Direction = direction;
    }

    public void Execute(FountainOfObjectsGame game)
    {
        Coordinate currentCoordinate = game.Player.Coordinate;
        Room playersRoom = game.Map.GetRoom(game.Player.Coordinate);

        if (game.Map.CanShoot(game.Player, playersRoom))
        {
            Room clearRoom = Direction switch
            {
                Direction.North => game.Map.SetRoom(currentCoordinate.Row - 1, currentCoordinate.Column, new EmptyRoom()),
                Direction.South => game.Map.SetRoom(currentCoordinate.Row + 1, currentCoordinate.Column, new EmptyRoom()),
                Direction.East => game.Map.SetRoom(currentCoordinate.Row, currentCoordinate.Column + 1, new EmptyRoom()),
                Direction.West => game.Map.SetRoom(currentCoordinate.Row, currentCoordinate.Column - 1, new EmptyRoom()),
                _ => throw new NotImplementedException(),
            };
            Console.Clear();
            Console.WriteLine("You killed an Amarok!");
            game.Map.SetUpAmarokVicinity();
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
    public int Arrows { get; set; }

    public Map()
    {
        Arrows = 5;
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

        if (input == 4)
        {
            _rooms[3, 3] = new FountainRoom();
            _rooms[2, 1] = new PitRoom();
            _rooms[0, 2] = new MaelstromRoom();
            _rooms[2, 2] = new AmarokRoom();
        }
        else if (input == 6)
        {
            _rooms[5, 5] = new FountainRoom();
            _rooms[1, 1] = new PitRoom();
            _rooms[3, 2] = new MaelstromRoom();
            _rooms[4, 5] = new AmarokRoom();
        }
        else if (input == 8)
        {
            _rooms[0, 6] = new FountainRoom();
            _rooms[1, 4] = new PitRoom();
            _rooms[3, 3] = new PitRoom();
            _rooms[4, 7] = new PitRoom();
        }

        for (int row = 0; row < Rows; row++)
            for (int column = 0; column < Columns; column++)
                if (_rooms[row, column] is PitRoom)
                {
                    try { _rooms[row - 1, column - 1].IsAdjacentToPitRoom = true; } catch { }
                    try { _rooms[row - 1, column].IsAdjacentToPitRoom = true; } catch { }
                    try { _rooms[row - 1, column + 1].IsAdjacentToPitRoom = true; } catch { }
                    try { _rooms[row, column - 1].IsAdjacentToPitRoom = true; } catch { }
                    try { _rooms[row, column + 1].IsAdjacentToPitRoom = true; } catch { }
                    try { _rooms[row + 1, column - 1].IsAdjacentToPitRoom = true; } catch { }
                    try { _rooms[row + 1, column].IsAdjacentToPitRoom = true; } catch { }
                    try { _rooms[row + 1, column + 1].IsAdjacentToPitRoom = true; } catch { }
                }

        for (int row = 0; row < Rows; row++)
            for (int column = 0; column < Columns; column++)
                if (_rooms[row, column] is AmarokRoom)
                {
                    try { _rooms[row - 1, column - 1].IsAdjacentToAmarokRoom = true; } catch { }
                    try { _rooms[row - 1, column].IsAdjacentToAmarokRoom = true; } catch { }
                    try { _rooms[row - 1, column + 1].IsAdjacentToAmarokRoom = true; } catch { }
                    try { _rooms[row, column - 1].IsAdjacentToAmarokRoom = true; } catch { }
                    try { _rooms[row, column + 1].IsAdjacentToAmarokRoom = true; } catch { }
                    try { _rooms[row + 1, column - 1].IsAdjacentToAmarokRoom = true; } catch { }
                    try { _rooms[row + 1, column].IsAdjacentToAmarokRoom = true; } catch { }
                    try { _rooms[row + 1, column + 1].IsAdjacentToAmarokRoom = true; } catch { }

                    try { _rooms[row - 1, column].IsAmarokShootingZone = true; } catch { }
                    try { _rooms[row + 1, column].IsAmarokShootingZone = true; } catch { }
                    try { _rooms[row, column - 1].IsAmarokShootingZone = true; } catch { }
                    try { _rooms[row, column + 1].IsAmarokShootingZone = true; } catch { }
                }

        SetUpMaelstromVicinity();
    }

    public void SetUpMaelstromVicinity()
    {
        for (int row = 0; row < Rows; row++)
            for (int column = 0; column < Columns; column++)
                if (_rooms[row, column] is MaelstromRoom)
                {
                    try { _rooms[row - 1, column - 1].IsAdjacentToMaelstromRoom = true; } catch { }
                    try { _rooms[row - 1, column].IsAdjacentToMaelstromRoom = true; } catch { }
                    try { _rooms[row - 1, column + 1].IsAdjacentToMaelstromRoom = true; } catch { }
                    try { _rooms[row, column - 1].IsAdjacentToMaelstromRoom = true; } catch { }
                    try { _rooms[row, column + 1].IsAdjacentToMaelstromRoom = true; } catch { }
                    try { _rooms[row + 1, column - 1].IsAdjacentToMaelstromRoom = true; } catch { }
                    try { _rooms[row + 1, column].IsAdjacentToMaelstromRoom = true; } catch { }
                    try { _rooms[row + 1, column + 1].IsAdjacentToMaelstromRoom = true; } catch { }
                }
    }

    public void SetUpAmarokVicinity()
    {
        for (int row = 0; row < Rows; row++)
            for (int column = 0; column < Columns; column++)
            {
                _rooms[row, column].IsAdjacentToAmarokRoom = false;

                if (_rooms[row, column] is AmarokRoom)
                {
                    try { _rooms[row - 1, column - 1].IsAdjacentToAmarokRoom = true; } catch { }
                    try { _rooms[row - 1, column].IsAdjacentToAmarokRoom = true; } catch { }
                    try { _rooms[row - 1, column + 1].IsAdjacentToAmarokRoom = true; } catch { }
                    try { _rooms[row, column - 1].IsAdjacentToAmarokRoom = true; } catch { }
                    try { _rooms[row, column + 1].IsAdjacentToAmarokRoom = true; } catch { }
                    try { _rooms[row + 1, column - 1].IsAdjacentToAmarokRoom = true; } catch { }
                    try { _rooms[row + 1, column].IsAdjacentToAmarokRoom = true; } catch { }
                    try { _rooms[row + 1, column + 1].IsAdjacentToAmarokRoom = true; } catch { }

                    try { _rooms[row - 1, column].IsAmarokShootingZone = true; } catch { }
                    try { _rooms[row + 1, column].IsAmarokShootingZone = true; } catch { }
                    try { _rooms[row, column - 1].IsAmarokShootingZone = true; } catch { }
                    try { _rooms[row, column + 1].IsAmarokShootingZone = true; } catch { }
                }
            }
    }

    public void MaelstromMove(Player player)
    {
        Room playersRoom = GetRoom(player.Coordinate);
        for (int row = 0; row < Rows; row++)
            for (int column = 0; column < Columns; column++)
            {
                try { _rooms[row, column].IsAdjacentToMaelstromRoom = false; } catch { }

                if ((_rooms[row, column] is MaelstromRoom) && (playersRoom is MaelstromRoom))
                {

                    _rooms[row, column] = new EmptyRoom();
                    row = row + 1;
                    column = column - 2;

                    if (row < 0 || column < 0)
                    {
                        _rooms[0, 5] = new MaelstromRoom();
                    }
                    else
                    {
                        SetRoom(row, column, new MaelstromRoom());
                    }
                }
            }
        SetUpMaelstromVicinity();
    }

    public bool PitInVicinity(Player player, Room room)
    {
        Room playersRoom = GetRoom(player.Coordinate);
        if ((playersRoom == room) && (room.IsAdjacentToPitRoom == true))
            return true;
        return false;
    }

    public bool MaelstromInVicinity(Player player, Room room)
    {
        Room playersRoom = GetRoom(player.Coordinate);
        if ((playersRoom == room) && (room.IsAdjacentToMaelstromRoom == true))
            return true;
        return false;
    }

    public bool AmarokInVicinity(Player player, Room room)
    {
        Room playersRoom = GetRoom(player.Coordinate);
        if ((playersRoom == room) && (room.IsAdjacentToAmarokRoom == true))
            return true;
        return false;
    }

    public bool CanShoot(Player player, Room room)
    {
        Room playersRoom = GetRoom(player.Coordinate);
        if ((playersRoom == room) && (room.IsAmarokShootingZone == true))
            return true;
        return false;
    }



    public Room GetRoom(int row, int column) => _rooms[row, column];
    public Room GetRoom(Coordinate coordinate) => _rooms[coordinate.Row, coordinate.Column];
    public Room SetRoom(int row, int column, Room room)
    {
        return _rooms[row, column] = room;
    }

    public bool OnMap(Coordinate coordinate)
    {
        if ((coordinate.Row <= -1) || (coordinate.Column <= -1)) return false;
        else if ((coordinate.Row >= Rows) || (coordinate.Column >= Columns)) return false;
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
            Console.WriteLine($"│ {room[1, 0]} │ {room[1, 1]} │ {room[1, 2]} │ {room[1, 3]} │ {room[1, 4]} │ {room[1, 5]} │  Possible directions:");
            Console.WriteLine($"├───┼───┼───┼───┼───┼───┤  north / south / east / west");
            Console.WriteLine($"│ {room[2, 0]} │ {room[2, 1]} │ {room[2, 2]} │ {room[2, 3]} │ {room[2, 4]} │ {room[2, 5]} │  When in fountain room:");
            Console.WriteLine($"├───┼───┼───┼───┼───┼───┤  on / off ");
            Console.WriteLine($"│ {room[3, 0]} │ {room[3, 1]} │ {room[3, 2]} │ {room[3, 3]} │ {room[3, 4]} │ {room[3, 5]} │  To shoot type:");
            Console.WriteLine($"├───┼───┼───┼───┼───┼───┤  shoot north/south/east/west");
            Console.WriteLine($"│ {room[4, 0]} │ {room[4, 1]} │ {room[4, 2]} │ {room[4, 3]} │ {room[4, 4]} │ {room[4, 5]} │");
            Console.WriteLine($"├───┼───┼───┼───┼───┼───┤  Arrows left: {Arrows}");
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

public abstract class Room
{
    public bool IsAdjacentToPitRoom { get; set; }
    public bool IsAdjacentToMaelstromRoom { get; set; }
    public bool IsAdjacentToAmarokRoom { get; set; }
    public bool IsAmarokShootingZone { get; set; }
}
public class EmptyRoom : Room { }
public class EntranceRoom : Room { }
public class FountainRoom : Room
{
    public bool IsOn { get; set; }
    public bool IsOff => !IsOn;
}
public class PitRoom : Room { }
public class MaelstromRoom : Room { }
public class AmarokRoom : Room { }

public record Coordinate(int Row, int Column)
{
    public sealed override string ToString()
    {
        return $"You are at ({Row}, {Column})";
    }
}
public enum Direction { North, South, East, West }