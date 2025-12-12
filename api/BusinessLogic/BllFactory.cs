using System;
using Microsoft.Extensions.DependencyInjection;

namespace api.BusinessLogic
{
    public class BllFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public BllFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public ICourseBll CreateCourseBll(BllMode mode)
        {
            return mode switch
            {
                BllMode.Linq => _serviceProvider.GetRequiredService<CourseLinqBll>(),
                BllMode.Sproc => _serviceProvider.GetRequiredService<CourseSpBll>(),
                _ => throw new ArgumentException($"Unknown BLL mode: {mode}")
            };
        }

        public IStudentBll CreateStudentBll(BllMode mode)
        {
            return mode switch
            {
                BllMode.Linq => _serviceProvider.GetRequiredService<StudentLinqBll>(),
                BllMode.Sproc => _serviceProvider.GetRequiredService<StudentSpBll>(),
                _ => throw new ArgumentException($"Unknown BLL mode: {mode}")
            };
        }

        public IAdminBll CreateAdminBll(BllMode mode)
        {
            return mode switch
            {
                BllMode.Linq => _serviceProvider.GetRequiredService<AdminLinqBll>(),
                BllMode.Sproc => _serviceProvider.GetRequiredService<AdminSpBll>(),
                _ => throw new ArgumentException($"Unknown BLL mode: {mode}")
            };
        }
    }

    public enum BllMode
    {
        Linq,
        Sproc
    }
}
