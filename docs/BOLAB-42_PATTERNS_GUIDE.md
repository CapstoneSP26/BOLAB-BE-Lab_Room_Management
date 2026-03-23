# BOLAB-42 Implementation Patterns & Guide

## 📚 CQRS & MediatR Patterns

### Pattern 1: Command Pattern (Create/Update/Delete)

#### Structure
```
Command → Validator → Handler → Repository → Response
```

#### Example: CreateGroupCommand

**File 1: CreateGroupCommand.cs**
```csharp
using MediatR;

public record CreateGroupCommand : IRequest<Guid>
{
    public string GroupName { get; init; } = string.Empty;
    public string? Description { get; init; }
}
```

**File 2: CreateGroupValidator.cs**
```csharp
using FluentValidation;

public class CreateGroupValidator : AbstractValidator<CreateGroupCommand>
{
    public CreateGroupValidator()
    {
        RuleFor(x => x.GroupName)
            .NotEmpty().WithMessage("Tên nhóm không được bỏ trống")
            .MaximumLength(150).WithMessage("Tên nhóm không được vượt quá 150 ký tự")
            .MinimumLength(2).WithMessage("Tên nhóm phải có ít nhất 2 ký tự");
    }
}
```

**File 3: CreateGroupCommandHandler.cs**
```csharp
using MediatR;

public class CreateGroupCommandHandler : IRequestHandler<CreateGroupCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public async Task<Guid> Handle(CreateGroupCommand request, CancellationToken cancellationToken)
    {
        // 1. Get current user
        var currentUserId = _currentUserService.UserId ?? Guid.Empty;

        // 2. Check authorization & business rules
        var user = await _unitOfWork.Repository<User>().GetByIdAsync(currentUserId);
        if (user == null)
            throw new NotFoundException("Người dùng không tồn tại");

        // 3. Check duplicate
        var existingGroup = await _unitOfWork.Repository<Group>().Entities
            .FirstOrDefaultAsync(g => g.OwnerId == currentUserId 
                && g.GroupName == request.GroupName 
                && !g.IsDeleted, cancellationToken);
        if (existingGroup != null)
            throw new BusinessException("Nhóm với tên này đã tồn tại");

        // 4. Create entity
        var group = new Group
        {
            Id = Guid.NewGuid(),
            GroupName = request.GroupName,
            OwnerId = currentUserId,
            IsDeleted = false,
            CreatedAt = DateTimeOffset.UtcNow,
            CreatedBy = currentUserId
        };

        // 5. Persist
        await _unitOfWork.Repository<Group>().AddAsync(group);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return group.Id;
    }
}
```

**Controller Usage**
```csharp
[HttpPost]
public async Task<IActionResult> CreateGroup([FromBody] CreateGroupCommand command, CancellationToken cancellationToken)
{
    var groupId = await _mediator.Send(command, cancellationToken);
    return CreatedAtAction(nameof(GetGroupById), new { id = groupId }, new { groupId });
}
```

---

### Pattern 2: Query Pattern (Read Operations)

#### Structure
```
Query → Handler → Repository → DTO → Response
```

#### Example: GetGroupsQuery

**File 1: GetGroupsQuery.cs**
```csharp
using MediatR;

public record GetGroupsQuery : IRequest<List<GroupDto>>
{
}

public class GroupDto
{
    public Guid Id { get; set; }
    public string GroupName { get; set; } = string.Empty;
    public Guid OwnerId { get; set; }
    public string OwnerName { get; set; } = string.Empty;
    public int MembersCount { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}
```

**File 2: GetGroupsQueryHandler.cs**
```csharp
using MediatR;

public class GetGroupsQueryHandler : IRequestHandler<GetGroupsQuery, List<GroupDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public async Task<List<GroupDto>> Handle(GetGroupsQuery request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.UserId ?? Guid.Empty;

        // 1. Fetch groups for current user
        var groups = await _unitOfWork.Repository<Group>().Entities
            .Where(g => g.OwnerId == currentUserId && !g.IsDeleted)
            .Include(g => g.User)
            .Select(g => new GroupDto
            {
                Id = g.Id,
                GroupName = g.GroupName,
                OwnerId = g.OwnerId,
                OwnerName = g.User.FullName,
                MembersCount = 0, // Will be updated
                CreatedAt = g.CreatedAt,
                UpdatedAt = g.UpdatedAt
            })
            .ToListAsync(cancellationToken);

        // 2. Fetch member counts
        var groupIds = groups.Select(g => g.Id).ToList();
        var memberCounts = await _unitOfWork.Repository<GroupMember>().Entities
            .Where(gm => groupIds.Contains(gm.GroupId))
            .GroupBy(gm => gm.GroupId)
            .Select(g => new { GroupId = g.Key, Count = g.Count() })
            .ToListAsync(cancellationToken);

        // 3. Aggregate results
        foreach (var group in groups)
        {
            var count = memberCounts.FirstOrDefault(mc => mc.GroupId == group.Id);
            group.MembersCount = count?.Count ?? 0;
        }

        return groups;
    }
}
```

