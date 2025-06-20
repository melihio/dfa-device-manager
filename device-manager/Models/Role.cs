namespace dfa_device_manager.API;

public class Role
{
    public int Id { get; set; }
    public string Name { get; set; } 
    public ICollection<Account> Accounts { get; set; } = new List<Account>();
}