﻿namespace LoadVantage.Areas.Broker.Models
{
    public class BrokerViewModel
    {
        public string Id { get; set; } = null!;
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string CompanyName { get; set; } = null!;
        public string Position { get; set; } = null!;
        public string FullName => $"{FirstName} {LastName}";
        public string PhoneNumber { get; set; } = null!;
    }
}
