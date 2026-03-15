using Azure.Identity;
using Blocks.Core.FluenValidation;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Submission.Application.Features.UploadFile;

public record UploadManuscriptFileCommand : ArticleCommand
{
    // The asset type of the file
    [Required]
    public AssetType AssetType { get; init; }

    // The file to be uploaded
    [Required]
    public IFormFile File { get; init; } = null!;

    public override ArticleActionType ActionType => ArticleActionType.UploadAsset;
}

public class UploadManuscriptCommandValidator : ArticleCommandValidator<UploadManuscriptFileCommand>
{
    public UploadManuscriptCommandValidator()
    {
        RuleFor(x => x.File)
            .NotNullWithMessage();

        // todo - Validate the filesize and the file exstension

        RuleFor(r => r.AssetType).Must(IsAssetTypeAllowed)
            .WithMessage(x => $"{x.AssetType} not allowed.");
    }

    private bool IsAssetTypeAllowed(AssetType assetType)
        => AllowedAssetTypes.Contains(assetType);


    public IReadOnlyCollection<AssetType> AllowedAssetTypes => new HashSet<AssetType> { AssetType.Manuscript };
}