namespace BasketService.Services.DiscountModel;


public class DiscountInfoModel
{
    public string Id { get; set; }
    public string Code { get; set; }
    public decimal Amount { get; set; }
    public bool Used { get; set; }
}



public class OperationResult<TData>
{
    public const string SuccessMessage = "عملیات با موفقیت انجام شد";
    public const string ErrorMessage = "عملیات با شکست مواجه شد";

    public string Message { get; set; }
    public string Title { get; set; } = null;
    public OperationResultStatus Status { get; set; }
    public bool HasError { get; set; }

    public TData Data { get; set; }
    public static OperationResult<TData> Success(TData data)
    {
        return new OperationResult<TData>()
        {
            Status = OperationResultStatus.Success,
            Message = SuccessMessage,
            Data = data,
            HasError = false,
        };
    }
    public static OperationResult<TData> NotFound()
    {
        return new OperationResult<TData>()
        {
            Status = OperationResultStatus.NotFound,
            Title = "NotFound",
            Data = default(TData),
            HasError = true,
        };
    }
    public static OperationResult<TData> Error(string message = ErrorMessage)
    {
        return new OperationResult<TData>()
        {
            Status = OperationResultStatus.Error,
            Title = "مشکلی در عملیات رخ داده",
            Data = default(TData),
            Message = message,
            HasError = true,
        };
    }
}
public class OperationResult
{
    public const string SuccessMessage = "عملیات با موفقیت انجام شد";
    public const string ErrorMessage = "عملیات با شکست مواجه شد";
    public const string NotFoundMessage = "اطلاعات یافت نشد";
    public string Message { get; set; }
    public string Title { get; set; } = null;
    public bool HasError { get; set; }
    public OperationResultStatus Status { get; set; }

    public static OperationResult Error()
    {
        return new OperationResult()
        {
            Status = OperationResultStatus.Error,
            Message = ErrorMessage,
            HasError = true
        };
    }
    public static OperationResult NotFound(string message)
    {
        return new OperationResult()
        {
            Status = OperationResultStatus.NotFound,
            Message = message,
            HasError = true,
        };
    }
    public static OperationResult NotFound()
    {
        return new OperationResult()
        {
            Status = OperationResultStatus.NotFound,
            Message = NotFoundMessage,
            HasError = true,
        };
    }
    public static OperationResult Error(string message)
    {
        return new OperationResult()
        {
            Status = OperationResultStatus.Error,
            Message = message,
            HasError = true,
        };
    }
    public static OperationResult Error(string message, OperationResultStatus status)
    {
        return new OperationResult()
        {
            Status = status,
            Message = message,
            HasError = true
        };
    }
    public static OperationResult Success()
    {
        return new OperationResult()
        {
            Status = OperationResultStatus.Success,
            Message = SuccessMessage,
            HasError = false,
        };
    }
    public static OperationResult Success(string message)
    {
        return new OperationResult()
        {
            Status = OperationResultStatus.Success,
            Message = message,
            HasError = false,
        };
    }
}


public enum OperationResultStatus
{
    Error = 10,
    Success = 200,
    NotFound = 404
}