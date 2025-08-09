namespace PlanyApp.Service.Interfaces
{
    public interface IPlanAccessService
    {
        Task<bool> CanViewPlanAsync(int userId, int planId);
        Task<bool> CanEditPlanAsync(int userId, int planId);
        Task<bool> LinkPlanToGroupAsync(int planId, int groupId, int userId);
    }
} 