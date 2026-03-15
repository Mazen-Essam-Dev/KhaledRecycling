using Microsoft.AspNetCore.Http;

namespace Domain.DTOs.Member.Account
{
    public class FilesDTO
    {
        public string? MemberCode { get; set; }
        public string? ProfileImagePath { get; set; }
        public IFormFile? ProfileImage { get; set; }
        public string? IdImagePath { get; set; }
        public IFormFile? IdImage { get; set; }
        public string? PassportImagePath { get; set; }
        public IFormFile? PassportImage { get; set; }
    }
}