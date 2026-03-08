using Articles.Abstractions.Enums;
using Blocks.Domain;

namespace Submission.Domain.Entities;

public partial class Article
{
    public void AssignAuthor(Author author, HashSet<ContributionArea> contributionAreas, bool isCorrespondingAuthor)
    {
        var role = isCorrespondingAuthor ? UserRoleType.CORAUT : UserRoleType.AUT;

        if (Actors.Exists(a => a.Person.Id == author.Id && a.Role == role))
        {
            throw new DomainException($"Author with ID {author.Id} already has the role {role} assigned to this article.");
        }

        Actors.Add(new ArticleAuthor()
        {
            ContributionAreas = contributionAreas,
            Person = author,
            Role = role
        });

        // todo - create domain event
    }

    public Asset CreateAsset(AssetTypeDefinition type)
    {
        var assetCount = _assets
            .Where(a => a.Type == type.Id)
            .Count();

        if(type.MaxAssetCount > assetCount - 1)
            throw new DomainException($"The maximum number of files allowed for {type.Name.ToString()} was already reached.");

        var asset = Asset.Create(this, type);
        _assets.Add(asset);

        return asset;
    }

    public void Approve(Person person)
    {
        Actors.Add(new ArticleActor
            {
                Person = person,
                Role = UserRoleType.REVED
            });
    }
}
