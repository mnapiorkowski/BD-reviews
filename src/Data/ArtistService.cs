using System.Data;
using Dapper;

namespace TRreviews.Data;

public class ArtistService
{
    private readonly IDbConnection _dbConnection;

    public ArtistService(IDbConnection dbConnection) => 
        _dbConnection = dbConnection;
    
    public IReadOnlyList<Artist> GetArtists(int minReviews, string searchText)
    {
        if (String.IsNullOrEmpty(searchText) && minReviews == 0)
        {
            return GetArtists(20, "");
        }
        var ar = _dbConnection.Query<Artist, int, Artist>(
        @"select 
            ar.id as artistId,
	        ar.name as name,
	        cast(count(*) as int) as number_of_reviews
        from (
            select
		        al.artist_id as artist_id
	        from album al 
	        join review r on al.id = r.album_id 
	    ) as reviews
        join artist ar on reviews.artist_id = ar.id
        group by ar.id
        having count(*) >= @Min
        and lower(ar.name) like lower(@Name)
        order by number_of_reviews desc;",
        (ar, n) =>
        {
            ar.ReviewsCount = n;
            return ar;
        },
        new { Min = minReviews, Name = "%" + searchText + "%" },
        splitOn: "number_of_reviews");

        return ar.ToList();
    }

    public IReadOnlyList<Album> GetExpandableContent(Artist artist)
    {
        var artistId = artist.Id;
        var r = _dbConnection.Query<Album, int, decimal, Album>(
        @"select 
            albums.id as albumId,
	        albums.title,
	        cast(count(*) as int) as number_of_reviews,
	        coalesce(cast(avg(mark) as decimal(3, 2)), 0) as avg_mark
        from 
	        (select
		        al.id as id,
		        al.title as title
	        from album al 
	        join artist ar on al.artist_id = ar.id 
	        where ar.id = @Id) as albums
        join review r on r.album_id = albums.id
        group by albums.id, albums.title
        order by number_of_reviews desc, avg_mark desc;",
        (al, n, avg) =>
        {
            al.ReviewsCount = n;
            al.AvgMark = avg;
            return al;
        },
        new { Id = artistId },
        splitOn: "number_of_reviews, avg_mark");

        return r.ToList();
    }
}
