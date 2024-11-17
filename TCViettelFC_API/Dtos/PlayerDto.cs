using System;
using System.Collections.Generic;
using TCViettelFC_API.Models;

namespace TCViettelFC_API.Dtos
{
    public class PlayerDto
    {
        public int PlayerId { get; set; }
        public string FullName { get; set; }
        public int? ShirtNumber { get; set; }
        public string Position { get; set; }
        public DateTime? JoinDate { get; set; }
        public DateTime? OutDate { get; set; }
        public string Description { get; set; }
        public int? Status { get; set; }
        public IFormFile? Avatar { get; set; }
        public int? SeasonId { get; set; }
        public string SeasonName { get; set; }
    }
    //cái này đẻ show resu
    public class ShowPlayerDtos {
        public int PlayerId { get; set; }
        public string? FullName { get; set; }
        public int? ShirtNumber { get; set; }
        public int? SeasonId { get; set; }
        public string? Position { get; set; }
        public DateTime? JoinDate { get; set; }
        public DateTime? OutDate { get; set; }
        public string? Description { get; set; }
        public int? Status { get; set; }
        public string? Avatar { get; set; }
        public string SeasonName { get; set; }
    }
    //cai nay de dien vao controller
    public class PlayerInputDto
    {
        public string FullName { get; set; }
        public int ShirtNumber { get; set; }
        public string Position { get; set; }
        public DateTime? JoinDate { get; set; }
        public DateTime? OutDate { get; set; }
        public string? Description { get; set; }
        public int Status { get; set; }
        public int SeasonId { get; set; }
        public IFormFile? Avatar { get; set; }
    }
}
