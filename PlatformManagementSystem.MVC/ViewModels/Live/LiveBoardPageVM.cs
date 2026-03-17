public class LiveBoardPageVM
{
    public int SessionId { get; set; }
    public string CourseTitle { get; set; } = "";
    public bool IsInstructor { get; set; }

    public required List<WhiteboardEventVM> History { get; set; } 
}
