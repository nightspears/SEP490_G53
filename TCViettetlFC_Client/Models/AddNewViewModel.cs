﻿namespace TCViettetlFC_Client.Models
{
    public class AddNewViewModel
    {
        public int? creatorId { get; set; }
        public int? newsCategoryId { get; set; }
        public string? title { get; set; }
        public string? content { get; set; }
        public string? image { get; set; }
        public DateTime? createdAt { get; set; }
        public int? status { get; set; }
    }
}
