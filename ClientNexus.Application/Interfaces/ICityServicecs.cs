using ClientNexus.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientNexus.Application.Interfaces
{
    public interface ICityServicecs
    {
        public Task AddCityAsync(CityDTO cityDTO);
        public Task DeleteCityAsync(int id);
    }
}
