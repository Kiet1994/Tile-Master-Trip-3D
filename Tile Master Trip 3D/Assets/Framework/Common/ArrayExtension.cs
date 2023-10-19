using System.Collections.Generic;
using Random = System.Random;

public static class ArrayExtension
{
    // The random number generator.
    private static Random Rand = new Random();

    // Return a random item from an array.
    public static T RandomElement<T>(this T[] items)
    {
        // Return a random item.
        return items[Rand.Next(0, items.Length)];
    }

    // Return a random item from a list.
    public static T RandomElement<T>(this List<T> items)
    {
        // Return a random item.
        return items[Rand.Next(0, items.Count)];
    }

    public static void Shuffle<T>(this IList<T> list)
    {
        Random rng = new Random();
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
}