**Controller Usage**
```csharp
[HttpGet]
public async Task<IActionResult> GetGroups(CancellationToken cancellationToken)
{
    var groups = await _mediator.Send(new GetGroupsQuery(), cancellationToken);
    return Ok(groups);
}
```

---

## 🔐 Authorization Pattern

### Authorization at Handler Level

```csharp
public class UpdateGroupCommandHandler : IRequestHandler<UpdateGroupCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public async Task Handle(UpdateGroupCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.UserId ?? Guid.Empty;

        // 1. Get resource
        var group = await _unitOfWork.Repository<Group>().GetByIdAsync(request.GroupId);
        if (group == null || group.IsDeleted)
            throw new NotFoundException("Nhóm không tồn tại");

        // 2. Authorize: Only owner can update
        if (group.OwnerId != currentUserId)
            throw new ForbiddenException("Bạn không có quyền cập nhật nhóm này");

        // 3. Update
        group.GroupName = request.GroupName;
        group.UpdatedAt = DateTimeOffset.UtcNow;
        group.UpdatedBy = currentUserId;

        _unitOfWork.Repository<Group>().Update(group);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
```

---

## 🛑 Error Handling Pattern

### Custom Exception Types

```csharp
// NotFoundException
throw new NotFoundException("Nhóm không tồn tại");
// → HTTP 404

// ForbiddenException
throw new ForbiddenException("Bạn không có quyền cập nhật");
// → HTTP 403

// BusinessException
throw new BusinessException("Nhóm với tên này đã tồn tại");
// → HTTP 400
```

### Exception Handling in Middleware
```
The ExceptionHandlingMiddleware automatically converts exceptions to appropriate HTTP responses and logging occurs automatically.
```

---

## 📊 Data Access Pattern

### Using UnitOfWork & Repository

```csharp
// Get single entity
var group = await _unitOfWork.Repository<Group>()
    .GetByIdAsync(requestId);

// Query with filters
var groups = await _unitOfWork.Repository<Group>().Entities
    .Where(g => g.OwnerId == userId && !g.IsDeleted)
    .Include(g => g.User)
    .ToListAsync(cancellationToken);

// Add entity
await _unitOfWork.Repository<Group>().AddAsync(group);

// Update entity
_unitOfWork.Repository<Group>().Update(group);

// Delete entity
_unitOfWork.Repository<Group>().Delete(group);

// Save changes
await _unitOfWork.SaveChangesAsync(cancellationToken);
```

---

## 📝 Validation Pattern

### FluentValidation Rules

```csharp
public class CreateGroupValidator : AbstractValidator<CreateGroupCommand>
{
    public CreateGroupValidator()
    {
        // Required field
        RuleFor(x => x.GroupName)
            .NotEmpty()
            .WithMessage("Tên nhóm không được bỏ trống");

        // Length constraints
        RuleFor(x => x.GroupName)
            .MaximumLength(150)
            .WithMessage("Tên nhóm không được vượt quá 150 ký tự");

        // Conditional validation
        RuleFor(x => x.Description)
            .MaximumLength(500)
            .When(x => !string.IsNullOrEmpty(x.Description));

        // Custom rules
        RuleFor(x => x.GroupName)
            .MinimumLength(2)
            .WithMessage("Tên nhóm phải có ít nhất 2 ký tự");
    }
}
```

### Validation Flow
```
1. Command sent to handler
2. ValidationBehavior intercepts (MediatR Pipeline)
3. Validator runs automatically
4. Returns 400 BadRequest if validation fails
5. Handler executes only if validation passes
```

---

## 🔄 Soft Delete Pattern

### Implementation

```csharp
// In Entity (Group.cs)
public class Group : BaseEntity, ISoftDeletable, IAuditable
{
    public bool IsDeleted { get; set; }
    // ... other properties
}

// In Configuration (GroupConfiguration.cs)
builder.HasQueryFilter(g => !g.IsDeleted);

// When deleting
group.IsDeleted = true;
group.UpdatedAt = DateTimeOffset.UtcNow;
group.UpdatedBy = currentUserId;
_unitOfWork.Repository<Group>().Update(group);
await _unitOfWork.SaveChangesAsync(cancellationToken);
```

### Result
```
✅ Automatically excluded from all queries via global query filter
✅ Data preserved in database
✅ Audit trail maintained
✅ Can be restored if needed
```

---

## 🏛️ Controller Pattern

### RESTful API Design

