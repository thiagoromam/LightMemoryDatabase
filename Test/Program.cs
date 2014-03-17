using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using LightMemoryDatabase.Api;
using Test.Database;
using Test.Stream;

namespace Test
{
    class Program
    {
        static void Main()
        {
            TestDatabase();
        }

        private static void TestDatabase()
        {
            DependencyRegistry.Register<DataContext>();
            DependencyRegistry.Register<IStorageService, StorageService>();

            //InsertBooks();
            //ReadBooks();
            //AlterBook();
            TransactionTest();

            Console.ReadLine();
        }

        private static async void InsertBooks()
        {
            var newBooks = new List<Book>();
            for (var i = 0; i < 1000; i++)
            {
                newBooks.Add(new Book
                {
                    Title = "Title " + (i + 1),
                    Authors = new List<Author>
                    {
                        new Author { Name = "Author " + (i + 1) }
                    },
                    Tags = new List<string> { "tag1", "tag2" },
                    BookSerie = new BooksSerie { Name = "Books Serie " + (i + 1) },
                });
            }

            var stopwatch = Stopwatch.StartNew();

            var context = DependencyRegistry.Resolve<DataContext>();
            var books = await context.Books;

            books.Store(newBooks);

            context.SaveDatabase();

            stopwatch.Stop();
            Console.WriteLine("books inserted");
            Console.WriteLine("stopwatch: {0}", stopwatch.ElapsedMilliseconds);
        }

        private async static void ReadBooks(int times = 0)
        {
            var stopwatch = Stopwatch.StartNew();

            var context = DependencyRegistry.Resolve<DataContext>();
            var books = await context.Books;
            var authors = await context.Authors;

            Console.WriteLine("Books: {0}", books.Count());
            Console.WriteLine("Authors: {0}", authors.Count());

            var booksFound = books.Where(f => f.BookSerie.Id > 0 && f.BookSerie.Id < 10);

            foreach (var book in booksFound)
            {
                Console.WriteLine("\nId: {0}\nTitle: {1}\nAuthors: {2}\nTags: {3}\nSerie: {4}\n",
                    book.Id,
                    book.Title,
                    book.Authors != null ? string.Join(", ", book.Authors.Select(a => a.Name)) : string.Empty,
                    string.Join(", ", book.Tags),
                    book.BookSerie != null ? book.BookSerie.Name : string.Empty
                );
            }

            stopwatch.Stop();

            Console.WriteLine("stopwatch: {0}", stopwatch.ElapsedMilliseconds);

            if (times < 1)
            {
                Console.WriteLine("\n\n\n\n");
                ReadBooks(++times);
            }
        }

        private async static void AlterBook()
        {
            var stopwatch = Stopwatch.StartNew();

            var context = DependencyRegistry.Resolve<DataContext>();
            var livros = await context.Books;

            var book1 = livros.Load(1);
            book1.Title = "Jurassic Park";
            book1.Authors.First().Name = "Michael Crichton";
            book1.BookSerie.Name = "Jurassic Park";

            var book2 = livros.Load(4);
            book2.Title = "The Lost World";
            book2.Authors.First().Name = "Michael Crichton";
            book2.BookSerie.Name = "Jurassic Park";

            context.SaveDatabase();

            stopwatch.Stop();

            Console.WriteLine("books altered");
            Console.WriteLine("stopwatch: {0}", stopwatch.ElapsedMilliseconds);
        }

        private async static void TransactionTest()
        {
            var context = DependencyRegistry.Resolve<DataContext>();
            var books = await context.Books;
            var book1 = books.First();
            var book2 = books.Skip(1).First();

            var stopwatch = Stopwatch.StartNew();
            try
            {
                await TransactionExtensions.ExecuteInTransaction(() =>
                {
                    book1.Authors.Clear();
                    //book1.Title = "test 1";
                    //book2.Title = "test 2";

                    // TODO validate when add an new author, and remove it if added to the main context

                    throw new Exception();
                });
            }
            catch (Exception e)
            {
                Console.WriteLine("erro: {0}", e.Message);
            }
            stopwatch.Stop();
            Console.WriteLine("stopwatch: {0}", stopwatch.ElapsedMilliseconds);
        }
    }
}
