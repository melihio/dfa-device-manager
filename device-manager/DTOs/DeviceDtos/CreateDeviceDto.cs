using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace dfa_device_manager.API.DTOs.DeviceDtos;

public class CreateDeviceDto
{
    [Required] 
    public string Name { get; set; }

    [Required] 
    public string DeviceTypeName { get; set; }

    public bool IsEnabled { get; set; }
    
    [Required]
    public JsonElement AdditionalProperties { get; set; }
}
