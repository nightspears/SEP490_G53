using System;
using System.Collections.Generic;

namespace TCViettelFC_API.Dtos
{
    public class PlayerDto
    {
        public int PlayerId { get; set; }
        public string? FullName { get; set; }
        public int? ShirtNumber { get; set; }
        public int? SeasonId { get; set; }
        public string? Position { get; set; }
        public DateTime? JoinDate { get; set; }
        public DateTime? OutDate { get; set; }
        public string? Description { get; set; }
        public string? BackShirtImage { get; set; }
        public string? avatar { get; set; }
        public int? Status { get; set; }
    }
}
