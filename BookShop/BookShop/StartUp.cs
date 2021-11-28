using BookShop.Models.Enums;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using BookShop.Models;
using Microsoft.EntityFrameworkCore;

namespace BookShop
{

    using Data;

    using Initializer;

    public class StartUp
    {
        public static void Main(string[] args)
        {
            using var db = new BookShopContext();
            DbInitializer.ResetDatabase(db);

            //string input = Console.ReadLine();
            //Console.WriteLine(GetBooksByAgeRestriction(db, input));

            //Console.WriteLine(GetGoldenBooks(db));

            //Console.WriteLine(GetBooksByPrice(db));

            //int year = int.Parse(Console.ReadLine());
            //Console.WriteLine(GetBooksNotReleasedIn(db, year));

            //string endsWith = Console.ReadLine();
            //Console.WriteLine(GetAuthorNamesEndingIn(db, endsWith));

            //Console.WriteLine(CountCopiesByAuthor(db));

            //int lengthCheck = int.Parse(Console.ReadLine());
            //CountBooks(db, lengthCheck);

            //string input = Console.ReadLine();
            //string result = GetBooksByCategory(db, input);
            //Console.WriteLine(result);

            //Console.WriteLine(GetTotalProfitByCategory(db));

            //Console.WriteLine(GetMostRecentBooks(db));

            //Stopwatch sw = new Stopwatch();
            //sw.Start();
            //IncreasePrices(db);
            //Console.WriteLine(sw.ElapsedMilliseconds);

            //Console.WriteLine(RemoveBooks(db));

            //string input = Console.ReadLine();
            //Console.WriteLine(GetBooksByAuthor(db, input));

            string input = Console.ReadLine();
            Console.WriteLine(GetBookTitlesContaining(db, input));

            //string date = Console.ReadLine();
            //Console.WriteLine(GetBooksReleasedBefore(db, date));
        }

