using System.Data;
using Dapper;

namespace TRreviews.Data;

public class AlbumService
{
    private readonly IDbConnection _dbConnection;

    public AlbumService(IDbConnection dbConnection) => 
        _dbConnection = dbConnection;
    
    public IReadOnlyList<Album> GetAlbums(int minReviews, decimal minAvgMark, string searchText)
    {
        if (String.IsNullOrEmpty(searchText) && minReviews == 0)
        {
            return GetAlbums(4, 0, "");
        }
        var al = _dbConnection.Query<Album, Artist, int, decimal, Album>(
        @"select
            albums.id as albumId,
	        albums.title as title,
            ar.id as artistId,
	        ar.name as name,
	        albums.number_of_reviews as number_of_reviews,
	        albums.avg_mark as avg_mark
        from (
	        select
		        al.id as id,
		        al.artist_id as artist_id,
		        al.title as title,
		        cast(count(*) as int) as number_of_reviews,
		        coalesce(cast(avg(mark) as decimal(3, 2)), 0) as avg_mark
	        from album al
	        join review r on al.id = r.album_id 
	        group by al.id
	        having count(*) >= @Rev 
            and avg(mark) >= @AvgMark 
            and lower(al.title) like lower(@Title)
	        order by number_of_reviews desc
        ) as albums
        join artist ar on ar.id = albums.artist_id
        order by number_of_reviews desc, avg_mark desc;",
        (al, ar, n, avg) =>
        {
            al.Artist = ar;
            al.ReviewsCount = n;
            al.AvgMark = avg;
            return al;
        },
        new { Rev = minReviews, AvgMark = minAvgMark, Title = "%" + searchText + "%" },
        splitOn: "artistId, number_of_reviews, avg_mark");

        return al.ToList();
    }


    public IReadOnlyList<Review> GetExpandableContent(Album album)
    {
        var albumId = album.Id;
        var r = _dbConnection.Query<Review, Reviewer, Review>(
        @"select 
            r.id as reviewId,
            r.mark as mark,
	        r.issue as issue, 
	        r.sect as sect, 
	        r.page as page,
            au.id as reviewerId,
            au.name as name
        from review r
        join reviewer au on au.id = r.reviewer_id
        where r.album_id = @Id
        order by issue;",
        (r, au) =>
        {
            r.Reviewer = au;
            return r;
        },
        new { Id = albumId },
        splitOn: "reviewerId");

        return r.ToList();
    }
}
