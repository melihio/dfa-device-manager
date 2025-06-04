using dfa_device_manager.API.DTOs.EmployeeDtos;

namespace dfa_device_manager.API.DTOs.DeviceDtos;

public class DeviceDetailsDto
{
    public string Name { get; set; }
    public string DeviceTypeName { get; set; }
    public bool IsEnabled { get; set; }
    public object? AdditionalProperties { get; set; }
    public CurrentEmployeeDto? CurrentEmployee { get; set; }
}
