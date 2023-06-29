﻿using Models.Baselinker;

namespace Abstractions;

public interface IBaselinkerService
{
    Task AddOrderAsync(NewOrder newOrder);
}