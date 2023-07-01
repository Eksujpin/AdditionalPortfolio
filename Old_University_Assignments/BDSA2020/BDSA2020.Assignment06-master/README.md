# Assignment #6

## C&#35; - Kanban Board part quatre

[![Simple-kanban-board-](https://upload.wikimedia.org/wikipedia/commons/thumb/d/d3/Simple-kanban-board-.jpg/512px-Simple-kanban-board-.jpg)](https://commons.wikimedia.org/wiki/File:Simple-kanban-board-.jpg "Jeff.lasovski [CC BY-SA 3.0 (https://creativecommons.org/licenses/by-sa/3.0)], via Wikimedia Commons")

Fork this repository and implement the code required for the assignments below.

### Prequel

Inspect the code for `TagRepository`, `TagRepositoryTests`, `TaskRepository`, and `TaskRepositoryTests`. They contain a sample solution for last weeks assignment.

### Exercise 1

Implement and test the `TasksController` class such that it exposes the `ITasksRepository` interface using *REST*.
Tests should use `Moq` and not a live database.

### Exercise 2

Implement and test the `TagsController` class such that it exposes the `ITagsRepository` interface using *REST*.
Tests should use `Moq` and not a live database.

### Exercise 3

Implement and test the `UsersController` class such that it exposes the `IUsersRepository` interface using *REST*.
Tests should use `Moq` and not a live database.

**Note**: You do not need an implementation of `IUsersRepository` for this.

### Exercise 4

Configure the `Startup` class with dependencies such that the system can run and targets either a *SQLite* or a *SQL Server* database.





## Software Engineering

### Exercise 1

In Section 6.4.2 [OOSE], design goals are classified into five categories: performance, dependability, cost, maintenance, and end user. Assign one or more categories to each of the following goals:

- Users must be given a feedback within 1 second after they issue any command.
- The `TicketDistributor` must be able to issue train tickets, even in the event of a network failure.
- The housing of the `TicketDistributor` must allow for new buttons to be installed in the event the number of different fares increases.
- The `AutomatedTellerMachine` must withstand dictionary attacks (i.e., users attempting to discover a identification number by systematic trial).
- The user interface of the system should prevent users from issuing commands in the wrong order.

### Exercise 2

Consider the model/view/control example depicted in Figures 6-16 and 6-15 [OOSE].  Discuss how the MVC architecture helps or hurts the following design goals.

- Extensibility (e.g., the addition of new types of views).
- Response time (e.g., the time between a user input and the time all views have been updated).
- Modifiability (e.g., the addition of new attributes in the model).
- Access control (i.e., the ability to ensure that only legitimate users can access specific parts of the model).
