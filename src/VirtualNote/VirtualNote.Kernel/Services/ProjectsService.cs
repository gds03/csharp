using System;
using System.Collections.Generic;
using System.Linq;
using VirtualNote.Kernel.Contracts;
using VirtualNote.Kernel.DTO;
using VirtualNote.Kernel.DTO.Extensions;
using VirtualNote.Database;
using VirtualNote.Database.DomainObjects;
using VirtualNote.Kernel.Query.ConversionsDTO;
using VirtualNote.Kernel.Query.Repository;

namespace VirtualNote.Kernel.Services
{
    public sealed class ProjectsService : ServiceBase, IProjectsService
    {
        public ProjectsService(IRepository db) : base(db)
        {
            
        }

        public bool Add(ProjectServiceDTO projectDto)
        {
            if (projectDto == null)
                throw new ArgumentNullException("projectDto");

            Project dbProject = _db.Query<Project>().GetByName(projectDto.Name);

            if (dbProject != null)
                return false;

            Member responsable = _db.Query<Member>().GetById(projectDto.ResponsableId);
            Client client = _db.Query<Client>().GetById(projectDto.ClientId);
            
            _db.Insert(projectDto.CopyToDomainObject(responsable, client));
            return true;
        }

        public bool Update(ProjectServiceDTO projectDto)
        {
            if (projectDto == null)
                throw new ArgumentNullException("projectDto");

            Project dbProject = _db.Query<Project>().GetByIdIncludeResponsableAndWorkers(projectDto.ProjectID);

            Project dbProjectFind;
            if (dbProject.Name != projectDto.Name && ((dbProjectFind = _db.Query<Project>().GetByName(projectDto.Name)) != null) && dbProjectFind.ProjectID != dbProject.ProjectID)
                return false;

            // 
            // verificar se o responsavel mudou => ir a tabela intermedia workers
            // e se existir um worker com o mesmo id, retira-lo dessa tabela
            Member responsable;

            if(dbProject.Responsable.UserID != projectDto.ResponsableId)
            {
                Member possibleConflictMember = dbProject.Workers.SingleOrDefault(m => m.UserID == projectDto.ResponsableId);
                
                if(possibleConflictMember != null)
                {
                    dbProject.Workers.Remove(possibleConflictMember);
                }

                responsable = _db.Query<Member>().GetById(projectDto.ResponsableId);
            }else
            {
                responsable = dbProject.Responsable;
            }

            
            Client client = _db.Query<Client>().GetById(projectDto.ClientId);

            dbProject.UpdateDomainObject(projectDto, responsable, client);
            return true;
        }


        public bool Remove(int projectId, out string projectName)
        {
            bool hasIssues;
            Project dbProject = _db.Query<Project>().GetHasIssues(projectId, out hasIssues);
            projectName = dbProject.Name;

            if(!hasIssues)
            {
                _db.Delete(dbProject);
                return true;
            }

            dbProject.Enabled = false;
            return false;
        }



        public void Assign(ProjectServiceAssignWorkersDTO infoDto, out string projectName)
        {
            if (infoDto == null)
                throw new ArgumentNullException("infoDto");

            Project dbProject = _db.Query<Project>().GetByIdIncludeResponsableAndWorkers(infoDto.ProjectId);
            dbProject.Workers.Clear();

            projectName = dbProject.Name;
            if (infoDto.workersIds.Count() > 0)
            {
                IEnumerable<Member> workers = _db.Query<Member>().GetByIdBundle(infoDto.workersIds);
                foreach (Member m in workers)
                    dbProject.Workers.Add(m);
            }
        }
    }
}
