using Business.Exceptions;
using Business.Services.Abstracts;
using Core.Models;
using Core.RepositoryAbstracts;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Services.Concretes
{
    public class TeamServices : ITeamServices
    {
        private readonly ITeamRepository _teamRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public TeamServices(ITeamRepository teamRepository, IWebHostEnvironment webHostEnvironment)
        {
            _teamRepository = teamRepository;
            _webHostEnvironment = webHostEnvironment;
        }

        public void AddTeam(Team team)
        {
            if (team == null) 
                throw new EntityNullException("","Entity not found!");

            if (team.PhotoFile == null)
                throw new FileNotFoundexception("PhotoFile", "File not found!");

            if(!team.PhotoFile.ContentType.Contains("image/"))
                throw new FileContentTypeException("PhotoFile", "File content type is not valid!");

            if (team.PhotoFile.Length > 2097152)
                throw new FileSizeException("PhotoFile", "The size of the file is large");

            string fileName=Guid.NewGuid().ToString()+Path.GetExtension(team.PhotoFile.FileName);
            string path = _webHostEnvironment.WebRootPath + @"\Uploads\Team\" + fileName;
            using(FileStream stream=new FileStream(path, FileMode.Create))
            {
                team.PhotoFile.CopyTo(stream);
            }
            team.ImgUrl = fileName;
            _teamRepository.Add(team);
            _teamRepository.Commit();
        }

        public void DeleteTeam(int id)
        {
            var team=_teamRepository.Get(x=> x.Id == id);
            if (team == null)
                throw new EntityNullException("", "Entity not found!");

            string path = _webHostEnvironment.WebRootPath + @"\Uploads\Team\" + team.ImgUrl;
            if(!File.Exists(path))
                throw new FileNotFoundexception("PhotoFile", "File not found!");
            File.Delete(path);

            _teamRepository.Delete(team);
            _teamRepository.Commit();
        }

        public List<Team> GetAllTeams(Func<Team, bool>? func = null)
        {
            return _teamRepository.GetAll(func);
        }

        public Team GetTeam(Func<Team, bool>? func = null)
        {
            return _teamRepository.Get(func);
        }

        public void UpdateTeam(int id, Team team)
        {
            var oldteam = _teamRepository.Get(x => x.Id == id);
            if (oldteam == null)
                throw new EntityNullException("", "Entity not found!");

            if(team.PhotoFile!=null)
            {
                if (!team.PhotoFile.ContentType.Contains("image/"))
                    throw new FileContentTypeException("PhotoFile", "File content type is not valid!");

                if (team.PhotoFile.Length > 2097152)
                    throw new FileSizeException("PhotoFile", "The size of the file is large");

                string path=_webHostEnvironment.WebRootPath +@"\Uploads\Team\"+oldteam.ImgUrl;
                if(!File.Exists(path))
                    throw new FileNotFoundexception("PhotoFile", "File not found!");
                File.Delete(path );

                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(team.PhotoFile.FileName);
                string path1 = _webHostEnvironment.WebRootPath + @"\Uploads\Team\" + fileName;
                using(FileStream stream=new FileStream(path1, FileMode.Create))
                {
                    team.PhotoFile.CopyTo(stream);
                }
                oldteam.ImgUrl = fileName;

            }
            oldteam.FullName = team.FullName;
            oldteam.Designation = team.Designation;
            _teamRepository.Commit();
        }
    }
}
