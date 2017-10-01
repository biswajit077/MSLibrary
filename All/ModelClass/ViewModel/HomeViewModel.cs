using System.Collections.Generic;

namespace ModelClass.ViewModel
{
	public class HomeViewModel
	{
		public HomeViewModel()
		{
			ClassicBooks = new List<BookViewModel>();
			NewBooks = new List<BookViewModel>();
		}

		public IList<BookViewModel> ClassicBooks { get; set; }
		public IList<BookViewModel> NewBooks { get; set; }

		public void PopulateClassicBooks()
		{
			ClassicBooks = new[]
			{
				new BookViewModel("For Whom The Bells Toll", "Ernest Hemmingway", 
					"http://ecx.images-amazon.com/images/I/71ahJkud8uL.jpg"),
				new BookViewModel("Crime and Punishment", "Fyodor Dostoyevsky",
					"http://upload.wikimedia.org/wikipedia/en/4/4b/Crimeandpunishmentcover.png"),
				new BookViewModel("Slaughterhouse Five", "Kurt Vonnegut",
					"http://ecx.images-amazon.com/images/I/4120yizU-2L._SY344_BO1,204,203,200_.jpg"),
				new BookViewModel("1984", "George Orwell",
					"http://media.philly.com/images/1984-book.jpg"),
				new BookViewModel("The Bonfire of the Vanities", "Tom Wolfe",
					"http://khofler.files.wordpress.com/2011/11/tumblr_kxdl36zdu91qaouh8o1_4002.jpeg")
			};
		}
	}
}
