using SSI_Holder.Interfaces;

namespace SSI_Holder.Services
{
    public class DefaultNavigationService : INavigationService
    {
        public async Task GoToAsync(string target)
        {
            try
            {
                await Shell.Current.GoToAsync(target);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }
        }
    }
}
