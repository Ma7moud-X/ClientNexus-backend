﻿using ClientNexus.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientNexus.Application.Interfaces
{
    public interface IDocumentTypeService
    {
        public  Task AddDocumentTypeAsync(DocumentTypeDTO dto);
        public Task DeleteDocumentTypeAsync(int DocumentTypeId);
    }
}
