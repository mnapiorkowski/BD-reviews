using System.Data;
using Dapper;

namespace TRreviews.Data;

public class ReviewerService
{
    private readonly IDbConnection _dbConnection;

    public ReviewerService(IDbConnection dbConnection) => 
        _dbConnection = dbConnection;

    public IReadOnlyList<Reviewer> GetReviewers(string searchText)
    {
        var r = _dbConnection.Query<Reviewer, int, decimal, Reviewer>(
        @"select 
            au.id as reviewerId,
	        au.name as name,
	        cast(count(*) as int) as albums_reviewed,
	        coalesce(cast(avg(mark) as decimal(3, 2)), 0) as avg_mark
        from reviewer au
        join review r on r.reviewer_id = au.id 
        group by au.id
        having lower(au.name) like lower(@Name)
        order by albums_reviewed desc, avg_mark desc;",
        (au, n, avg) =>
        {
            au.AlbumsReviewed = n;
            au.AvgMark = avg;
            return au;
        },
        new { Name = "%" + searchText + "%" },
        splitOn: "albums_reviewed, avg_mark");

        return r.ToList();
    }

    public IReadOnlyList<int> GetExpandableContent(Reviewer reviewer)
    {
        var reviewerId = reviewer.Id;
        var r = _dbConnection.Query<int, int, int, int, int, int, List<int>>(
        @"select
	        cast(sum(case when mark = 5 then 1 else 0 end) as int) as fives,
	        cast(sum(case when mark >= 4 and mark < 5 then 1 else 0 end) as int) as fours,
	        cast(sum(case when mark >= 3 and mark < 4 then 1 else 0 end) as int) as threes,
	        cast(sum(case when mark >= 2 and mark < 3 then 1 else 0 end) as int) as twos,
	        cast(sum(case when mark >= 1 and mark < 2 then 1 else 0 end) as int) as ones,
            cast(sum(case when mark is null then 1 else 0 end) as int) as nulls
        from review r where r.reviewer_id = @Id",
        (fi, fo, th, tw, on, nu) =>
        {
            List<int> l = new List<int> { fi, fo, th, tw, on, nu };
            return l;
        },
        new { Id = reviewerId },
        splitOn: "fours, threes, twos, ones, nulls");

        return r.ToList().ElementAt(0);
    }


    // public async Task<IReadOnlyList<Reviewer>> ReviewerSearch(string searchReviewer)
    // {
    //     return await Task.FromResult(GetReviewer(searchReviewer));
    // }
}
