using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BokuNoGame2.IntegrationServices
{
    public interface IBaseIntegrationService
    {
        string ExternalSystemDescriptor { get; }
        Task GetActualDataAsync();
        Task SaveChangesAsync();
    }
}
