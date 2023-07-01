using BDSA2020.Assignment04.Entities;
using System.Collections.Generic;

namespace BDSA2020.Assignment04.Models.Tests
{
    public static class TestDataGenerator
    {
        public static void GenerateTestData(this KanbanContext context)
        {
            //generate tags
            var tagMedium = new Tag
            {
                Id = 1,
                Name = "medium difficulty",
                Tasks = new List<TaskTag>()
            };

            var tagHard = new Tag
            {
                Id = 2,
                Name = "ROUGH LIFE difficulty",
                Tasks = new List<TaskTag>()
            };

            var tagBDSA = new Tag
            {
                Id = 3,
                Name = "what is this difficulty",
                Tasks = new List<TaskTag>()
            };


            // Generate task
            var taskOne = new Task
            {
                Id = 1,
                Title = "Clean kitty litter",
                State = State.New,
                Tags = new List<TaskTag>()
            };
            var tasktwo = new Task
            {
                Id = 2,
                Title = "Schedule party",
                State = State.Active,
                Tags = new List<TaskTag>()
            };
            var taskThree = new Task
            {
                Id = 3,
                Title = "Procrastinate homework",
                State = State.Resolved,
                Tags = new List<TaskTag>()
            };
            var taskFour = new Task
            {
                Id = 4,
                Title = "Do the dishes",
                State = State.Closed,
                Tags = new List<TaskTag>()
            };
            var taskFive = new Task
            {
                Id = 5,
                Title = "Watch new netflix show",
                State = State.Removed,
                Tags = new List<TaskTag>()
            };

            // Generate Users
            var userOne = new User
            {
                Id = 1,
                EmailAddress = "pfurlong1@unc.edu",
                Name = "Pascal Furlong",
                Tasks = new List<Task>()
            };

            var userTwo = new User
            {
                Id = 2,
                EmailAddress = "aswire0@stumbleupon.com",
                Name = "Allie Swire",
                Tasks = new List<Task>()
            };

            //link task 2 and tag 2 together
            var setTaskTag = new TaskTag
            {
                Tag = tagHard,
                TagId = tagHard.Id,
                Task = tasktwo,
                TaskId = tasktwo.Id
            };

            tagHard.Tasks = new List<TaskTag>
                {
                    setTaskTag
                };
            tasktwo.Tags = new List<TaskTag>
                {
                    setTaskTag
                };
            tasktwo.AssignedToId = 2;
            tasktwo.AssignedTo = userTwo;

            context.Tasks.AddRange(taskOne, tasktwo, taskThree, taskFour, taskFive);
            context.Tags.AddRange(tagMedium, tagHard, tagBDSA);
            context.Users.AddRange(userOne, userTwo);
            context.SaveChanges();
        }

    }

}
