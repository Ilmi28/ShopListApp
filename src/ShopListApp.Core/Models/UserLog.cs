﻿using ShopListApp.Core.Enums;

namespace ShopListApp.Core.Models
{
    public class UserLog
    {
        public int Id { get; set; }
        public required string UserId { get; set; }
        public Operation Operation { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
    }
}
