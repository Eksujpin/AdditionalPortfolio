# Assignment #4

## C&#35;

Fork this repository and implement the code required for the assignments below.

### Kanban Board part deux

[![Simple-kanban-board-](https://upload.wikimedia.org/wikipedia/commons/thumb/d/d3/Simple-kanban-board-.jpg/512px-Simple-kanban-board-.jpg)](https://commons.wikimedia.org/wiki/File:Simple-kanban-board-.jpg "Jeff.lasovski [CC BY-SA 3.0 (https://creativecommons.org/licenses/by-sa/3.0)], via Wikimedia Commons")

Implement and test the `ITaskRepository` and `ITagRepository` interfaces.

```csharp
public enum Response
{
    Created,
    Updated,
    Deleted,
    NotFound,
    BadRequest,
    Conflict
}

public interface ITaskRepository
{
    (Response response, int taskId) Create(TaskCreateDTO task);
    IQueryable<TaskListDTO> Read(bool includeRemoved = false);
    TaskDetailsDTO Read(int taskId);
    Response Update(TaskUpdateDTO task);
    Response Delete(int taskId);
}

public interface ITagRepository
{
    (Response response, int taskId) Create(TagCreateDTO tag);
    IQueryable<TagDTO> Read();
    TagDTO Read(int tagId);
    Response Update(TagUpdateDTO tag);
    Response Delete(int tagId, bool force = false);
}
```

with the following rules:

#### 1. General

1. Trying to updated or delete a non-existing entity should return `NotFound`.
1. `CUD` should return a proper `Response`.
1. Your are not allowed to write `throw new ...` - use the `Response` instead.
1. Your code must use an in-memory database and/or mocks for testing.
1. If a task or tag is not found, return `null`.

#### 2. Task Repository

1. Only tasks which have the state `New` can be deleted from the database.
1. Deleting a task which is `Active` should set its state to `Removed`.
1. Deleting a task which is `Resolved`, `Closed`, or `Removed` should return `Conflict`.
1. Creating a task will set its state to `New`.
1. Create/update task must allow for editing tags.
1. Assigning a user which does not exist should return `Conflict`.
1. TaskRepository may *not* reference *TagRepository*.
1. Create/update should allow adding/removing a user - and return `BadRequest` if the user does not exist.

#### 3. Tag Repository

1. Tags which are in use may only be deleted using the `force`.
1. Trying to delete a tag in use without the `force` should return `Conflict`.
1. Trying to create a tag which exists already should return `Conflict`.







## Software Engineering

### Exercise 1

Consider a file system with a graphical user interface, such as Macintosh’s Finder, Microsoft’s Windows Explorer, or Linux’s KDE. The following objects were identified from a use case describing how to copy a file from a floppy disk to a hard disk: File, Icon, TrashCan, Folder, Disk, Pointer. Specify which are entity objects, which are boundary objects, and which are control (interactor) objects.

### Exercise 2

Assuming the same file system as before, consider a scenario consisting of selecting a file on a floppy, dragging it to Folder and releasing the mouse. Identify and define one control (interactor) object associated with this scenario.

### Exercise 3

Arrange the objects listed in Exercises SE.1-2 horizontally on a sequence diagram, the boundary objects to the left, then the control (interactor) object you identified, and finally, the entity objects. Draw the sequence of interactions resulting from dropping the file into a folder. For now, ignore the exceptional cases.

### Exercise 4

From the sequence diagram Figure 2-34, draw the corresponding class diagram. Hint: Start with the participating objects in the sequence diagram.



## Submitting the assignment

To submit the assignment you need to create a .pdf document using LaTeX containing the answers to the questions and a link to a public repository containing your fork of the completed code.

Members of the triplets should submit the same PDF file to pass the assignments. Make sure all group names and ID are clearly marked on the front page.
