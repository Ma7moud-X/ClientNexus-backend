using ClientNexus.Application.Interfaces;
using ClientNexus.Domain.Entities;
using ClientNexus.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientNexus.Application.Services
{
    public class PhoneNumberService : IPhoneNumberService
    {
        private readonly IUnitOfWork unitOfWork;
        public PhoneNumberService(IUnitOfWork unitOfWork)
        {

            this.unitOfWork = unitOfWork;
        }

        public async Task AddCollectionOfPhoneNumer(ICollection<PhoneNumber> phoneNumbers, List<string> Numbers)
        {
            foreach (var Number in Numbers)
            {
                PhoneNumber phoneNumber = new PhoneNumber()
                {
                    Number = Number,
                };
                phoneNumbers.Add(phoneNumber);
            }
            await unitOfWork.SaveChangesAsync();
        }
    }
}
