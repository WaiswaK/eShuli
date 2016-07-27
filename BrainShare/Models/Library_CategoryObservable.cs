using System.Collections.Generic;

namespace BrainShare.Models
{
    class Library_CategoryObservable
    {
        public int category_id { get; set; }
        public string category_name { get; set; }
        public int book_count { get; set; }
        public string category_image { get; set; }
        public List<BookObservable> category_books { get; set; }
        public Library_CategoryObservable()
        {
        }
    }
}
