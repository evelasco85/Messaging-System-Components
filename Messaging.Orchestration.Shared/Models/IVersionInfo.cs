using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messaging.Orchestration.Shared.Models
{
    public interface IVersionInfo<TVersion>
    {
        TVersion Version { get; }
    }
}
