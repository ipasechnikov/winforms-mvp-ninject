namespace WinFormsMvpNinject.App.Models
{
    public class DefaultUser : IUser
    {
        public string Name { get; set; } = null!;
        public int Age { get; set; }
    }
}
