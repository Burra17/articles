using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Blocks.Domain;

public interface IAuditableAction
{
    public DateTime CreatedOn => DateTime.UtcNow;
    public int CreatedById { get; set; }
}

public interface IAuditableAction<TAction> : IAuditableAction
    where TAction : Enum
{
    TAction ActionType { get; }
}
