namespace DotNetChallenge.Common.Authorization
{
    public static class PolicyConstants
    {
        public const string AdminOnly = "AdminOnly";

        public const string ManageOrders = "ManageOrders";

        public const string CreateSalesOrder = "CreateSalesOrder";

        public const string InventoryManagement = "InventoryManagement";

        public const string ViewInventory = "ViewInventory";

        public const string PaymentAccess = "PaymentAccess";

        public const string ReportAccess = "ReportAccess";
    }
}