```csharp
[Authorize]  // All endpoints require authentication
[ApiController]
[Route("api/[controller]")]
public class GroupsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<GroupsController> _logger;

    // ✅ CREATE - POST returns 201
    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateGroup([FromBody] CreateGroupCommand command, CancellationToken cancellationToken)
    {
        var groupId = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetGroupById), new { id = groupId }, new { groupId });
    }

    // ✅ READ - GET returns 200
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(GroupDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetGroupById(Guid id, CancellationToken cancellationToken)
    {
        var group = await _mediator.Send(new GetGroupByIdQuery { GroupId = id }, cancellationToken);
        return Ok(group);
    }

    // ✅ UPDATE - PUT returns 204
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateGroup(Guid id, [FromBody] UpdateGroupCommand command, CancellationToken cancellationToken)
    {
        var updateCommand = command with { GroupId = id };
        await _mediator.Send(updateCommand, cancellationToken);
        return NoContent();
    }

    // ✅ DELETE - DELETE returns 204
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteGroup(Guid id, CancellationToken cancellationToken)
    {
        await _mediator.Send(new DeleteGroupCommand { GroupId = id }, cancellationToken);
        return NoContent();
    }
}
```

---

## 🧪 Testing Pattern

### Unit Test Examples

```csharp
[Test]
public async Task CreateGroup_ValidName_ReturnsGroupId()
{
    // Arrange
    var command = new CreateGroupCommand { GroupName = "Team Alpha" };
    var handler = new CreateGroupCommandHandler(_unitOfWork, _currentUserService);

    // Act
    var result = await handler.Handle(command, CancellationToken.None);

    // Assert
    Assert.That(result, Is.Not.EqualTo(Guid.Empty));
}

[Test]
public async Task CreateGroup_DuplicateName_ThrowsBusinessException()
{
    // Arrange
    var command = new CreateGroupCommand { GroupName = "Team Alpha" };
    // ... Create first group

    // Act & Assert
    Assert.ThrowsAsync<BusinessException>(async () =>
        await handler.Handle(command, CancellationToken.None)
    );
}

[Test]
public async Task UpdateGroup_NotOwner_ThrowsForbiddenException()
{
    // Arrange
    // Create group as User A
    // Try to update as User B

    // Act & Assert
    Assert.ThrowsAsync<ForbiddenException>(async () =>
        await handler.Handle(updateCommand, CancellationToken.None)
    );
}
```

---

## 🔍 Key Concepts Summary

| Concept | Usage | Benefit |
|---------|-------|---------|
| **CQRS** | Separate commands/queries | Scalability, clarity |
| **MediatR** | Mediator pattern | Decoupling, testability |
| **Repository** | Data access abstraction | Testability, flexibility |
| **UnitOfWork** | Transaction management | Consistency, atomicity |
| **Validators** | Input validation | Data integrity |
| **DTOs** | Data transfer | Encapsulation |
| **Soft Delete** | Logical deletion | Data preservation |
| **Authorization** | Handler level checks | Explicit security |
| **Async/Await** | Async operations | Performance, scalability |
| **CancellationToken** | Operation cancellation | Resource management |

---

## 📌 Best Practices Applied

✅ **Async Throughout**: All I/O operations are async  
✅ **CancellationToken Support**: Proper cancellation handling  
✅ **Proper HTTP Status Codes**: 201 Create, 204 NoContent, 400/403/404 Errors  
✅ **Authorization Checks**: Owner-based access control  
✅ **Validation**: FluentValidation for all inputs  
✅ **Error Handling**: Custom exceptions with meaningful messages  
✅ **Logging**: ILogger for debugging and monitoring  
✅ **DTOs**: Separation between internal models and API contracts  
✅ **Soft Delete**: Data preservation with IsDeleted flag  
✅ **Auditing**: CreatedAt/UpdatedAt tracking  

---

## 🚀 Integration Checklist

- [ ] Ensure appsettings.json has database connection
- [ ] Run migrations: `dotnet ef database update`
- [ ] Test with JWT Bearer token
- [ ] Verify ExceptionHandlingMiddleware is configured
- [ ] Check Authorize attribute on all endpoints
- [ ] Validate MediatR registration in DependencyInjection.cs
- [ ] Test with HTTP client or Postman

---

## 📚 Related Topics

- **Clean Architecture**: Follows Clean Architecture with CQRS
- **Repository Pattern**: Using IRepository<T> for data access
- **Service Locator**: IMediator for command/query handling
- **Dependency Injection**: All dependencies injected via constructor
- **Entity Framework Core**: Using EF Core for ORM
- **FluentValidation**: For input validation

---

**Last Updated:** March 23, 2026  
**Pattern Version:** v1.0  
**Framework:** .NET 10.0, ASP.NET Core
