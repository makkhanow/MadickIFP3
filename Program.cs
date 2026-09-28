using System;
using System.Collections.Generic;
using System.Linq;

public sealed record Book : IComparable<Book>, IEquatable<Book>
{
    public string Title { get; init; }
    public string Isbn { get; init; }
    public decimal Price { get; init; }
    public int StockCount { get; private init; }

    public Book(string title, string isbn, decimal price, int stockCount)
    {
        if (stockCount < 0)
            throw new ArgumentException("Stock cannot be negative.");

        Title = title;
        Isbn = isbn;
        Price = price;
        StockCount = stockCount;
    }

    public bool Equals(Book? other)
    {
        if (other is null)
            return false;

        return Isbn == other.Isbn;
    }

    public override int GetHashCode()
    {
        return Isbn.GetHashCode();
    }

    public int CompareTo(Book? other)
    {
        if (other is null)
            return 1;

        return Price.CompareTo(other.Price);
    }

    public Book Restock(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Restock quantity must be positive.");

        return this with
        {
            StockCount = StockCount + quantity
        };
    }

    public Book Sell(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Sell quantity must be positive.");

        if (quantity > StockCount)
            throw new InvalidOperationException("Cannot sell more than available stock.");

        return this with
        {
            StockCount = StockCount - quantity
        };
    }
}

public record DefaultBook(string Title, string Isbn, decimal Price, int StockCount);

public class Program
{
    public static void Main()
    {
        List<Book> catalog = new List<Book>
        {
            new Book("Refactoring", "111", 45.00m, 4),
            new Book("Clean Code", "222", 35.50m, 2),
            new Book("The Pragmatic Programmer", "333", 40.00m, 6)
        };

        // Task 1
        Console.WriteLine("TASK 1");

        Book featured = catalog[0];
        Book display = featured;

        Console.WriteLine("Shared reference demonstration:");
        Console.WriteLine("featured and display are the same object: " + ReferenceEquals(featured, display));
        Console.WriteLine("Featured stock: " + featured.StockCount);
        Console.WriteLine("StockCount cannot be changed directly because it has private init.");
        Console.WriteLine();

        DefaultBook defaultBook1 = new DefaultBook("Book A", "111", 45.00m, 4);
        DefaultBook defaultBook2 = new DefaultBook("Book B", "111", 99.99m, 100);

        Console.WriteLine("Default record equality:");
        Console.WriteLine("Same ISBN but different properties: " + defaultBook1.Equals(defaultBook2));
        Console.WriteLine("Default record equality compares all record properties.");
        Console.WriteLine();

        Book equalBook = new Book("Another Refactoring", "111", 99.99m, 100);
        Book differentBook = new Book("Another Book", "999", 99.99m, 100);

        Console.WriteLine("Custom equality:");
        Console.WriteLine("Same ISBN: " + featured.Equals(equalBook));
        Console.WriteLine("Different ISBN: " + featured.Equals(differentBook));
        Console.WriteLine();

        // Task 2
        Console.WriteLine("TASK 2");

        List<Book> sortedCatalog = new List<Book>(catalog);
        sortedCatalog.Sort();

        Console.WriteLine("Price-sorted catalogue:");

        foreach (Book book in sortedCatalog)
        {
            Console.WriteLine($"{book.Title} - {book.Price:F2} - Stock: {book.StockCount}");
        }

        Console.WriteLine();

        Book selected = catalog[0];

        Console.WriteLine("Original stock: " + selected.StockCount);

        Book restocked = selected.Restock(3);

        Console.WriteLine("After restock: " + restocked.StockCount);
        Console.WriteLine("Original book stock: " + selected.StockCount);

        Book sold = restocked.Sell(2);

        Console.WriteLine("After selling 2: " + sold.StockCount);
        Console.WriteLine("Restocked book stock: " + restocked.StockCount);

        Console.WriteLine();

        try
        {
            sold.Sell(100);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Oversell rejected: " + ex.Message);
        }

        Console.WriteLine();

        // Task 3
        Console.WriteLine("TASK 3");

        Console.WriteLine("Sorted catalogue:");

        foreach (Book book in sortedCatalog)
        {
            Console.WriteLine($"{book.Title} - {book.Price:F2} - Stock: {book.StockCount}");
        }

        Console.WriteLine();

        Book duplicate = new Book("Duplicate Refactoring", "111", 999.99m, 50);

        List<Book> catalogueCopy = new List<Book>(catalog);
        catalogueCopy.Add(duplicate);

        Console.WriteLine("Catalogue with duplicate ISBN:");
        Console.WriteLine("Count: " + catalogueCopy.Count);
        Console.WriteLine("Duplicate detected: " + duplicate.Equals(catalog[0]));
        Console.WriteLine();

        List<Book> equalsFiltered = catalogueCopy
            .Where(b => !b.Equals(duplicate))
            .ToList();

        Console.WriteLine("Using Equals-based filtering:");
        Console.WriteLine("Count after filtering: " + equalsFiltered.Count);
        Console.WriteLine("Equals removes both books with ISBN 111.");
        Console.WriteLine();

        List<Book> referenceFiltered = catalogueCopy
            .Where(b => !ReferenceEquals(b, duplicate))
            .ToList();

        Console.WriteLine("Using ReferenceEquals-based filtering:");
        Console.WriteLine("Original count: " + catalogueCopy.Count);
        Console.WriteLine("Filtered count: " + referenceFiltered.Count);

        Book original = referenceFiltered.First(b => b.Isbn == "111");

        Console.WriteLine("Original ISBN 111 price: " + original.Price);
        Console.WriteLine("Original ISBN 111 stock: " + original.StockCount);
        Console.WriteLine();

        Console.WriteLine("Original catalogue order:");

        foreach (Book book in catalog)
        {
            Console.WriteLine(book.Title);
        }

        Console.WriteLine();

        Console.WriteLine("Original selected book stock: " + selected.StockCount);
        Console.WriteLine("Restocked book stock: " + restocked.StockCount);
        Console.WriteLine("Sold book stock: " + sold.StockCount);
    }
}