namespace freelancer_hub_backend.DTO_s
{
    public class UserCreateDto
    {
        public string Name { get; set; } = default!;
        public string Email { get; set; } = default!;
    }

    public class UserUpdateDto
    {
        public string Name { get; set; } = default!;
        public string Email { get; set; } = default!;
    }

    public class UserDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = default!;
        public string Email { get; set; } = default!;
        public DateTime CreatedAt { get; set; }
    }

}
