using ClientNexus.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientNexus.Application.Interfaces
{
    public interface IPhoneNumberService
    {
        public Task AddCollectionOfPhoneNumer(ICollection<PhoneNumber> phoneNumbers, List<string> Numbers);

    }
}
