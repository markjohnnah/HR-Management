﻿using AutoMapper;
using Business.Communication;
using Business.CustomException;
using Business.Domain.Models;
using Business.Domain.Repositories;
using Business.Domain.Services;
using Business.Extensions;
using Business.Resources;
using Business.Resources.Person;
using Business.Resources.Technology;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Business.Services
{
    public class PersonService : BaseService<PersonResource, CreatePersonResource, UpdatePersonResource, Person>, IPersonService
    {
        #region Constructor
        public PersonService(IPersonRepository personRepository,
            ITechnologyService technologyService,
            IOfficeRepository officeRepository,
            IGroupRepository groupRepository,
            IMapper mapper,
            IUnitOfWork unitOfWork,
            IOptionsMonitor<ResponseMessage> responseMessage) : base(personRepository, mapper, unitOfWork, responseMessage)
        {
            this._personRepository = personRepository;
            this._officeRepository = officeRepository;
            this._groupRepository = groupRepository;
            this._technologyService = technologyService;
        }
        #endregion

        #region Property
        private readonly IPersonRepository _personRepository;
        private readonly IOfficeRepository _officeRepository;
        private readonly IGroupRepository _groupRepository;
        private readonly ITechnologyService _technologyService;
        #endregion

        #region Method
        public override async Task<BaseResponse<PersonResource>> InsertAsync(CreatePersonResource createPersonResource)
        {
            try
            {
                // Validate Office is existent?
                var tempOffice = await _officeRepository.GetByIdAsync(createPersonResource.OfficeId);
                if (tempOffice is null)
                    return new BaseResponse<PersonResource>(ResponseMessage.Values["Office_NoData"]);

                // Validate Group is existent?
                if(createPersonResource.GroupId != null)
                {
                    var tempGroup = await _groupRepository.GetByIdAsync((int)createPersonResource.GroupId);
                    if (tempOffice is null)
                        return new BaseResponse<PersonResource>(ResponseMessage.Values["Group_NoData"]);
                }

                // Mapping Resource to Person
                var person = Mapper.Map<CreatePersonResource, Person>(createPersonResource);

                await _personRepository.InsertAsync(person);
                await UnitOfWork.CompleteAsync();

                // Mappping response
                var technologyResource = await _technologyService.GetAllAsync();
                var personResource = ConvertPersonResource(technologyResource.Resource, person);

                return new BaseResponse<PersonResource>(personResource);
            }
            catch (Exception ex)
            {
                throw new MessageResultException(ResponseMessage.Values["Person_Saving_Error"], ex);
            }
        }

        public override async Task<BaseResponse<PersonResource>> UpdateAsync(int id, UpdatePersonResource updatePersonResource)
        {
            try
            {
                // Validate Id is existent?
                var tempPerson = await _personRepository.GetByIdAsync(id);
                if (tempPerson is null)
                    return new BaseResponse<PersonResource>(ResponseMessage.Values["Person_Id_NoData"]);
                // Validate Office is existent?
                var tempOffice = await _officeRepository.GetByIdAsync(updatePersonResource.OfficeId);
                if (tempOffice is null)
                    return new BaseResponse<PersonResource>(ResponseMessage.Values["Office_NoData"]);

                // Mapping Resource to Person
                Mapper.Map(updatePersonResource, tempPerson);

                await UnitOfWork.CompleteAsync();

                return new BaseResponse<PersonResource>(Mapper.Map<Person, PersonResource>(tempPerson));
            }
            catch (Exception ex)
            {
                throw new MessageResultException(ResponseMessage.Values["Person_Saving_Error"], ex);
            }
        }

        public async Task<BaseResponse<PersonResource>> AssignComponentAsync(int id, ComponentResource component)
        {
            try
            {
                // Validate Id is existent?
                var tempPerson = await _personRepository.GetByIdAsync(id);
                if (tempPerson is null)
                    return new BaseResponse<PersonResource>(ResponseMessage.Values["Person_Id_NoData"]);

                tempPerson.OrderIndex = component.OrderIndex.RemoveDuplicate().ConcatenateWithComma();

                await UnitOfWork.CompleteAsync();
                // Mapping
                var resource = Mapper.Map<Person, PersonResource>(tempPerson);

                return new BaseResponse<PersonResource>(resource);
            }
            catch (Exception ex)
            {
                throw new MessageResultException(ResponseMessage.Values["Person_Updating_Error"], ex);
            }
        }

        public async Task<PaginationResponse<IEnumerable<PersonResource>>> GetPaginationAsync(QueryResource pagination, FilterPersonResource filterResource)
        {
            var totalTechnology = await _technologyService.GetAllAsync();
            var paginationPerson = await _personRepository.GetPaginationAsync(pagination, filterResource);

            // Mapping
            var tempResource = ConvertPersonResource(totalTechnology.Resource, paginationPerson.records);

            var resource = new PaginationResponse<IEnumerable<PersonResource>>(tempResource);

            // Using extension-method for pagination
            resource.CreatePaginationResponse(pagination, paginationPerson.total);

            return resource;
        }

        #region Private work
        private IEnumerable<PersonResource> ConvertPersonResource(IEnumerable<TechnologyResource> totalTechnology, IEnumerable<Person> totalPerson)
        {
            List<PersonResource> listPersonResource = new List<PersonResource>(totalPerson.Count());

            foreach (var person in totalPerson)
            {
                var tempPersonResource = ConvertPersonResource(totalTechnology, person);

                listPersonResource.Add(tempPersonResource);
            }

            return listPersonResource;
        }

        private PersonResource ConvertPersonResource(IEnumerable<TechnologyResource> totalTechnology, Person person)
        {
            var tempPersonResource = Mapper.Map<Person, PersonResource>(person);

            // Project mapping
            var listProject = person.Projects.ToList();
            var countProject = listProject.Count;
            for (int i = 0; i < countProject; i++)
                if (!string.IsNullOrEmpty(listProject?[i]?.Group?.Technology))
                    tempPersonResource.Project[i].Technology = totalTechnology.IntersectTechnology(listProject[i]?.Group.Technology);

            // Category-Person mapping
            var listCategoryPerson = person.CategoryPersons.ToList();
            var countCategoryPerson = listCategoryPerson.Count;
            for (int i = 0; i < countCategoryPerson; i++)
                if (!string.IsNullOrEmpty(listCategoryPerson?[i].Technology))
                    tempPersonResource.CategoryPerson[i].Technologies = totalTechnology.IntersectTechnology(listCategoryPerson[i].Technology);

            return tempPersonResource;
        }
        #endregion

        #endregion
    }
}
