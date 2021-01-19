using ESMS.Data.EF;
using ESMS.Data.Entities;
using ESMS.Data.Enums;
using ESMS.Utilities.Exceptions;
using ESMS.ViewModels.Project;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ESMS.Application.Projects
{
    public class ProjectService : IProjectService
    {
        private readonly ESMSDbContext _context;

        public ProjectService(ESMSDbContext context)
        {
            _context = context;
        }

        public async Task<int> Create(ProjectCreateRequest request, string empID)
        {
            var project = new Project()
            {
                ProjectName = request.ProjectName,
                Description = request.Description,
                Skateholder = request.Skateholder,
                DateCreated = DateTime.Now,
                Status = ProjectStatus.OnGoing,
                ProjectManagerID = empID
            };
            _context.Projects.Add(project);
            await _context.SaveChangesAsync();
            return project.ProjectID;
        }

        public async Task<int> Delete(int projectID)
        {
            var project = await _context.Projects.FindAsync(projectID);
            if (project == null) throw new ESMSException($"Cannot find a projectID: {projectID}");

            _context.Projects.Remove(project);
            return await _context.SaveChangesAsync();
        }

        public async Task<ProjectViewModel> GetByID(int projectID)
        {
            var project = await _context.Projects.FindAsync(projectID);
            if (project == null) throw new ESMSException($"Cannot find a projectID: {projectID}");

            var projectViewModel = new ProjectViewModel()
            {
                ProjectID = project.ProjectID,
                ProjectName = project.ProjectName,
                Description = project.Description,
                Skateholder = project.Skateholder,
                Status = project.Status
            };

            return projectViewModel;
        }

        public async Task<int> Update(ProjectUpdateRequest request)
        {
            var project = await _context.Projects.FindAsync(request.ProjectID);
            if (project == null) throw new ESMSException($"Cannot find a projectID: {request.ProjectID}");

            project.ProjectName = request.ProjectName;
            project.Description = request.Description;
            project.Skateholder = request.Skateholder;

            return await _context.SaveChangesAsync();
        }
    }
}