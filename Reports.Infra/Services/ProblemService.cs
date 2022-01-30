﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Reports.Core.Entities;
using Reports.Core.Services;
using Reports.Infra.Data;

namespace Reports.Infra.Services
{
    public class ProblemService : IProblemService
    {
        private readonly ReportsContext _context;
        private readonly IPersonService _personService;
        private readonly ISprintService _sprintService;

        public ProblemService(ReportsContext context, IPersonService personService, ISprintService sprintService)
        {
            _context = context;
            _personService = personService;
            _sprintService = sprintService;
        }

        public Guid CreateProblem(string title, string content, Guid authorId, Guid sprintId)
        {
            Person author = _personService.GetPersonById(authorId);
            Sprint sprint = _sprintService.GetSprintById(sprintId);
            DateTime now = DateTime.Now;

            Problem problem = new ()
            {
                Title = title,
                Content = content,
                Sprint = sprint,
                Created = now,
                Updated = now,
                Author = author,
                Comments = new List<Comment>(),
                State = Problem.ProblemState.Open
            };

            _context.Problems.Add(problem);
            _context.SaveChanges();
            return problem.Id;
        }

        public Problem GetProblemById(Guid id)
        {
            return _context.Problems
                       .Include(problem => problem.Sprint)
                       .Include(problem => problem.Author)
                       .Include(problem => problem.Executor)
                       .Include(problem => problem.Comments)
                       .FirstOrDefault(problem => id.Equals(problem.Id))
                   ?? throw new ArgumentException($"There is no problem with id {id}");
        }

        public IEnumerable<Problem> GetProblemsList()
        {
            return _context.Problems
                .Include(problem => problem.Sprint)
                .Include(problem => problem.Author)
                .Include(problem => problem.Executor)
                .Include(problem => problem.Comments)
                .ToList();
        }

        public IEnumerable<Problem> FindProblemsCreatedInPeriod(DateTime since, DateTime upto)
        {
            return _context.Problems
                .Include(problem => problem.Executor)
                .Where(problem => problem.Created >= since && problem.Created < upto)
                .ToList();
        }

        public IEnumerable<Problem> FindProblemsUpdatedInPeriod(DateTime since, DateTime upto)
        {
            return _context.Problems
                .Include(problem => problem.Executor)
                .Where(problem => problem.Updated >= since && problem.Updated < upto)
                .ToList();
        }

        public IEnumerable<Problem> FindProblemsByExecutor(Guid executorId)
        {
            return _context.Problems
                .Include(problem => problem.Executor)
                .Where(problem => problem.Executor != null && problem.Executor.Id.Equals(executorId))
                .ToList();
        }

        public IEnumerable<Problem> FindProblemsByExecutorDirector(Guid directorId)
        {
            return _context.Problems
                .Include(problem => problem.Executor)
                .Where(problem => problem.Executor != null && problem.Executor.Director != null && problem.Executor.Director.Id.Equals(directorId))
                .ToList();
        }

        public IEnumerable<Problem> FindProblemsBySprint(Guid sprintId)
        {
            return _context.Problems
                .Include(problem => problem.Author)
                .Include(problem => problem.Executor)
                .Include(problem => problem.Sprint)
                .Where(problem => problem.Sprint.Id.Equals(sprintId))
                .ToList();
        }

        public void EditProblem(Guid problemId, string newTitle, string newContent, Guid newSprintId)
        {
            Problem problem = GetProblemById(problemId);
            Sprint sprint = _sprintService.GetSprintById(newSprintId);
            problem.Title = newTitle;
            problem.Content = newContent;
            problem.Updated = DateTime.Now;
            problem.Sprint = sprint;
            _context.Problems.Update(problem);
            _context.SaveChanges();
        }

        public void SetState(Guid problemId, Problem.ProblemState state)
        {
            Problem problem = GetProblemById(problemId);
            problem.State = state;
            problem.Updated = DateTime.Now;
            _context.Problems.Update(problem);
            _context.SaveChanges();
        }

        public void SetExecutor(Guid problemId, Guid personId)
        {
            Problem problem = GetProblemById(problemId);
            Person person = _personService.GetPersonById(personId);
            problem.Executor = person;
            problem.Updated = DateTime.Now;
            _context.Update(person);
            _context.SaveChanges();
        }

        public void WriteComment(Guid problemId, Guid authorId, string content)
        {
            Problem problem = GetProblemById(problemId);
            Person author = _personService.GetPersonById(authorId);
            DateTime now = DateTime.Now;
            Comment comment = new ()
            {
                Author = author,
                Content = content,
                Created = now,
                Updated = now,
            };
            _context.Comments.Add(comment);
            problem.Comments.Add(comment);
            problem.Updated = now;
            _context.Problems.Update(problem);
            _context.SaveChanges();
        }

        public void EditComment(Guid problemId, Guid commentId, string newContent)
        {
            Problem problem = GetProblemById(problemId);
            Comment comment = GetCommentById(commentId);
            DateTime now = DateTime.Now;

            comment.Content = newContent;
            comment.Updated = now;
            _context.Comments.Update(comment);
            problem.Updated = now;
            _context.Problems.Update(problem);
            _context.SaveChanges();
        }

        private Comment GetCommentById(Guid id)
        {
            return _context.Comments.FirstOrDefault(comment => comment.Id.Equals(id))
                   ?? throw new ArgumentException($"There is no comment with id {id}");
        }
    }
}