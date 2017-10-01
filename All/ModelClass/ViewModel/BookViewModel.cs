using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ModelClass.ViewModel
{
    public class BookViewModel
    {
        public BookViewModel()
        {
            Publisher = new PublisherViewModel();

            Characters = new List<CharacterViewModel>();
            for (int i = 0; i < 3; i++)
            {
                Characters.Add(new CharacterViewModel());
            }
        }

        public BookViewModel(string title, string author, string bookCoverUrl)
        {
            Title = title;
            Author = author;
            BookCoverUrl = bookCoverUrl;
        }

        public string Title { get; set; }
        public string Author { get; set; }
        [Display(Name = "Book Cover")]
        public string BookCoverUrl { get; set; }

        public PublisherViewModel Publisher { get; set; }
        public IList<CharacterViewModel> Characters { get; set; }
    }
}
