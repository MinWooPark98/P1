using System.Collections.Generic;

public static class StaticData
{
    public static float CARD_SIZE_WIDTH = 320;
    public static float CARD_SIZE_HEIGHT = 440f;

    public static int MAX_HAND_COUNT = 10;

    public static int MAP_FLOORS = 15;
    public static int MAP_ROOMS_FLOOR = 7;
    public static int MAP_PATHS = 6;


    public static List<int> startDeckIronclad = new List<int>() { 1, 1, 1, 1, 1, 0, 0, 0, 2, 2, 2, 2, 2, 0 };
}
