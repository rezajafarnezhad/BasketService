namespace BasketService.Services;
public class BasketModel
{
    public Guid Id { get; set; }
    public string UserId { get; set; }
    public Guid? DiscountId { get; set; }
    public List<BasketItemModel> BasketItems { get; set; } = [];

    public decimal Total()
    {
        if (BasketItems.Count > 0)
        {
            return BasketItems.Sum(c => c.Product.UnitPrice * c.Quantity);
        }

        return 0;
    }
}

public class BasketItemModel
{
    public Guid Id { get; set; }
    public Guid BasketId { get; set; }
    public int Quantity { get; set; }
    public Guid ProductId { get; set; }
    public ProductModel Product { get; set; }
}

public class ProductModel
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; }
    public decimal UnitPrice { get; set; }
    public string ImageProduct { get; set; }
}

public class AddItemToBasketModel
{
    public Guid BasketId { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public string ImageProduct { get; set; }
}
public class CheckOutBasketModel
{
    public Guid BasketId { get; set; }
    public string UserId { get; set; }
    public string UserName { get; set; }
    public string Address { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string PostalCode { get; set; }
    public string PhoneNumber { get; set; }
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