        public static string GetBooksByAgeRestriction(BookShopContext context, string command)
        {
            StringBuilder sb = new StringBuilder();

            AgeRestriction ageRestriction = Enum.Parse<AgeRestriction>(command, true);

            string[] books = context
                             .Books
                             .Where(b => b.AgeRestriction == ageRestriction)
                             .OrderBy(b => b.Title)
                             .Select(b => b.Title)
                             .ToArray();

            foreach (string book in books)
            {
                sb.AppendLine(book);
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetGoldenBooks(BookShopContext context)
        {
            StringBuilder sb = new StringBuilder();

            string[] books = context
                             .Books
                             .Where(b => b.EditionType == EditionType.Gold &&
                                         b.Copies < 5000)
                             .OrderBy(b => b.BookId)
                             .Select(b => b.Title)
                             .ToArray();

            foreach (string titel in books)
            {
                sb.AppendLine(titel);
            }



            return sb.ToString().TrimEnd();
        }

        public static string GetBooksByPrice(BookShopContext context)
        {
            StringBuilder sb = new StringBuilder();

            var books = context
                             .Books
                             .Where(b => b.Price > 40)
                             .OrderByDescending(b => b.Price)
                             .Select(b => new
                             {
                                 b.Title,
                                 b.Price
                             })
                             .ToList();

            foreach (var book in books)
            {
                sb.AppendLine($"{book.Title} - ${book.Price:f2}");
            }



            return sb.ToString().TrimEnd();

        }

        public static string GetBooksNotReleasedIn(BookShopContext context, int year)
        {
            StringBuilder sb = new StringBuilder();

            string[] books = context
                             .Books
                             .Where(b => (b.ReleaseDate.HasValue && b.ReleaseDate.Value.Year != year) || !b.ReleaseDate.HasValue)
                             .OrderBy(b => b.BookId)
                             .Select(b => b.Title)
                             .ToArray();

            foreach (string titel in books)
            {
                sb.AppendLine(titel);
            }



            return sb.ToString().TrimEnd();

        }

        public static string GetBooksByCategory(BookShopContext context, string input)
        {
            StringBuilder sb = new StringBuilder();

            var inputStrings = input.Split(" ", StringSplitOptions.RemoveEmptyEntries).ToArray();

            var books = context
                        .BooksCategories
                        .Select(bc => new
                        {
                            CategoryName = bc.Category.Name,
                            BookTitle = bc.Book.Title
                        })
                        .OrderBy(b => b.BookTitle)
                        .ToArray();

            List<string> result = new List<string>();

            foreach (var book in books)
            {
                for (int i = 0; i < inputStrings.Length; i++)
                {
                    if (book.CategoryName.ToLower() == inputStrings[i].ToLower())
                    {
                        result.Add(book.BookTitle);
                    }
                }
            }

            foreach (var s in result)
            {
                sb.AppendLine(s);
            }
            return sb.ToString().TrimEnd();

        }

        public static string GetBooksReleasedBefore(BookShopContext context, string date)
        {
            StringBuilder sb = new StringBuilder();

            string[] dateStrings = date.Split("-").ToArray();
            int day = int.Parse(dateStrings[0]);
            int month = int.Parse(dateStrings[1]);
            int year = int.Parse(dateStrings[2]);
            DateTime givenDate = new DateTime(year, month, day);
            

            var books = context
                        .Books
                        .Where(b => b.ReleaseDate.HasValue && 
                                        b.ReleaseDate < givenDate)
                        .OrderByDescending(b => b.ReleaseDate)
                        .Select(b => new
                        {
                            b.Title,
                            b.EditionType,
                            b.Price
                        })
                        .ToList();

            foreach (var book in books)
            {
                sb.AppendLine($"{book.Title} - {book.EditionType} - ${book.Price:f2}");
            }

            return sb.ToString().TrimEnd();

        }

        public static string GetAuthorNamesEndingIn(BookShopContext context, string input)
        {
            StringBuilder sb = new StringBuilder();

            var autors = context
                         .Authors
                         .ToArray()
                         .Where(a => a.FirstName.ToLower().EndsWith(input.ToLower()))
                         .Select(a => $"{a.FirstName} {a.LastName}")
                         .OrderBy(n => n)
                         .ToArray();

            foreach (string autor in autors)
            {
                sb.AppendLine(autor);
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetBooksByAuthor(BookShopContext context, string input)
        {
            StringBuilder sb = new StringBuilder();

            var booksByAuthors = context
                                .Books
                                .Where(b => b.Author.LastName.ToLower().StartsWith(input.ToLower()))
                                .OrderBy(b => b.BookId)
                                .Select(b => new
                                {
                                    BookTitle = b.Title,
                                    AuthorsName = b.Author.FirstName + " " + b.Author.LastName
                                })
                                .ToList();

            foreach (var book in booksByAuthors)
            {
                sb.AppendLine($"{book.BookTitle} ({book.AuthorsName})");
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetBookTitlesContaining(BookShopContext context, string input)
        {
            StringBuilder sb = new StringBuilder();

            var bookTitles = context
                             .Books
                             .ToArray()
                             .Where(b => b.Title.ToLower().Contains(input.ToLower()))
                             .OrderBy(b => b.Title)
                             .Select(b => b.Title)
                             .ToArray();

            foreach (var bookTitle in bookTitles)
            {
                sb.AppendLine($"{bookTitle}");
            }

            return sb.ToString().TrimEnd();
        }

        public static int CountBooks(BookShopContext context, int lengthCheck)
        {
            var booksSum = context
                           .Books
                           .ToArray()
                           .Where(b => b.Title.Length > lengthCheck);

            int result = booksSum.Count();
            Console.WriteLine($"There are {result} books with longer title than {lengthCheck} symbols");

            return result;
        }

        public static string CountCopiesByAuthor(BookShopContext context)
        {
            StringBuilder sb = new StringBuilder();

            var autorsWhithBookCopies = context
                                        .Authors
                                        .Include(a => a.Books)
                                        .Select(a => new
                                        {
                                            FullName = a.FirstName + " " + a.LastName,
                                            TotalBookCopies = a.Books.Sum(b => b.Copies)
                                        })
                                        .OrderByDescending(a => a.TotalBookCopies)
                                        .ToArray();

            foreach (var autor in autorsWhithBookCopies)
            {
                sb.AppendLine($"{autor.FullName} - {autor.TotalBookCopies}");
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetTotalProfitByCategory(BookShopContext context)
        {
            StringBuilder sb = new StringBuilder();

            var categoriesByProfit = context
                                     .Categories
                                     .Select(c => new
                                     {
                                         CategoryName = c.Name,
                                         TotalProfit = c.CategoryBooks.Sum(cb => cb.Book.Copies * cb.Book.Price)
                                     })
                                     .OrderByDescending(c => c.TotalProfit)
                                     .ThenBy(c => c.CategoryName)
                                     .ToArray();

            foreach (var category in categoriesByProfit)
            {
                sb.AppendLine($"{category.CategoryName} ${category.TotalProfit:f2}");
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetMostRecentBooks(BookShopContext context)
        {
            StringBuilder sb = new StringBuilder();

            var categoriesByMostRecentBooks = context
                              .Categories
                              .Select(c => new
                              {
                                  CategoryName = c.Name,
                                  MostRecentBooks = c.CategoryBooks
                                                     .Select(cb => cb.Book)
                                                     .OrderByDescending(b => b.ReleaseDate)
                                                     .Select(b => new
                                                     {
                                                         BookTitle = b.Title,
                                                         ReleaseYear = b.ReleaseDate.Value.Year
                                                     })
                                                     .Take(3)
                                                     .ToArray()
                              })
                              .OrderBy(c => c.CategoryName)
                              .ToArray();

            foreach (var category in categoriesByMostRecentBooks)
            {
                sb.AppendLine($"--{category.CategoryName}");
                foreach (var book in category.MostRecentBooks)
                {
                    sb.AppendLine($"{book.BookTitle} ({book.ReleaseYear})");
                }
            }

            return sb.ToString().TrimEnd();
        }

        public static void IncreasePrices(BookShopContext context)
        {
            IQueryable<Book> allBooksBefore2010 = context
                                                  .Books
                                                  .Where(b => b.ReleaseDate.HasValue &&
                                                              b.ReleaseDate.Value.Year < 2010);

            foreach (var book in allBooksBefore2010)
            {
                book.Price += 5;
            }

            context.SaveChanges();
        }

        public static int RemoveBooks(BookShopContext context)
        {

            var booksToRemove = context
                                .Books
                                .Where(b => b.Copies < 4200)
                                .ToArray();

            context.BulkDelete(booksToRemove);

            int result = booksToRemove.Length;
            return result;
        }

    }
}
