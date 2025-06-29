public class GetGroupsNamesResponseDto
{
    public class GroupNameDto
    {
        public Guid GroupId { get; set; }
        public string Name { get; set; }
    }

    public List<GroupNameDto> Groups { get; set; }
}