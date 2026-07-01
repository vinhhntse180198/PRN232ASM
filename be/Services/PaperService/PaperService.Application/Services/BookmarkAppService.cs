using PaperService.Application.DTOs;
using PaperService.Application.Interfaces;
using PaperService.Domain.Entities;

namespace PaperService.Application.Services;

public class BookmarkAppService : IBookmarkService
{
    private readonly IUnitOfWork _unitOfWork;

    public BookmarkAppService(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<IReadOnlyList<BookmarkResponse>> GetMyBookmarksAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var bookmarks = await _unitOfWork.Bookmarks.GetByUserIdAsync(userId, cancellationToken);
        return bookmarks.Select(b => new BookmarkResponse
        {
            Id = b.Id,
            PaperId = b.PaperId,
            CreatedAt = b.CreatedAt,
            Paper = PaperMapper.Map(b.Paper, true)
        }).ToList();
    }

    public async Task<BookmarkResponse> AddAsync(Guid userId, Guid paperId, CancellationToken cancellationToken = default)
    {
        var paper = await _unitOfWork.Papers.GetByIdWithDetailsAsync(paperId, cancellationToken)
            ?? throw new InvalidOperationException("Paper not found.");

        if (await _unitOfWork.Bookmarks.ExistsAsync(userId, paperId, cancellationToken))
            throw new InvalidOperationException("Paper is already bookmarked.");

        var bookmark = new Bookmark
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            PaperId = paperId,
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Bookmarks.AddAsync(bookmark, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new BookmarkResponse
        {
            Id = bookmark.Id,
            PaperId = paperId,
            CreatedAt = bookmark.CreatedAt,
            Paper = PaperMapper.Map(paper, true)
        };
    }

    public async Task RemoveAsync(Guid userId, Guid paperId, CancellationToken cancellationToken = default)
    {
        var removed = await _unitOfWork.Bookmarks.RemoveAsync(userId, paperId, cancellationToken);
        if (!removed)
            throw new InvalidOperationException("Bookmark not found.");

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
