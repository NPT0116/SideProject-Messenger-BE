using System;
using System.Collections.Generic;
using System.Linq;

namespace Infrastructure.Seed;
public static class DataGenerator
{
    private static readonly Random Random = new();

    public static string RandomString(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        return new string(Enumerable.Repeat(chars, length).Select(s => s[Random.Next(s.Length)]).ToArray());
    }

    public static DateTime RandomDate(DateTime start, DateTime end)
    {
        var range = (end - start).Days;
        return start.AddDays(Random.Next(range));
    }

    public static Guid RandomGuid() => Guid.NewGuid();
}
