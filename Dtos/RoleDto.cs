namespace AbcLettingAgency.Dtos;

public sealed record RoleDto(string Name, string[] Permissions);
public sealed record CreateRoleRequest(string Name, string[]? Permissions);
public sealed record RenameRoleRequest(string NewName);
public sealed record SetPermissionsRequest(string[] Permissions); 
public sealed record AddPermissionsRequest(string[] Permissions);
