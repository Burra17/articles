using FastEndpoints;
using FluentValidation;
using System.ComponentModel.DataAnnotations;

namespace Journals.API.Features.Journals.Create;

public record CreateJournalCommand(string Name, string Abbrevation, string Description, string ISSN, int ChiefEditorId)
{

}

public class CreateJournalValidator : Validator<CreateJournalCommand>
{
    public CreateJournalValidator()
    {
        RuleFor(c => c.Name).NotEmpty();
        RuleFor(c => c.Abbrevation).NotEmpty();
        RuleFor(c => c.Description).NotEmpty();
        RuleFor(c => c.ISSN).NotEmpty().Matches(@"\d{4}-\d{3}[\dX]").WithMessage("Invalid ISSN format");
        RuleFor(c => c.ChiefEditorId).GreaterThan(0);
    }
}