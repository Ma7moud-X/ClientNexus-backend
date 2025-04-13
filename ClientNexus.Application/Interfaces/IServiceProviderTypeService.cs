using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientNexus.Application.Interfaces
{
    public interface IServiceProviderTypeService
    {
        public Task AddServiceProviderTypeAsyn(string Name);
        public Task DeleteServiceProviderTypeAsync(int id);
    }
}
