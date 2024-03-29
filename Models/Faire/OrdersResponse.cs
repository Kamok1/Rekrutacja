﻿namespace Models.Faire;

public record OrdersResponse
{
    public int Page { get; set; }
    public int Limit { get; set; }
    public List<FaireOrder> Orders { get; set; }
}