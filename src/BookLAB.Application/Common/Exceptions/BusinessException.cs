namespace BookLAB.Application.Common.Exceptions;

public class BusinessException : Exception
{
    public BusinessException()
        : base()
    {
    }

    public BusinessException(string message)
        : base(message)
    {
    }

    public BusinessException(string message, params object[] args)
        : base(string.Format(message, args))
    {
    }
}