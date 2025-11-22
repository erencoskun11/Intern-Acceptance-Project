using Application.DTOs.InternshipApplication;
using Application.Interfaces;
using Application.Services.Interfaces;
using AutoMapper;
using Domain.Entities;

namespace Application.Services
{
    public class InternshipApplicationService : GenericService<InternshipApplication, InternshipApplicationListDto, InternshipApplicationCreateDto, InternshipApplicationUpdateDto>, IInternshipApplicationService
    {
        public InternshipApplicationService(IGenericRepository<InternshipApplication> repository, IMapper mapper)
            : base(repository, mapper)
        {
        }
    }
}
