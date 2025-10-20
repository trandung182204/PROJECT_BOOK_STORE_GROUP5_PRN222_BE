namespace PROJECT_BOOK_STORE_GROUP5_PRN222.DTOs
{
    public class UserDto
    {
        public string Id { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? UserName { get; set; }
        public string? Address { get; set; }
        public string? AvatarUrl { get; set; }

        public bool IsDeleted { get; set; }
        public bool IsLocked { get; set; }  // => từ LockoutEnabled/LockoutEnd
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}