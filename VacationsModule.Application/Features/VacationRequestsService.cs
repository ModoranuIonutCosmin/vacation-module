using AutoMapper;
using VacationsModule.Application.Auth;
using VacationsModule.Application.DTOs;
using VacationsModule.Application.Interfaces.Repositories;
using VacationsModule.Domain.Datamodels;
using VacationsModule.Domain.DomainServices.Interfaces;
using VacationsModule.Domain.Entities;
using VacationsModule.Domain.Exceptions;
using VacationsModule.Domain.Models;

namespace VacationsModule.Application.Features;

public class VacationRequestsService : IVacationRequestsService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEmployeesRepository _employeesRepository;
    private readonly IVacationRequestsRepository _vacationRequestsRepository;
    private readonly IAuthorizationService _authorizationService;
    private readonly IVacationRequestValidator _vacationRequestValidator;
    private readonly IMapper _mapper;
    
    public VacationRequestsService(IUnitOfWork unitOfWork,
        IEmployeesRepository employeesRepository,
        IVacationRequestsRepository vacationRequestsRepository,
        IAuthorizationService authorizationService,
        IVacationRequestValidator vacationRequestValidator,
        IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _employeesRepository = employeesRepository;
        _vacationRequestsRepository = vacationRequestsRepository;
        _authorizationService = authorizationService;
        _vacationRequestValidator = vacationRequestValidator;
        _mapper = mapper;
    }

    public async Task<CreateVacationRequestResponse> CreateVacationRequest(Guid requestingUserId, CreateVacationRequestRequest request)
    {
        
        var employeeId = requestingUserId;
        
        if (!await _authorizationService.HasSpecificRoles(employeeId, RolesEnum.Employee.ToString()))
        {
            throw new UnauthorizedAccessException("You are not authorized to create vacation requests!");
        }
        
        Employee employee = await _employeesRepository.GetEmployeeByUserIdEagerAsync(employeeId);

        List<VacationRequestInterval> vacationDateIntervals = _mapper.Map<List<DateInterval>, List<VacationRequestInterval>>(request.RequestedDateIntervals);

        var vacationRequest = new VacationRequest
        {
            Employee = employee,
            Status = VacationRequestStatus.Pending,
            CreatedAt = DateTimeOffset.UtcNow,
            VacationIntervals = vacationDateIntervals,
            Description = request.Description,
            Comments = new List<VacationRequestComment>(),
        };
        
        var validationResult = _vacationRequestValidator.ValidateVacationRequest(vacationRequest);
        
        if (validationResult.Issues.Any())
        {
            throw new VacationRequestValidationException(issues: validationResult.Issues);
        }

        await _vacationRequestsRepository.AddAsync(vacationRequest);
        await _unitOfWork.CommitTransaction();

        return _mapper.Map<VacationRequest, CreateVacationRequestResponse>(vacationRequest);
    }
    
    
    // Update Vacation Request
    public async Task<UpdateVacationRequestResponse> UpdateVacationRequest(Guid requestingUserId, UpdateVacationRequestRequest request)
    {
        var employeeId = requestingUserId;
        
        if (!await _authorizationService.HasSpecificRoles(employeeId, RolesEnum.Employee.ToString()))
        {
            throw new UnauthorizedAccessException("You are not authorized to create vacation requests!");
        }
        
        Employee employee = await _employeesRepository.GetEmployeeByUserIdEagerAsync(employeeId);
        
        var vacationRequest = await _vacationRequestsRepository.GetByIdAsync(request.VacationRequestId);
        var submissionTime = DateTimeOffset.UtcNow;

        if (!vacationRequest.Employee.Id.Equals(employee.Id))
        {
            throw new UnauthorizedAccessException("You are not authorized to update this vacation request!");
        }

        List<VacationRequestInterval> vacationDateIntervals 
            = _mapper.Map<List<DateInterval>, List<VacationRequestInterval>>(request.RequestedDateIntervals);

        vacationRequest.VacationIntervals = vacationDateIntervals;
        vacationRequest.Description = request.Description;
        vacationRequest.UpdatedAt = submissionTime;
        
        if (request.ExtraComment != null && !string.IsNullOrWhiteSpace(request.ExtraComment.Message))
        {
            
            if (vacationRequest.Comments == null)
            {
                vacationRequest.Comments = new List<VacationRequestComment>();
            }
            
            vacationRequest.Comments.Add(new VacationRequestComment
            {
                Author = vacationRequest.Employee,
                Message = request.ExtraComment.Message,
                PostedAt = submissionTime,
                VacationRequest = vacationRequest,
            });
        }
        
        var validationResult = _vacationRequestValidator.ValidateVacationRequest(vacationRequest);
        
        if (!validationResult.IsValidated)
        {
            throw new VacationRequestValidationException(issues: validationResult.Issues);
        }

        await _vacationRequestsRepository.UpdateAsync(vacationRequest);
        await _unitOfWork.CommitTransaction();

        return new UpdateVacationRequestResponse
        {
            VacationRequestId = vacationRequest.Id,
            EmployeeId = vacationRequest.Employee.Id,
            RequestedDateIntervals = request.RequestedDateIntervals,
            Description = vacationRequest.Description,
            LastUpdate = vacationRequest.UpdatedAt ?? DateTimeOffset.UtcNow,
        };
    }
    
   
    
    
    public async Task<GetVacationRequestsPaginatedResponse> GetVacationRequestsByStatusAndEmployeeId(Guid requestingUserId, 
        GetVacationRequestsPaginatedRequest requestPaginated)
    {
        //TODO: E manager sau employee
        
        var employeeId = requestingUserId;

        List<VacationRequest> vacationRequests;
        var isManager = await _authorizationService.HasSpecificRoles(employeeId, RolesEnum.Manager.ToString());
        var isEmployee = await _authorizationService.HasSpecificRoles(employeeId, RolesEnum.Employee.ToString());
        
        if (isManager)
        {  
            //managerul poate vedea toate cererile deci se poate pasa si employeeId-ul cerut pentru care sa vede cererile
            //daca nu se specifica employeeId-ul, se vor vedea toate cererile de la toti angajatii
            vacationRequests =  await _vacationRequestsRepository.GetVacationRequestsByStatusAndEmployeeIdPaginated(
                requestPaginated.Status,
                employeeId: requestPaginated.EmployeeId, requestPaginated.Page, requestPaginated.PageSize);
        }
        else if (isEmployee)
        {
            //limiteaza la employeeId-ul userului logat
            vacationRequests =  await _vacationRequestsRepository.GetVacationRequestsByStatusAndEmployeeIdPaginated(
                requestPaginated.Status,
                employeeId: employeeId, requestPaginated.Page, requestPaginated.PageSize);
        }
        else
        {
            throw new UnauthorizedAccessException("You are not authorized to view vacation requests!");
        }

        
        return new GetVacationRequestsPaginatedResponse()
        {
            VacationRequests =  _mapper.Map<List<VacationRequest>, List<VacationRequestModel>>(vacationRequests)
        };
    }

    public async Task<VacationRequestModel> GetVacationRequestById(Guid requestingUserId, Guid vacationRequestId)
    {

        VacationRequest vacationRequest;
        
        if (await _authorizationService.HasSpecificRoles(requestingUserId, RolesEnum.Manager.ToString()))
        {
            vacationRequest = await _vacationRequestsRepository.GetByIdAsync(vacationRequestId);
        }
        else if (await _authorizationService.HasSpecificRoles(requestingUserId, RolesEnum.Employee.ToString()))
        {
            vacationRequest = await _vacationRequestsRepository
                .GetVacationRequestByEmployeeIdAndVacationRequestId(vacationRequestId: vacationRequestId, employeeId: requestingUserId);
        }
        else
        {
            throw new UnauthorizedAccessException("You are not authorized to view vacation requests!");
        }
        
        return _mapper.Map<VacationRequest, VacationRequestModel>(vacationRequest);
    }
}