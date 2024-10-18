﻿namespace TCViettetlFC_Client.Models
{
    public class CheckoutModel
    {

        public int ProductId { get; set; }
        public string ?ProductName { get; set; }
        public decimal Price { get; set; }
        public string ?SelectedSize { get; set; }
        public string ?ProductImage { get; set; }


        public string Email { get; set; }
        public string FullName { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string District { get; set; }
        public string Ward { get; set; }
        public string ?Notes { get; set; }
        public int TotalAmount { get; set; }
        public string SelectedShipping { get; set; }
    }
}