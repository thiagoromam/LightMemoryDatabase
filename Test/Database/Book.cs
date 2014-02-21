using LightMemoryDatabase.Api;
using System.Collections.Generic;

namespace Test.Database
{
    public class Book : IPlainObject
    {
        [CascadeDelete]
        private IPlainObjectReference<BooksSerie> _bookSerie;
        [CascadeDelete]
        private IPlainObjectsReference<Author> _authors;
        
        public Book()
        {
            _bookSerie = this.CreateNewReference(l => l._bookSerie);
            _authors = this.CreateNewReference(l => l._authors);
        }

        public int Id { get; set; }
        public string Title { get; set; }
        public IList<Author> Authors
        {
            get { return _authors.Value; }
            set { _authors.Value = value; }
        }
        public List<string> Tags { get; set; }
        public BooksSerie BookSerie
        {
            get { return _bookSerie.Value; }
            set { _bookSerie.Value = value; }
        }
    }
}
;