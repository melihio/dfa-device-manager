namespace dfa_device_manager.API.DTOs.DeviceDtos;

public class DeviceDto
{
    public int Id { get; set; }
    public string Name { get; set; }

    public DeviceDto(int id, string name)
    {
        Id = id;
        Name = name;
    }
}