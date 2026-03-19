// Third-party libraries
global using MediatR;
global using FluentValidation;

// Internal libraries
global using Blocks.MediatR;
global using Articles.Abstractions;
global using Articles.Abstractions.Enums;

// Domain
global using Submission.Domain.Entities;
global using Submission.Domain.Enums;

// Application
global using Submission.Application.Features.Shared;

// Domain StateMachines
global using Submission.Domain.StateMachines;

// Persistence
global using Submission.Persistence.Repositories;

global using AssetTypeDefinitionRepository = Blocks.EntityFrameworkCore.CachedRepository<
    Submission.Persistence.SubmissionDbContext, 
    Submission.Domain.Entities.AssetTypeDefinition, 
    Articles.Abstractions.Enums.AssetType>;