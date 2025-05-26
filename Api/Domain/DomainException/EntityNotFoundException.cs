using System;

namespace Api.Domain.DomainException;

public class EntityNotFoundException(string message="") : Exception(message)
{
}
