using System.Collections;
using System.Collections.Generic;

namespace RunbookAPI.Models
{
    public interface IDataService
    {
        
        User GetUser(User user);

        bool CreateBook(Book book, int userId, int tenantId);

        Book GetBook(int id);

        IEnumerable<Book> GetAllBooks(int userId,int tenantId);

        bool CreateStage(Stage stage,int bookId);

        IEnumerable<Stage> GetAllStages(int bookId,int envId);

        bool CreateTask(Task task,string stageName,int bookId);

        IEnumerable<Task> GetAllTasks(int stageId);

        IEnumerable<Status> GetStatuses();

        bool UpdateTaskStatus(int[] taskids,int statusId);

        bool UpdateStageStatus(int id,int nextStageId,int statusId);

        IEnumerable<Environments> GetAllEnvironments(int tenantId);

        bool UpdateBookStatus(int bookId,int envId,int statusId);

        int DeleteTasks(int bookId,string taskName);

        IEnumerable<User> GetAllUsers();
    }
}