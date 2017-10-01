using System.Collections.Generic;

namespace ModelClass.ViewModel
{
	public class PublisherViewModel
	{
		public PublisherViewModel()
		{
			Books = new List<PublisherBookViewModel>();
			for ( int i = 0; i < 3; i++ )
			{
				Books.Add( new PublisherBookViewModel() );
			}
		}
		public string Name { get; set; }

		public IList<PublisherBookViewModel> Books { get; set; }
	}
}
